using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using EventBus.Sqs.Events;
using EventBus.Sqs.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EventBus.Sqs
{
    internal class EventBusSqs : IEventBus
    {
        private readonly ILogger<EventBusSqs> logger;
        private readonly IAmazonSQS amazonSQS;
        private IConfiguration configuration;

        public EventBusSqs(IAmazonSQS amazonSQS, ILogger<EventBusSqs> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.amazonSQS = amazonSQS;
            this.configuration = configuration;
        }

        public async Task<DeleteMessageResponse> Dequeue(IntegrationEvent @event)
        {
            logger.LogInformation($"Initialize Dequeue event: {@event}");

            var deleteRequest = new DeleteMessageRequest
            {
                QueueUrl = @event.ReplaceIntegrationEventName().BuildQueueUrl(IsTypeFifo),
                ReceiptHandle = @event.ReceiptId
            };
            return await amazonSQS.DeleteMessageAsync(deleteRequest);
        }

        public async Task<SendMessageResponse> Enqueue(IntegrationEvent @event)
        {
            logger.LogInformation($"Initialize Publish event: {@event}");

            var contractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };

            var jsonMessage = JsonConvert.SerializeObject(@event,

                new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                });
            
            var createRequest = new SendMessageRequest
            {
                MessageDeduplicationId = IsTypeFifo ? Guid.NewGuid().ToString() : null,
                MessageBody = jsonMessage,
                QueueUrl = @event.ReplaceIntegrationEventName().BuildQueueUrl(IsTypeFifo),
                MessageGroupId = IsTypeFifo ? @event.Id : null
            };

            @event.MessageAttributes.BuildToMessageAttribute(createRequest.MessageAttributes);

            return await amazonSQS.SendMessageAsync(createRequest);
        }

        public IEnumerable<TEvent> ReceiveMessage<TEvent>(int maxNumberOfMessages = 1, int waitTimeSeconds = 5) where TEvent : IntegrationEvent
        {
            var eventName = typeof(TEvent).Name.ReplaceSufixEvent();

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = eventName.BuildQueueUrl(IsTypeFifo),
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

        private bool IsTypeFifo
        {
            get
            {
                return configuration["SQS_IS_FIFO"].Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
