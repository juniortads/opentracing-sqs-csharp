using System;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Sqs
{
    public class EventBusBuilder : IEventBusBuilder
    {
        public IServiceCollection Services { get; }

        public EventBusBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentException("The parameter services is null or empty.");
        }
    }
}
