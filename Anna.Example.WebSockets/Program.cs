using System;
using Anna.Responses;

namespace Anna.Example.WebSockets
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var server = new HttpServer("http://*:555/"))
            {
                server.GET("test")
                        .Subscribe(ctx => ctx.Respond(new StaticFileResponse(@"views\index.html")));

                Console.ReadLine();
            }
        }
    }
}
