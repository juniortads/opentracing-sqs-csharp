using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using Xunit;

namespace EventBus.Sqs.Tests.Unit.EventBusSqs
{
    public class EventBusSqsTests
    {
        [Fact(DisplayName = "When call Dequeue return from EventBusSqs return HttpStatusCode.OK")]
        public async Task WhenCallDequeueReturnFromEventBusSqsReturnStatusCodeOk()
        {
            var mockAmazonSQS = new Mock<IAmazonSQS>();

            mockAmazonSQS.Setup(m => m.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new DeleteMessageResponse { HttpStatusCode = HttpStatusCode.OK });

            var eventBus = new Sqs.EventBusSqs(mockAmazonSQS.Object, MockingHelpers.BuildConfiguration.Object);

            var result = await eventBus.Dequeue(MockingHelpers.BuildEvent);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        }

        [Fact(DisplayName = "When call Dequeue return from EventBusSqs return InternalServerError")]
        public async Task WhenCallDequeueReturnFromEventBusSqsReturnStatusCodeInternalServerError()
        {
            var mockAmazonSQS = new Mock<IAmazonSQS>();

            mockAmazonSQS.Setup(m => m.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new DeleteMessageResponse { HttpStatusCode = HttpStatusCode.InternalServerError });

            var eventBus = new Sqs.EventBusSqs(mockAmazonSQS.Object, MockingHelpers.BuildConfiguration.Object);

            var result = await eventBus.Dequeue(MockingHelpers.BuildEvent);

            Assert.Equal(HttpStatusCode.InternalServerError, result.HttpStatusCode);
        }

        [Fact(DisplayName = "When call Enqueue return from EventBusSqs return HttpStatusCode.OK")]
        public async Task WhenCallEnqueueReturnFromEventBusSqsReturnStatusCodeOk()
        {
            var mockAmazonSQS = new Mock<IAmazonSQS>();

            mockAmazonSQS.Setup(m => m.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new SendMessageResponse { HttpStatusCode = HttpStatusCode.OK });

            var eventBus = new Sqs.EventBusSqs(mockAmazonSQS.Object, MockingHelpers.BuildConfiguration.Object);

            var result = await eventBus.Enqueue(MockingHelpers.BuildEvent);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        }

        [Fact(DisplayName = "When call Enqueue return from EventBusSqs return InternalServerError")]
        public async Task WhenCallEnqueueReturnFromEventBusSqsReturnStatusCodeInternalServerError()
        {
            var mockAmazonSQS = new Mock<IAmazonSQS>();

            mockAmazonSQS.Setup(m => m.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new SendMessageResponse { HttpStatusCode = HttpStatusCode.InternalServerError });

            var eventBus = new Sqs.EventBusSqs(mockAmazonSQS.Object, MockingHelpers.BuildConfiguration.Object);

            var result = await eventBus.Enqueue(MockingHelpers.BuildEvent);

            Assert.Equal(HttpStatusCode.InternalServerError, result.HttpStatusCode);
        }
    }
}
