using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Useful.Azure.EventHub.Abstractions.Receiver
{
    public interface IEventHubReceiver
    {
        Task CreateReceivers(EventPosition eventPosition, string consumerGroupName = null, ReceiverOptions receiverOptions = null);

        Task<IEnumerable<EventData>> ReceiveAsync(int maxMessageCount, TimeSpan waitTime);

        Task<IEnumerable<EventData>> ReceiveAsync(int maxMessageCount);

        Task<IEnumerable<T>> ReceiveAsync<T>(int maxMessageCount, TimeSpan waitTime);

        Task<IEnumerable<T>> ReceiveAsync<T>(int maxMessageCount);

        IObservable<T> Receive<T>(TimeSpan ago, int messageCountPerRun = 10);

        IObservable<T> ReceiveProcessor<T>(Action<string> onError, Action<string> onEvent, string storageConnectionString, string storageContainerName);
    }
}