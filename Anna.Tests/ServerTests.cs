using System;
using System.Net;
using Anna.Tests.Util;
using NUnit.Framework;
using SharpTestsEx;

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
        public void CanHandleQueryArguments()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.GET("customers")
                    .Subscribe(ctx => ctx.Respond(string.Format("customers where name equals to {0}", ctx.Request.QueryString.Name)));

                Browser.ExecuteGet("http://localhost:1234/customers?name=jose")
                    .ReadAllContent().Should().Be.EqualTo("customers where name equals to jose");
            }
        }
    }
}