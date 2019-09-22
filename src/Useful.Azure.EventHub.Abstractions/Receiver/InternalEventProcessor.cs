using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace Useful.Azure.EventHub.Abstractions.Receiver
{
    public class InternalEventProcessor : IEventProcessor
    {
        private readonly Action<IEnumerable<EventData>> _processMessage;
        private readonly Action<string> _processEvent;
        private readonly Action<string> _processError;

        public InternalEventProcessor(Action<IEnumerable<EventData>> processMessage, Action<string> processEvent, Action<string> processError)
        {
            _processMessage = processMessage;
            _processEvent = processEvent;
            _processError = processError;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            _processEvent($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            _processEvent($"Processor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            _processError($"Processor Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            _processMessage(messages);

            return context.CheckpointAsync();
        }
    }
}