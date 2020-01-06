using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventBus.Sqs.Events
{
    public class IntegrationEvent
    {
        [JsonIgnore]
        internal const string INTEGRATION_EVENT_SUFIX = "Event";

        public IntegrationEvent(string id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }

        [JsonProperty]
        public string ReceiptId { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> MessageAttributes { get; set; }
    }
}
