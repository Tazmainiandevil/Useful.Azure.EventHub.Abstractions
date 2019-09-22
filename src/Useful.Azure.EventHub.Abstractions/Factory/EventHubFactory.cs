using Microsoft.Azure.EventHubs;
using System;
using Useful.Azure.EventHub.Abstractions.Receiver;
using Useful.Azure.EventHub.Abstractions.Sender;

namespace Useful.Azure.EventHub.Abstractions.Factory
{
    public class EventHubFactory : IEventHubFactory
    {
        public IEventHubSender CreateEventHubSender(string connectionString, string eventHubName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("To create an event hub sender the connection string cannot be null or empty", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(eventHubName))
            {
                throw new ArgumentException("To create an event hub sender the event hub name cannot be null or empty", nameof(eventHubName));
            }

            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString)
            {
                EntityPath = eventHubName
            };

            var sender = new EventHubSender(connectionStringBuilder);

            return sender;
        }

        public IEventHubSender CreateEventHubSender(EventHubsConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder), "To create an event hub sender the connection string builder cannot be null");
            }

            var sender = new EventHubSender(connectionStringBuilder);

            return sender;
        }

        public IEventHubReceiver CreateEventHubReceiver(string connectionString, string eventHubName, string consumerGroupName = "$Default", EventPosition eventPosition = null, ReceiverOptions receiverOptions = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("To create an event hub receiver the connection string cannot be null or empty", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(consumerGroupName))
            {
                throw new ArgumentException("To create an event hub receiver the consumer group name cannot be null or empty", nameof(consumerGroupName));
            }

            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString)
            {
                EntityPath = eventHubName
            };

            var receiver = new EventHubReceiver(connectionStringBuilder);
            receiver.CreateReceivers(eventPosition ?? EventPosition.FromStart(), consumerGroupName, receiverOptions).GetAwaiter().GetResult();

            return receiver;
        }

        public IEventHubReceiver CreateEventHubReceiver(EventHubsConnectionStringBuilder connectionStringBuilder, string consumerGroupName = null, EventPosition eventPosition = null, ReceiverOptions receiverOptions = null)
        {
            if (connectionStringBuilder == null)
            {
                throw new ArgumentNullException(nameof(connectionStringBuilder), "To create an event hub receiver the connection string builder cannot be null");
            }

            if (string.IsNullOrWhiteSpace(consumerGroupName))
            {
                throw new ArgumentException("To create an event hub receiver the consumer group name cannot be null or empty", nameof(consumerGroupName));
            }

            var receiver = new EventHubReceiver(connectionStringBuilder);
            receiver.CreateReceivers(eventPosition ?? EventPosition.FromStart(), consumerGroupName, receiverOptions).GetAwaiter().GetResult();

            return receiver;
        }
    }
}