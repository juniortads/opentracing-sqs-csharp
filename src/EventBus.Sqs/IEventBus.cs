using System;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using EventBus.Sqs.Events;

namespace EventBus.Sqs
{
    public interface IEventBus : IDisposable
    {
        Task<SendMessageResponse> Enqueue(IntegrationEvent @event);

        Task<DeleteMessageResponse> Dequeue(IntegrationEvent @event);

        TEvent ReceiveMessage<TEvent>(int waitTimeSeconds) where TEvent : IntegrationEvent;
    }
}
