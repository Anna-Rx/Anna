Anna
====

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