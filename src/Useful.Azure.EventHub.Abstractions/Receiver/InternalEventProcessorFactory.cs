using System;
using System.Collections.Generic;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace Useful.Azure.EventHub.Abstractions.Receiver
{
    internal class InternalEventProcessorFactory : IEventProcessorFactory
    {
        private readonly Action<IEnumerable<EventData>> _processMessage;
        private readonly Action<string> _processEvent;
        private readonly Action<string> _processError;

        public InternalEventProcessorFactory(Action<IEnumerable<EventData>> processMessage, Action<string> processEvent, Action<string> processError)
        {
            _processMessage = processMessage;
            _processEvent = processEvent;
            _processError = processError;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            var processor = new InternalEventProcessor(_processMessage, _processEvent, _processError);

            return processor;
        }
    }
}