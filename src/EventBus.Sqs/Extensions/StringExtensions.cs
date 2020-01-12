using System;
using EventBus.Sqs.Events;

namespace EventBus.Sqs.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceSufixEvent(this string name)
        {
            return name.Replace(IntegrationEvent.INTEGRATION_EVENT_SUFIX, string.Empty);
        }

        public static string BuildQueueUrl(this string eventName, bool isFifo)
        {
            return isFifo ? $"{eventName}.fifo" : $"{eventName}";
        }
    }
}
