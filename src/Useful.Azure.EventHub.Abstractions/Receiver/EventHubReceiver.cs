using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Useful.Azure.EventHub.Abstractions.Receiver
{
    public class EventHubReceiver : IEventHubReceiver
    {
        private readonly EventHubsConnectionStringBuilder _connectionString;
        private IList<PartitionReceiver> _receiver;
        private readonly EventHubClient _eventHubClient;
        private string _consumerGroupName = PartitionReceiver.DefaultConsumerGroupName;

        public EventHubReceiver(EventHubsConnectionStringBuilder connectionString)
        {
            _connectionString = connectionString;
            _eventHubClient = EventHubClient.Create(connectionString);
        }

        public async Task CreateReceivers(EventPosition eventPosition, string consumerGroupName = null, ReceiverOptions receiverOptions = null)
        {
            if (!string.IsNullOrWhiteSpace(consumerGroupName))
            {
                _consumerGroupName = consumerGroupName;
            }

            var info = await _eventHubClient.GetRuntimeInformationAsync();
            _receiver = new List<PartitionReceiver>(info.PartitionIds.Length);
            foreach (var id in info.PartitionIds)
            {
                _receiver.Add(_eventHubClient.CreateReceiver(_consumerGroupName, id, eventPosition, receiverOptions));
            }
        }

        public async Task<IEnumerable<EventData>> ReceiveAsync(int maxMessageCount, TimeSpan waitTime)
        {
            var results = await Task.WhenAll(from partitionReceiver in _receiver
                                             select partitionReceiver.ReceiveAsync(maxMessageCount, waitTime));

            return results.SelectMany(x => x);
        }

        public async Task<IEnumerable<EventData>> ReceiveAsync(int maxMessageCount)
        {
            var results = await Task.WhenAll(from partitionReceiver in _receiver
                                             select partitionReceiver.ReceiveAsync(maxMessageCount));

            return results.SelectMany(x => x);
        }

        public async Task<IEnumerable<T>> ReceiveAsync<T>(int maxMessageCount, TimeSpan waitTime)
        {
            var results = await Task.WhenAll(from partitionReceiver in _receiver
                                             select partitionReceiver.ReceiveAsync(maxMessageCount, waitTime));

            return results.SelectMany(x => x).ConvertEventData<T>();
        }

        public async Task<IEnumerable<T>> ReceiveAsync<T>(int maxMessageCount)
        {
            var results = await Task.WhenAll(from partitionReceiver in _receiver
                                             select partitionReceiver.ReceiveAsync(maxMessageCount));

            return results.SelectMany(x => x).ConvertEventData<T>();
        }

        public IObservable<T> Receive<T>(TimeSpan ago, int messageCountPerRun = 10)
        {
            var ob = Observable.Create<T>(async o =>
            {
                var results = await Task.WhenAll(from partitionReceiver in _receiver
                                                 select partitionReceiver.ReceiveAsync(messageCountPerRun));

                try
                {
                    foreach (var eventData in results.SelectMany(x => x).Where(x => x.SystemProperties.EnqueuedTimeUtc >= DateTime.UtcNow.Subtract(ago)).ConvertEventData<T>())
                    {
                        o.OnNext(eventData);
                    }
                }
                catch (Exception e)
                {
                    o.OnError(e);
                }
            });

            return ob;
        }

        public IObservable<T> ReceiveProcessor<T>(Action<string> onError, Action<string> onEvent, string storageConnectionString, string storageContainerName)
        {
            var eventProcessorHost = new EventProcessorHost(
                _connectionString.EntityPath,
                _consumerGroupName,
                _connectionString.ToString(),
                storageConnectionString,
                storageContainerName);

            var options = new EventProcessorOptions { EnableReceiverRuntimeMetric = true };

            var ob = Observable.Create<T>(async o =>
            {
                var factory = new InternalEventProcessorFactory(x => OnMessage(x, o), onEvent, onError);
                await eventProcessorHost.RegisterEventProcessorFactoryAsync(factory, options);

                return Disposable.Create(async () => { await eventProcessorHost.UnregisterEventProcessorAsync(); });
            });

            void OnMessage(IEnumerable<EventData> eventDataList, IObserver<T> observer)
            {
                try
                {
                    foreach (var eventData in eventDataList)
                    {
                        if (eventData?.Body.Array != null)
                        {
                            observer.OnNext(eventData.ConvertEventData<T>());
                        }
                    }
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                }
            }

            return ob;
        }
    }
}