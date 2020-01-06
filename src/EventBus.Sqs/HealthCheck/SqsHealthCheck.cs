using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EventBus.Sqs.HealthCheck
{
    public class SqsHealthCheck : IHealthCheck
    {
        private readonly IAmazonSQS amazonSQS;
        private readonly IConfiguration configuration;

        public SqsHealthCheck(IAmazonSQS amazonSQS, IConfiguration configuration)
        {
            this.amazonSQS = amazonSQS;
            this.configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var registrationName = IsTypeFifo ? $"{context.Registration.Name}.fifo" : $"{context.Registration.Name}";

                var response = await amazonSQS.GetQueueUrlAsync(registrationName, cancellationToken);

                return !string.IsNullOrEmpty(response.QueueUrl)
                    ? HealthCheckResult.Healthy()
                    : new HealthCheckResult(context.Registration.FailureStatus, description: $"Sqs ({context.Registration.Name}) check is not satisfied.");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }

        private bool IsTypeFifo
        {
            get
            {
                return configuration["SQS_IS_FIFO"].Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
