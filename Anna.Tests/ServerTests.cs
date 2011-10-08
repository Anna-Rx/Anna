using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anna.Responses;
using Anna.Tests.Util;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using Enumerable = System.Linq.Enumerable;

namespace Anna.Tests
{
    public class ServerTests
    {
        [Test]
        public void CanReturnAnString()
        {
            using(var server = new HttpServer("http://*:1234/"))
            {
                server.GET("/")
                      .Subscribe(ctx => ctx.Respond("hello world"));
                
                Browser.ExecuteGet("http://localhost:1234")
                    .ReadAllContent()
                    .Should().Be.EqualTo("hello world");    
            }
        }

        [Test]
        public void ExampleCode()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                // simple basic usage, all subscriptions will run in a single event-loop
                server.GET("/hello/{Name}")
                      .Subscribe(ctx => ctx.Respond("Hello, " + ctx.Request.UriArguments.Name + "!"));

                Browser.ExecuteGet("http://localhost:1234/hello/George")
                    .ReadAllContent()
                    .Should().Be.EqualTo("Hello, George!");

                // use Rx LINQ operators
                server.POST("/hi/{Name}")
                      .Where(ctx => ctx.Request.UriArguments.Name == "George")
                      .Subscribe(ctx => ctx.Respond("Hi, George!"));

                server.POST("/hi/{Name}")
                      .Where(ctx => ctx.Request.UriArguments.Name == "Pete")
                      .Subscribe(ctx => ctx.Respond("Hi, Pete!"));

                Browser.ExecutePost("http://localhost:1234/hi/George")
                     .ReadAllContent()
                     .Should().Be.EqualTo("Hi, George!");

                Browser.ExecutePost("http://localhost:1234/hi/Pete")
                     .ReadAllContent()
                     .Should().Be.EqualTo("Hi, Pete!");

                // This becomes a problem:
                //Browser.ExecutePost("http://localhost:1234/hi/Fran").StatusCode.Should().Be.EqualTo(404);
            }
        }

        [Test]
        public void CanReturnBinaryData()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                var expectedResponse = new byte[] { 0, 1, 2, 3, 4};
                server.GET("/")
                      .Subscribe(ctx => ctx.Response(expectedResponse).Send());

                var response = Browser.ExecuteGet("http://localhost:1234");
                byte[] data = new BinaryReader(response.GetResponseStream()).ReadBytes(1337);

                data.Length.Should().Be.EqualTo(expectedResponse.Length);
                for (int i = 0; i != data.Length; i++) data[i].Should().Be.EqualTo(expectedResponse[i]);
            }
        }

        [Test]
        public void CanDecodeARequestBody()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                string requestBody = "There's no business like \u0160ovs \u4F01\u696D";
                server.POST("/")
                      .Subscribe(ctx =>
                      {
                            ctx.Request.GetBody().Subscribe(body => 
                            { 
                                try
                                {
                                    Console.WriteLine(body);
                                    body.Should().Be.EqualTo(requestBody);
                                } 
                                finally 
                                {
                                   ctx.Respond("hi");
                                }
                            });
                      });

                //Browser.ExecutePost("http://posttestserver.com/post.php", requestBody)
                Browser.ExecutePost("http://localhost:1234", requestBody)
                    .ReadAllContent()
                    .Should().Contain("hi");
            }
        }
        
        [Test]
        public void CanReturnAStaticFile()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.GET("/")
                      .Subscribe(ctx => ctx.StaticFileResponse(@"samples\example_1.txt").Send());

                Browser.ExecuteGet("http://localhost:1234")
                    .ReadAllContent()
                    .Should().Contain(string.Join(Environment.NewLine, Enumerable.Range(1, 9)));
            }
        }



        [Test]
        public void CanReturnAnStatusCode()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.POST("/")
                    .Subscribe(ctx => ctx.Respond(201));

                Browser.ExecutePost("http://localhost:1234")
                    .StatusCode
                    .Should().Be.EqualTo(HttpStatusCode.Created);
            }
        }

        [Test]
        public void CanHandleUriArguments()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.GET("customer/{name}")
                      .Subscribe(ctx => ctx.Respond(string.Format("hello {0}", ctx.Request.UriArguments.name)));

                Browser.ExecuteGet("http://localhost:1234/customer/peter")
                    .ReadAllContent()
                    .Should().Be.EqualTo("hello peter");
            }
        }

        [Test]
        public void CanHandleQueryStringArguments()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.GET("customers")
                    .Subscribe(ctx => ctx.Respond(string.Format("customers where name equals to {0}", ctx.Request.QueryString.Name)));

                Browser.ExecuteGet("http://localhost:1234/customers?name=jose")
                    .ReadAllContent().Should().Be.EqualTo("customers where name equals to jose");
            }
        }

        [Test]
        public void WhenRequestingAnUnhandledRoute_ThenReturn404()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.GET("customer/{name}")
                      .Subscribe(ctx => ctx.Respond(string.Format("hello {0}", ctx.Request.UriArguments.name)));

                Executing.This(() => Browser.ExecuteGet("http://localhost:1234/customersssss/peter"))
                    .Should().Throw<WebException>()
                    .And.Exception.Response.OfType<HttpWebResponse>().StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
            }
        }

        [Test]
        public void WhenSubscribingToRaw_ThenIgnoreTheMethod()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.RAW("customer/{name}")
                      .Subscribe(ctx => ctx.Respond(string.Format("hello {0}", ctx.Request.UriArguments.name)));

                Browser.ExecuteGet("http://localhost:1234/customer/peter")
                    .ReadAllContent().Should().Be.EqualTo("hello peter");
                
                Browser.ExecutePost("http://localhost:1234/customer/peter")
                    .ReadAllContent().Should().Be.EqualTo("hello peter");
            }
        }

        [Test]
        public void CanSubscribeToRawInARawPath()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.RAW("website/*")
                      .Subscribe(ctx => ctx.Respond("hello master of puppets!"));

                Browser.ExecuteGet("http://localhost:1234/website/a/b/c/peter?thisQueryString=asdasdsa")
                    .ReadAllContent().Should().Be.EqualTo("hello master of puppets!");

                Browser.ExecutePost("http://localhost:1234/website/abcdeefasdasds?a=cdef")
                    .ReadAllContent().Should().Be.EqualTo("hello master of puppets!");
            }
        }


        [Test]
        public void UseSameThreadForAllRequests()
        {
            var bag = new ConcurrentBag<int>();

            using (var server = new HttpServer("http://*:1234/"))
            {
                server.RAW("")
                      .Subscribe(ctx =>
                                     {
                                         bag.Add(Thread.CurrentThread.ManagedThreadId);
                                         ctx.Respond(200);
                                     });
                Parallel.For(1, 1000, i => Browser.ExecuteGet("http://localhost:1234/"));

                bag.Distinct().Count()
                   .Should("The default scheduler should be Event Loop, and all subscriber run in same thread")
                   .Be.EqualTo(1);

            }
        }


        [Test]
        public void WhenWritingToTheStreamFail_ThenTryToRespond500()
        {
            using(var server = new HttpServer("http://*:1234/", Scheduler.CurrentThread))
            {
                server.RAW("")
                    .Subscribe(ctx =>
                                   {
                                       var mockedResponse = Mock.Of<Response>( r => r.WriteStream(It.IsAny<Stream>()) ==
                                                                Observable.Throw<Stream>(new InvalidOperationException()));

                                       mockedResponse.Send();
                                       //ctx.Respond(mockedResponse);
                                   });

                Executing.This(() => Browser.ExecuteGet("http://localhost:1234/"))
                    .Should().Throw<WebException>();
            }
        }
    }
}