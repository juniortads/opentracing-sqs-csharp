using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using EventBus.Sqs.Events;
using EventBus.Sqs.Extensions;
using EventBus.Sqs.Tracing.Extensions;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;

namespace EventBus.Sqs.Tracing
{
    internal class EventBusTracing : IEventBus
    {
        private readonly IEventBus eventBus;
        private readonly ITracer tracer;
        private readonly ILogger<EventBusTracing> logger;

        public EventBusTracing(IEventBus eventBus, ITracer tracer, ILogger<EventBusTracing> logger)
        {
            this.eventBus = eventBus;
            this.tracer = tracer;
            this.logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task<DeleteMessageResponse> Dequeue(IntegrationEvent @event)
        {
            using (var scope = tracer.StartSpanConsumer(@event.MessageAttributes, $"sqs-dequeue-event-{@event.GetType().Name.ReplaceSufixEvent().ToLower()}"))
            {
                return await eventBus.Dequeue(@event);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task<SendMessageResponse> Enqueue(IntegrationEvent @event)
        {
            try
            {
                using (var scope = tracer.BuildSpan($"sqs-enqueue-event-{@event.GetType().Name.ReplaceSufixEvent().ToLower()}").StartActive(finishSpanOnDispose: true))
                {
                    var span = scope.Span.SetTag(Tags.SpanKind, Tags.SpanKindProducer);

                    var attributes = new Dictionary<string, string>();
                    tracer.Inject(span.Context, BuiltinFormats.TextMap, new TextMapInjectAdapter(attributes));

                    @event.MessageAttributes = attributes;

                    return await eventBus.Enqueue(@event);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="maxNumberOfMessages"></param>
        /// <returns></returns>
        public IEnumerable<TEvent> ReceiveMessage<TEvent>(int maxNumberOfMessages, int waitTimeSeconds) where TEvent : IntegrationEvent
        {
            return eventBus.ReceiveMessage<TEvent>(maxNumberOfMessages, waitTimeSeconds);
        }
    }
}
