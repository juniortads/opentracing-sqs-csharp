using System;
using EventBus.Sqs.Events;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EventBus.Sqs.Tests.Unit
{
    public class MockingHelpers
    {
        public static Mock<IConfiguration> BuildConfiguration
        {
            get
            {
                var mockConfig = new Mock<IConfiguration>();
                mockConfig.SetupGet(o => o[It.Is<string>(s => s == "SQS_IS_FIFO")]).Returns("false");

                return mockConfig;
            }
        }

        public static IntegrationEvent BuildEvent
        {
            get
            {
                return new MyEvent();
            }
        }
    }

    public class MyEvent : IntegrationEvent
    {
        public MyEvent() : base(Guid.NewGuid().ToString(), DateTime.UtcNow)
        {

        }
    }
}
