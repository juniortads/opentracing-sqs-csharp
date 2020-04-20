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
        private ILogger<EventBusTracing> logger;

        public EventBusTracing(IEventBus eventBus, ITracer tracer)
        {
            this.eventBus = eventBus;
            this.tracer = tracer;

            CreateLogger();
        }

        private void CreateLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("EventBus.Sqs.Tracing", LogLevel.Debug)
                    .AddConsole();
            });
            logger = loggerFactory.CreateLogger<EventBusTracing>();
        }

        public async Task<DeleteMessageResponse> Dequeue(IntegrationEvent @event)
        {
            var operationName = "SQS::DeleteMessageAsync/";
            var eventName = $"{operationName}{@event.GetType().Name.ReplaceSufixEvent().ToLower()}";

            using (var scope = tracer.StartSpanConsumer(eventName))
            {
                logger.LogInformation($"Dequeue::{DateTime.Now}|{eventName} SpanId:{scope.Span.Context.SpanId} TraceId:{scope.Span.Context.TraceId}");

                return await eventBus.Dequeue(@event);
            }
        }

        public async Task<SendMessageResponse> Enqueue(IntegrationEvent @event)
        {
            var operationName = "SQS::SendMessageAsync/";
            var eventName = $"{operationName}{@event.GetType().Name.ReplaceSufixEvent().ToLower()}";

            using (var scope = tracer.BuildSpan(eventName).StartActive(true))
            {
                var span = scope.Span.SetTag(Tags.SpanKind, Tags.SpanKindProducer);

                var attributes = new Dictionary<string, string>();

                tracer.Inject(span.Context, BuiltinFormats.TextMap, new TextMapInjectAdapter(attributes));
                @event.MessageAttributes = attributes;

                logger.LogInformation($"Enqueue::{DateTime.UtcNow}|{eventName} SpanId:{scope.Span.Context.SpanId} TraceId:{scope.Span.Context.TraceId}");

                return await eventBus.Enqueue(@event);
            }
        }

        public TEvent ReceiveMessage<TEvent>(int waitTimeSeconds) where TEvent : IntegrationEvent
        {
            var message = eventBus.ReceiveMessage<TEvent>(waitTimeSeconds);

            if (message != null)
            {
                var operationName = "SQS::ReceiveMessageAsync/";
                var eventName = $"{operationName}{typeof(TEvent).Name.ReplaceSufixEvent().ToLower()}";

                using (var scope = tracer.StartSpanConsumer(message.MessageAttributes, eventName))
                {
                    tracer.ScopeManager.Activate(tracer.ActiveSpan, false);

                    logger.LogInformation($"ReceiveMessage::{DateTime.UtcNow} | {eventName} SpanId:{scope.Span.Context.SpanId} TraceId:{scope.Span.Context.TraceId}");

                    return message;
                }
            }
            return message;
        }

        public void Dispose()
        {
            IScope scope = tracer.ScopeManager.Active;
            if (scope != null)
                scope.Dispose();

            eventBus.Dispose();
        }
    }
}
