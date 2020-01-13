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

## Usage

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