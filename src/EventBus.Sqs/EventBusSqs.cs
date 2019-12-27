using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using EventBus.Sqs.Events;
using EventBus.Sqs.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventBus.Sqs
{
    internal class EventBusSqs : IEventBus
    {
        private readonly ILogger<EventBusSqs> logger;
        private readonly IAmazonSQS amazonSQS;

        public EventBusSqs(IAmazonSQS amazonSQS, ILogger<EventBusSqs> logger)
        {
            this.logger = logger;
            this.amazonSQS = amazonSQS;
        }

        public async Task<DeleteMessageResponse> Dequeue(IntegrationEvent @event)
        {
            logger.LogInformation($"Initialize Dequeue event: {@event}");

            var deleteRequest = new DeleteMessageRequest
            {
                //QueueUrl = CreateQueueUrl(@event.ReplaceIntegrationEventName()),
                ReceiptHandle = @event.ReceiptId
            };
            return await amazonSQS.DeleteMessageAsync(deleteRequest);
        }

        public async Task<SendMessageResponse> Enqueue(IntegrationEvent @event)
        {
            logger.LogInformation($"Initialize Publish event: {@event}");

            var jsonMessage = JsonConvert.SerializeObject(@event);

            var createRequest = new SendMessageRequest
            {
                //MessageDeduplicationId = _configEventBus.IsTypeFifo ? Guid.NewGuid().ToString() : null,
                MessageBody = jsonMessage,
                //QueueUrl = CreateQueueUrl(@event.ReplaceIntegrationEventName()),
                //MessageGroupId = _configEventBus.IsTypeFifo ? @event.Id : null
            };

            @event.MessageAttributes.BuildToMessageAttribute(createRequest.MessageAttributes);

            return await amazonSQS.SendMessageAsync(createRequest);
        }

        public IEnumerable<TEvent> ReceiveMessage<TEvent>(int maxNumberOfMessages = 1, int waitTimeSeconds = 5) where TEvent : IntegrationEvent
        {
            var eventName = typeof(TEvent).Name.ReplaceSufixEvent();

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                //QueueUrl = CreateQueueUrl(eventName),
                MaxNumberOfMessages = maxNumberOfMessages,
                AttributeNames = new List<string> { "All" },
                WaitTimeSeconds = waitTimeSeconds,
                VisibilityTimeout = 30,
                MessageAttributeNames = new List<string>() { "*" }
            };

            var result = amazonSQS.ReceiveMessageAsync(receiveMessageRequest)
                                 .GetAwaiter()
                                 .GetResult();

            var messageToReturn = new List<TEvent>();

            if (result.HttpStatusCode == HttpStatusCode.OK)
            {
                if (result.Messages.Any())
                {
                    foreach (var item in result.Messages)
                    {
                        var eventBody = JsonConvert.DeserializeObject<TEvent>(item.Body);
                        eventBody.ReceiptId = item.ReceiptHandle;
                        eventBody.MessageAttributes = item.MessageAttributes.BuildToMessageAttribute();

                        messageToReturn.Add(eventBody);
                    }
                }
            }
            return messageToReturn;
        }
    }
}
