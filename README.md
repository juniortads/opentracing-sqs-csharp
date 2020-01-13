[![Build status](https://ci.appveyor.com/api/projects/status/k7o1c0k2tbu4nhx6/branch/master?svg=true)](https://ci.appveyor.com/project/juniortads/opentracing-sqs-csharp/branch/master) [![NuGet](https://buildstats.info/nuget/EventBus.Sqs.Tracing)](http://www.nuget.org/packages/EventBus.Sqs.Tracing)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

# opentracing-sqs-csharp

A simple C# library for instrumentation of [SQS](https://aws.amazon.com/sqs) messages 
with [OpenTracing](http://opentracing.io/).

## Dependencies

* NET Core 3.1 
* AWSSDK.SQS 3.3.102.54
* OpenTracing

## Installation

Install the [NuGet package](https://www.nuget.org/packages/EventBus.Sqs.Tracing/):

    > dotnet add package EventBus.Sqs

    > dotnet add package EventBus.Sqs.Tracing

## Getting started

**Before**, in your application's _Startup.cs_ file, configure OpenTracing first.
```cs
using OpenTracing.Util;
...
services.AddSingleton(serviceProvider =>
{
    var loggerFactory = new LoggerFactory();

    var config = Jaeger.Configuration.FromEnv(loggerFactory);
    var tracer = config.GetTracer();

    if (!GlobalTracer.IsRegistered())
        GlobalTracer.Register(tracer);

    return tracer;
});
```
#### Check this link for more information about Jaeger configuration via environment.
https://github.com/jaegertracing/jaeger-client-csharp

**Next**, configure EventBus.Sqs with tracing: 
```cs
using EventBus.Sqs.Configuration;
using EventBus.Sqs.Tracing.Configuration;
...
services.AddEventBusSQS(Configuration)
        .AddOpenTracing();
```
### Configure health checks for SQS
```cs
services.AddHealthChecks()
        .AddSqsCheck<HereAddYourIntegrationEvent>();
```
**Finally**, We can using in your application's _Controller.cs_ file, Check below this example:
```cs
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IEventBus eventBus;
    public WeatherForecastController(IEventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    [HttpPost]
    public async Task<ActionResult> Create()
    {
        var rng = new Random();
        //Create sample Integration Event
        var weatherInEvent = new WeatherInEvent(Guid.NewGuid().ToString(), "Freezing");

        await eventBus.Enqueue(weatherInEvent);

        return Accepted();
    }
}
```
**Don't forget**, Creating your Integration event:
```cs
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
```
### More examples:
[Link](../examples)

![Tracing](../images/tracing-sqs.png)

## Contact

These [email addresses](MAINTAINERS) serve as the main contact addresses for this project.

Bug reports and feature requests are more likely to be addressed
if posted as [issues](../../issues) here on GitHub.

## License

This project is licensed under the terms of the MIT license, see also the full [LICENSE](LICENSE).

MIT License

Copyright (c) 2019 Joel Junior

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.