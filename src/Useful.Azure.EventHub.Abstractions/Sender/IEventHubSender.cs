using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace Useful.Azure.EventHub.Abstractions.Sender
{
    public interface IEventHubSender
    {
        /// <summary>
        /// Asynchronously sends event data to an Event Hub
        /// </summary>
        /// <param name="data">The event data</param>
        /// <returns>The task representing the asynchronous operation</returns>
        Task SendAsync(EventData data);

        /// <summary>
        /// Asynchronously sends multiple event data objects to an Event Hub
        /// </summary>
        /// <param name="eventDataList">The list of event data</param>
        Task SendAsync(IEnumerable<EventData> eventDataList);

        /// <summary>
        /// Send a message string
        /// </summary>
        /// <param name="message">The message to send</param>
        Task SendAsync(string message);

        /// <summary>
        /// Send messages
        /// </summary>
        /// <param name="messages">The messages to send</param>
        Task SendAsync(IEnumerable<string> messages);

        /// <summary>
        /// Send an object as a message
        /// Note: This will be converted to JSON string
        /// </summary>
        /// <param name="data"></param>
        Task SendAsJsonAsync<T>(T data);

        /// <summary>
        /// Send objects as a message
        /// Note: This will be converted to JSON strings
        /// </summary>
        /// <param name="messages">The messages to send</param>
        Task SendAsJsonAsync<T>(IEnumerable<T> messages);
    }
}