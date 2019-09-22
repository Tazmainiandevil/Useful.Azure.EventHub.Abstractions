using Microsoft.Azure.EventHubs;
using Useful.Azure.EventHub.Abstractions.Receiver;
using Useful.Azure.EventHub.Abstractions.Sender;

namespace Useful.Azure.EventHub.Abstractions.Factory
{
    /// <summary>
    /// Event hub factory
    /// </summary>
    public interface IEventHubFactory
    {
        IEventHubSender CreateEventHubSender(string connectionString, string eventHubName);
        IEventHubSender CreateEventHubSender(EventHubsConnectionStringBuilder connectionStringBuilder);
        IEventHubReceiver CreateEventHubReceiver(string connectionString, string eventHubName, string consumerGroupName = null, EventPosition eventPosition = null, ReceiverOptions receiverOptions = null);
        IEventHubReceiver CreateEventHubReceiver(EventHubsConnectionStringBuilder connectionStringBuilder, string consumerGroupName = null, EventPosition eventPosition = null, ReceiverOptions receiverOptions = null);
    }
}