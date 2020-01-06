using System;
using EventBus.Sqs.Events;

namespace EventBus.Sqs.Extensions
{
    internal static class StringExtensions
    {
        internal static string ReplaceSufixEvent(this string name)
        {
            return name.Replace(IntegrationEvent.INTEGRATION_EVENT_SUFIX, string.Empty);
        }

        internal static string BuildQueueUrl(this string eventName, bool isFifo)
        {
            return isFifo ? $"{eventName}.fifo" : $"{eventName}";
        }
    }
}
