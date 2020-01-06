using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using EventBus.Sqs.Events;

namespace EventBus.Sqs
{
    public interface IEventBus
    {
        Task<SendMessageResponse> Enqueue(IntegrationEvent @event);

        Task<DeleteMessageResponse> Dequeue(IntegrationEvent @event);

        IEnumerable<TEvent> ReceiveMessage<TEvent>(int maxNumberOfMessages, int waitTimeSeconds) where TEvent : IntegrationEvent;
    }
}
