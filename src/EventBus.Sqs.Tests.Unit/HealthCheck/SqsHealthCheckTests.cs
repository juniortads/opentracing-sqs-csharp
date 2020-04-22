using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using EventBus.Sqs.Configuration;
using EventBus.Sqs.Events;
using EventBus.Sqs.HealthCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EventBus.Sqs.Tests.Unit.HealthCheck
{
    public class SqsHealthCheckTests
    {
        [Fact(DisplayName = "Should Add HealthCheck")]
        public void ShouldAddHealthCheck()
        {
            var amazonMock = new Mock<IAmazonSQS>();

            var services = new ServiceCollection();

            services
                .AddScoped(x => amazonMock.Object)
                .AddHealthChecks()
                .AddSqsCheck<MyEvent>();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                   {
                        new KeyValuePair<string, string>("SQS_IS_FIFO", "false")
                   })
               .Build();

            services
                .AddScoped<IConfiguration>(x => config)
                .AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            Assert.IsType<SqsHealthCheck>(check);
            Assert.Equal("My", registration.Name);
        }

        [Fact(DisplayName = "Should Return Healthy")]
        public async Task ShouldReturnHealthy()
        {
            var amazonMock = new Mock<IAmazonSQS>();

            amazonMock
                .Setup(x => x.GetQueueUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetQueueUrlResponse() { QueueUrl = $"http://localstack:4576/event/My.fifo" });

            var sqsHealthCheck = new SqsHealthCheck(amazonMock.Object, MockingHelpers.BuildConfiguration.Object);

            var healthCheckContext = new HealthCheckContext()
            {
                Registration = new HealthCheckRegistration("My", sqsHealthCheck, default, default)
            };

            var healthCheckResult = await sqsHealthCheck.CheckHealthAsync(healthCheckContext);

            Assert.Equal(HealthStatus.Healthy, healthCheckResult.Status);
        }

        [Theory(DisplayName = "Should Return Unhealthy When Queue NotFound")]
        [InlineData("")]
        [InlineData(null)]
        public async Task ShouldReturnUnhealthyWhenQueueNotFound(string queueUrl)
        {
            var amazonMock = new Mock<IAmazonSQS>();

            amazonMock
                .Setup(x => x.GetQueueUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetQueueUrlResponse() { QueueUrl = queueUrl });

            var mockLog = new Mock<ILogger<SqsHealthCheck>>();
            var sqsHealthCheck = new SqsHealthCheck(amazonMock.Object, MockingHelpers.BuildConfiguration.Object);

            var healthCheckContext = new HealthCheckContext()
            {
                Registration = new HealthCheckRegistration("My", sqsHealthCheck, default, default)
            };

            var healthCheckResult = await sqsHealthCheck.CheckHealthAsync(healthCheckContext);

            Assert.Equal(HealthStatus.Unhealthy, healthCheckResult.Status);
        }

        [Fact(DisplayName = "Should Return Unhealthy If Has Any Error")]
        public async Task ShouldReturnUnhealthyIfHasAnyError()
        {
            var amazonMock = new Mock<IAmazonSQS>();
            

            var sqsHealthCheck = new SqsHealthCheck(amazonMock.Object, MockingHelpers.BuildConfiguration.Object);

            var healthCheckContext = new HealthCheckContext()
            {
                Registration = new HealthCheckRegistration("My", sqsHealthCheck, default, default)
            };

            var healthCheckResult = await sqsHealthCheck.CheckHealthAsync(healthCheckContext);

            Assert.Equal(HealthStatus.Unhealthy, healthCheckResult.Status);
        }
    }
}
