using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventBus.Sqs.Tests.Unit.EventBusBuilder
{
    public class EventBusBuilderTests
    {
        [Fact(DisplayName ="Should Return From EventBusBuilder Return Ok")]
        public void ShouldReturnFromEventBusBuilderReturnOk()
        {
            var serviceCollection = new ServiceCollection();
            var eventBusBuilder = new Sqs.EventBusBuilder(serviceCollection);

            Assert.Equal(serviceCollection, eventBusBuilder.Services);
        }

        [Fact(DisplayName = "Should Return From EventBusBuilder Return ArgumentException")]
        public void ShouldReturnFromEventBusBuilderReturnException()
        {
            Assert.Throws<ArgumentException>(() => new Sqs.EventBusBuilder(null));
        }
    }
}
