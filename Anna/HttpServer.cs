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
    /// <summary>
    /// The main HTTP server class. Implements IDisposable, so recommended usage is creating it in a using() statement. 
    /// </summary>
    public class HttpServer : IDisposable
    {
        private readonly HttpListener listener;
        private readonly IObservable<RequestContext> stream;
        private IDisposable subscription;

        //URI - METHOD
        private readonly List<Tuple<string, string>> handledRoutes 
            = new List<Tuple<string, string>>();
        private readonly IScheduler scheduler;

        /// <summary>
        /// Creates a new HTTP Server that listens on a particular port and prefix. Stops listening when Dispose() is called, so recommended usage is inside a using() block.
        /// </summary>
        /// <param name="url">Prefix URL for web service. Must include a trailing slash. Use e.g. "http://*:5555/" to listen on port 5555 for any given hostname.</param>
        /// <param name="scheduler">Any valid Rx scheduler</param>
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

            subscription = observableHttpContext.Subscribe(new UnhandledRouteObserver(handledRoutes));
            return observableHttpContext;
        }


        public void Dispose()
        {
            listener.Stop();
            subscription.Dispose();
        }

        /// <summary>
        /// Returns an observable sequence of GET requests that conform to the given URI template, relative to the server's prefix.
        /// </summary>
        /// <param name="uri">A URI template string. See <see cref="System.UriTemplate"/> for the template format.</param>
        /// <returns>An <c>IObservable</c> to which you should <c>Subscribe()</c></returns>
        public IObservable<RequestContext> GET(string uri)
        {
            return OnUriAndMethod(uri, "GET");
        }

        /// <summary>
        /// Returns an observable sequence of POST requests that conform to the given URI template, relative to the server's prefix.
        /// </summary>
        /// <param name="uri">A URI template string. See <see cref="System.UriTemplate"/> for the template format.</param>
        /// <returns>An <c>IObservable</c> to which you should <c>Subscribe()</c></returns>
        public IObservable<RequestContext> POST(string uri)
        {
            return OnUriAndMethod(uri, "POST");
        }

        /// <summary>
        /// Returns an observable sequence of PUT requests that conform to the given URI template, relative to the server's prefix.
        /// </summary>
        /// <param name="uri">A URI template string. See <see cref="System.UriTemplate"/> for the template format.</param>
        /// <returns>An <c>IObservable</c> to which you should <c>Subscribe()</c></returns>
        public IObservable<RequestContext> PUT(string uri)
        {
            return OnUriAndMethod(uri, "PUT");
        }

        /// <summary>
        /// Returns an observable sequence of HEAD requests that conform to the given URI template, relative to the server's prefix.
        /// </summary>
        /// <param name="uri">A URI template string. See <see cref="System.UriTemplate"/> for the template format.</param>
        /// <returns>An <c>IObservable</c> to which you should <c>Subscribe()</c></returns>
        public IObservable<RequestContext> HEAD(string uri)
        {
            return OnUriAndMethod(uri, "HEAD");
        }

        /// <summary>
        /// Returns an observable sequence of OPTIONS requests that conform to the given URI template, relative to the server's prefix.
        /// </summary>
        /// <param name="uri">A URI template string. See <see cref="System.UriTemplate"/> for the template format.</param>
        /// <returns>An <c>IObservable</c> to which you should <c>Subscribe()</c></returns>
        public IObservable<RequestContext> OPTIONS(string uri)
        {
            return OnUriAndMethod(uri, "OPTIONS");
        }

        /// <summary>
        /// Returns an observable sequence of DELETE requests that conform to the given URI template, relative to the server's prefix.
        /// </summary>
        /// <param name="uri">A URI template string. See <see cref="System.UriTemplate"/> for the template format.</param>
        /// <returns>An <c>IObservable</c> to which you should <c>Subscribe()</c></returns>
        public IObservable<RequestContext> DELETE(string uri)
        {
            return OnUriAndMethod(uri, "DELETE");
        }

        /// <summary>
        /// Returns an observable sequence of TRACE requests that conform to the given URI template, relative to the server's prefix.
        /// </summary>
        /// <param name="uri">A URI template string. See <see cref="System.UriTemplate"/> for the template format.</param>
        /// <returns>An <c>IObservable</c> to which you should <c>Subscribe()</c></returns>
        public IObservable<RequestContext> TRACE(string uri)
        {
            return OnUriAndMethod(uri, "TRACE");
        }

        /// <summary>
        /// Returns an observable sequence of HTTP requests, of any method, that conform to the given URI template, relative to the server's prefix.
        /// </summary>
        /// <param name="uri">A URI template string. See <see cref="System.UriTemplate"/> for the template format.</param>
        /// <returns>An <c>IObservable</c> to which you should <c>Subscribe()</c></returns>
        public IObservable<RequestContext> RAW(string uri)
        {
            return OnUriAndMethod(uri, null);
        }

        /// <summary>
        /// Returns an observable sequence of HTTP requests that conform to the given URI template, relative to the server's prefix.
        /// </summary>
        /// <param name="uri">A URI template string. See <see cref="System.UriTemplate"/> for the template format.</param>
        /// <param name="method">The method by which the HTTP request should be called. Pass in upper case. Pass <c>null</c> for any method</param>
        /// <returns>An <c>IObservable</c> to which you should <c>Subscribe()</c></returns>
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