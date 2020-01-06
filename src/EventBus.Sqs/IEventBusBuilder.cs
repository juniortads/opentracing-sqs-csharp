using System;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Sqs
{
    public interface IEventBusBuilder
    {
        IServiceCollection Services { get; }
    }
}
