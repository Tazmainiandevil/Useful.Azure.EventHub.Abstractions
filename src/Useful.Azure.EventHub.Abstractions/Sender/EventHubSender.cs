using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace Useful.Azure.EventHub.Abstractions.Sender
{
    public class EventHubSender : IEventHubSender
    {
        private readonly EventHubClient _eventHubClient;

        public EventHubSender(EventHubsConnectionStringBuilder connectionString)
        {
            _eventHubClient = EventHubClient.Create(connectionString);
        }

        /// <summary>
        /// Asynchronously sends event data to an Event Hub
        /// </summary>
        /// <param name="data">The event data</param>
        /// <returns>The task representing the asynchronous operation</returns>
        public Task SendAsync(EventData data)
        {
            return _eventHubClient.SendAsync(data);
        }

        /// <summary>
        /// Asynchronously sends multiple event data list to an Event Hub
        /// </summary>
        /// <param name="eventDataList">The list of event data</param>
        public Task SendAsync(IEnumerable<EventData> eventDataList)
        {
            return _eventHubClient.SendAsync(eventDataList);
        }

        /// <summary>
        /// Send a message string
        /// </summary>
        /// <param name="message">The message to send</param>
        public Task SendAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
        }

        /// <summary>
        /// Send messages
        /// </summary>
        /// <param name="messages">The messages to send</param>
        public Task SendAsync(IEnumerable<string> messages)
        {
            var eventDataMessages = messages.Select(message => new EventData(Encoding.UTF8.GetBytes(message)));

            return _eventHubClient.SendAsync(eventDataMessages);
        }

        /// <summary>
        /// Send an object as a message
        /// Note: This will be converted to JSON string
        /// </summary>
        /// <param name="data"></param>
        public Task SendAsJsonAsync<T>(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var jsonData = JsonConvert.SerializeObject(data);

            return _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(jsonData)));
        }

        /// <summary>
        /// Send objects as a message
        /// Note: This will be converted to JSON strings
        /// </summary>
        /// <param name="messages">The messages to send</param>
        public Task SendAsJsonAsync<T>(IEnumerable<T> messages)
        {
            var eventDataMessages = messages.Select(message => new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))));

            return _eventHubClient.SendAsync(eventDataMessages);
        }
    }
}