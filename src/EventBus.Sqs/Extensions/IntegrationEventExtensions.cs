using System;
using EventBus.Sqs.Events;

namespace EventBus.Sqs.Extensions
{
    internal static class IntegrationEventExtensions
    {
        internal static string ReplaceIntegrationEventName(this IntegrationEvent integrationEvent)
        {
            return integrationEvent.GetType().Name.Replace(IntegrationEvent.INTEGRATION_EVENT_SUFIX, "");
        }
    }
}
