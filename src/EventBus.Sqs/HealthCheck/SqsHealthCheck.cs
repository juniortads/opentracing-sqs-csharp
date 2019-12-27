using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EventBus.Sqs.HealthCheck
{
    public class SqsHealthCheck : IHealthCheck
    {
        private readonly IAmazonSQS amazonSqs;

        public SqsHealthCheck(IAmazonSQS amazonSQS)
        {
            this.amazonSqs = amazonSQS;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await amazonSqs.GetQueueUrlAsync(context.Registration.Name, cancellationToken);

                return !string.IsNullOrEmpty(response.QueueUrl)
                    ? HealthCheckResult.Healthy()
                    : new HealthCheckResult(context.Registration.FailureStatus, description: $"Sqs ({context.Registration.Name}) check is not satisfied.");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
