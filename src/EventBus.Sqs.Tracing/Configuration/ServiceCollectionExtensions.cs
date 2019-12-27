using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;

namespace EventBus.Sqs.Tracing.Configuration
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventBusBuilder"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static IEventBusBuilder AddOpenTracing(this IEventBusBuilder eventBusBuilder, ILogger logger)
        {
            var services = eventBusBuilder.Services;

            var eventBusDescriptor = services.First(s => s.ServiceType == typeof(IEventBus));

            services.Replace(ServiceDescriptor.Singleton<IEventBus>(locator =>
            {
                var eventBus = (IEventBus)(eventBusDescriptor?.ImplementationInstance ??
                                ActivatorUtilities.GetServiceOrCreateInstance(locator, eventBusDescriptor.ImplementationType));

                return new EventBusTracing(eventBus, GlobalTracer.Instance, logger);
            }));

            return eventBusBuilder;
        }
    }
}
