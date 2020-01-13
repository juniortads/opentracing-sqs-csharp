﻿using System;
using EventBus.Sqs.Events;

namespace EventBus.Sqs.Examples.AspNetCore.Events
{
    public class WeatherInEvent : IntegrationEvent
    {
        public string WeatherEventId { get; set; }
        public string Summary { get; set; }

        public WeatherInEvent(string id, string summary) : base(id, DateTime.UtcNow)
        {
            WeatherEventId = id;
            Summary = summary;
        }
    }
}
