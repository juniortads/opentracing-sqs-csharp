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
    public class EventBusSqs : IEventBus
    {
        private readonly IAmazonSQS amazonSQS;
        private IConfiguration configuration;

        public EventBusSqs(IAmazonSQS amazonSQS, IConfiguration configuration)
        {
            this.amazonSQS = amazonSQS;
            this.configuration = configuration;
        }

        public async Task<DeleteMessageResponse> Dequeue(IntegrationEvent @event)
        {
            var deleteRequest = new DeleteMessageRequest
            {
                QueueUrl = @event.ReplaceIntegrationEventName().BuildQueueUrl(IsTypeFifo),
                ReceiptHandle = @event.ReceiptId
            };
            return await amazonSQS.DeleteMessageAsync(deleteRequest);
        }

        public async Task<SendMessageResponse> Enqueue(IntegrationEvent @event)
        {
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

        public TEvent ReceiveMessage<TEvent>(int waitTimeSeconds = 5) where TEvent : IntegrationEvent
        {
            var eventName = typeof(TEvent).Name.ReplaceSufixEvent();

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = eventName.BuildQueueUrl(IsTypeFifo),
                MaxNumberOfMessages = 1,
                AttributeNames = new List<string> { "All" },
                WaitTimeSeconds = waitTimeSeconds,
                VisibilityTimeout = 30,
                MessageAttributeNames = new List<string>() { "*" }
            };

            var result = amazonSQS.ReceiveMessageAsync(receiveMessageRequest)
                                 .GetAwaiter()
                                 .GetResult();

            if (result.HttpStatusCode == HttpStatusCode.OK)
            {
                var message = result.Messages.FirstOrDefault();

                if (message == null)
                    return null;

                var eventBody = JsonConvert.DeserializeObject<TEvent>(message.Body);
                eventBody.ReceiptId = message.ReceiptHandle;
                eventBody.MessageAttributes = message.MessageAttributes.BuildToMessageAttribute();

                return eventBody;
            }
            return null;
        }

        public void Dispose()
        {
            if (amazonSQS != null)
                amazonSQS.Dispose();
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
