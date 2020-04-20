using System;
using System.Collections.Generic;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;

namespace EventBus.Sqs.Tracing.Extensions
{
    internal static class TracingExtensions
    {
        internal static IScope StartSpanConsumer(this ITracer tracer, IDictionary<string, string> messageAttributes, string operationName)
        {
            ISpanBuilder spanBuilder;
            try
            {
                ISpanContext spanContext = tracer.Extract(BuiltinFormats.TextMap, new TextMapExtractAdapter(messageAttributes));

                spanBuilder = tracer.BuildSpan(operationName);
                if (spanContext != null)
                {
                    spanBuilder = spanBuilder.AsChildOf(spanContext);
                }
            }
            catch (Exception)
            {
                spanBuilder = tracer.BuildSpan(operationName);
            }
            return spanBuilder.WithTag(Tags.SpanKind, Tags.SpanKindConsumer).StartActive(true);
        }

        internal static IScope StartSpanConsumer(this ITracer tracer, string operationName)
        {
            ISpanBuilder spanBuilder;

            try
            {
                var attributes = new Dictionary<string, string>();
                var spanContext = tracer.Extract(BuiltinFormats.TextMap, new TextMapExtractAdapter(attributes));

                spanBuilder = tracer.BuildSpan(operationName);
                if (spanContext != null)
                {
                    spanBuilder = spanBuilder.AsChildOf(spanContext);
                }
            }
            catch (Exception)
            {
                spanBuilder = tracer.BuildSpan(operationName);
            }
            return spanBuilder.WithTag(Tags.SpanKind, Tags.SpanKindConsumer).StartActive(true);
        }
    }
}
