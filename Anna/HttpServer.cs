using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Anna.Observers;
using Anna.Request;
using Anna.Util;

namespace Anna
{
    public class HttpServer : IDisposable
    {
        private readonly HttpListener listener;
        private readonly IObservable<RequestContext> stream;

        //URI - METHOD
        private readonly List<Tuple<string, string>> handledRoutes 
            = new List<Tuple<string, string>>();
        private readonly IScheduler scheduler;

        public HttpServer(string url, IScheduler scheduler = null)
        {
            this.scheduler = scheduler ?? new EventLoopScheduler();
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            stream = ObservableHttpContext();
        }

        private IObservable<RequestContext> ObservableHttpContext()
        {
            var observableHttpContext = Observable.Create<RequestContext>(obs =>
                Observable.FromAsyncPattern<HttpListenerContext>(listener.BeginGetContext, listener.EndGetContext)()
                          .Select(c => new RequestContext(c.Request, c.Response))
                          .Subscribe(obs))
                .Repeat().Retry()
                .Publish().RefCount().ObserveOn(scheduler);

            observableHttpContext.Subscribe(new UnhandledRouteObserver(handledRoutes));
            return observableHttpContext;
        }


        public void Dispose()
        {
            listener.Stop();
        }

        public IObservable<RequestContext> GET(string uri)
        {
            return OnUriAndMethod(uri, "GET");
        }

        public IObservable<RequestContext> POST(string uri)
        {
            return OnUriAndMethod(uri, "POST");
        }

        public IObservable<RequestContext> PUT(string uri)
        {
            return OnUriAndMethod(uri, "PUT");
        }

        public IObservable<RequestContext> HEAD(string uri)
        {
            return OnUriAndMethod(uri, "HEAD");
        }

        public IObservable<RequestContext> OPTIONS(string uri)
        {
            return OnUriAndMethod(uri, "OPTIONS");
        }

        public IObservable<RequestContext> DELETE(string uri)
        {
            return OnUriAndMethod(uri, "DELETE");
        }

        public IObservable<RequestContext> TRACE(string uri)
        {
            return OnUriAndMethod(uri, "TRACE");
        }

        public IObservable<RequestContext> RAW(string uri)
        {
            return OnUriAndMethod(uri, null);
        }

        public IObservable<RequestContext> OnUriAndMethod(string uri, string method)
        {
            handledRoutes.Add(new Tuple<string, string>(uri, method));

            var uriTemplate = new UriTemplate(uri);
            return Observable.Create<RequestContext>(obs => 
                stream.Subscribe(ctx => OnUriAndMethodHandler(ctx, method, uriTemplate, obs), 
                                 obs.OnError, obs.OnCompleted));
        }

        private static void OnUriAndMethodHandler(
                RequestContext ctx, 
                string method, 
                UriTemplate uriTemplate, 
                IObserver<RequestContext> obs)
        {
            if (!string.IsNullOrEmpty(method) && ctx.Request.HttpMethod != method) return;

            var serverPath = ctx.Request.Url.GetServerBaseUri();
            var uriTemplateMatch = uriTemplate.Match(new Uri(serverPath), ctx.Request.Url);
            if (uriTemplateMatch == null) return;

            ctx.Request.LoadArguments(uriTemplateMatch.BoundVariables);
            obs.OnNext(ctx);
        }
    }
}