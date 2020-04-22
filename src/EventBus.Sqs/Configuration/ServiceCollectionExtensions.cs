using System;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using EventBus.Sqs.AWSHelpers;
using EventBus.Sqs.Events;
using EventBus.Sqs.Extensions;
using EventBus.Sqs.HealthCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Sqs.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IHealthChecksBuilder AddSqsCheck<TEvent>(this IHealthChecksBuilder builder) where TEvent : IntegrationEvent
        {
            return builder.AddCheck<SqsHealthCheck>(typeof(TEvent).Name.ReplaceSufixEvent());
        }
        public static IEventBusBuilder AddEventBusSQS(this IServiceCollection services, IConfiguration configuration)
        {
            Validate(configuration);

            services.AddDefaultAWSOptions(new AWSOptions {
                Region = AWSGeneralHelper.GetRegionEndpoint(configuration["AWS:Region"])
            });
            services.AddScoped<IEventBus, EventBusSqs>();
            services.AddAWSService<IAmazonSQS>(ServiceLifetime.Scoped);

            return new EventBusBuilder(services);
        }

        private static void Validate(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["SQS_IS_FIFO"]))
                throw new ArgumentException("The parameter SQS_IS_FIFO is null or empty.");

            if(string.IsNullOrWhiteSpace(configuration["AWS:Region"]))
                throw new ArgumentException("The parameter AWS:Region is null or empty.");
        }
    }
}
