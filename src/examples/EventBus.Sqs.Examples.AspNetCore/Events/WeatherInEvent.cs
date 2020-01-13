using System;
using EventBus.Sqs.Events;

namespace EventBus.Sqs.Examples.AspNetCore.Events
{
    public class WeatherInEvent : IntegrationEvent
    {
        public string WeatherEventId { get; set; }

        public WeatherInEvent(string id) :
            base(id, DateTime.UtcNow)
        {
            WeatherEventId = id;
        }
    }
}
