using System;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using EventBus.Sqs.Events;
using EventBus.Sqs.Extensions;
using EventBus.Sqs.HealthCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Sqs.Configuration
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///      
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHealthChecksBuilder AddSqsCheck<T>(this IHealthChecksBuilder builder)
        {
            return builder.AddCheck<SqsHealthCheck>(typeof(T).Name.ReplaceSufixEvent());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEventBusBuilder AddEventBusSQS(this IServiceCollection services, IConfiguration configuration)
        {
            Validate(configuration);

            services.AddSingleton<IEventBus, EventBusSqs>();
            services.AddAWSService<IAmazonSQS>(ServiceLifetime.Singleton);

            return new EventBusBuilder(services);
        }

        private static void Validate(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["SQS_IS_FIFO"]))
                throw new ArgumentException("The parameter SQS_IS_FIFO is null or empty.");
        }
    }
}
