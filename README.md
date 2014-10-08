# Anna [![Build status](https://ci.appveyor.com/api/projects/status/mw9xg6c7jamxsq3c?svg=true&branch=master)](https://ci.appveyor.com/project/jfromaniello/anna) [![NuGet](http://img.shields.io/nuget/v/Anna.svg?style=flat-square)](https://www.nuget.org/packages/Anna/) [![NuGet downloads](http://img.shields.io/nuget/dt/Anna.svg?style=flat-square)](https://www.nuget.org/packages/Anna/)

Anna is an event-driven HTTP server library built with ReactiveExtensions (Rx). Anna was inspired in [Node.js](http://nodejs.org) and [Nancy](https://github.com/NancyFx/Nancy).

Anna exposes observable sequences of HTTP requests to which you can subscribe to handle the request.

```c#
using (var server = new HttpServer("http://*:1234/"))
{
    // simple basic usage, all subscriptions will run in a single event-loop
    server.GET("/hello/{Name}")
          .Subscribe(ctx => ctx.Respond("Hello, " + ctx.Request.UriArguments.Name + "!"));

    // use Rx LINQ operators
    server.POST("/hi/{Name}")
          .Where(ctx => ctx.Request.UriArguments.Name == "George")
          .Subscribe(ctx => ctx.Respond("Hi, George!"));

    server.POST("/hi/{Name}")
          .Where(ctx => ctx.Request.UriArguments.Name == "Pete")
          .Subscribe(ctx => ctx.Respond("Hi, Pete!"));
}
```

Another example is in my blog: [Long polling chat with Anna](http://joseoncode.com/2011/07/22/long-polling-chat-with-anna/)

## License

The MIT License (MIT)

Copyright (c) 2011, 2012, 2013, 2014 José F. Romaniello

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
