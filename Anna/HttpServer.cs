using System;
using System.Net;
using System.Reactive.Linq;

namespace Anna
{
    public class HttpServer : IObservable<RequestContext>, IDisposable
    {
        private readonly HttpListener listener;
        private readonly IObservable<RequestContext> stream;

        public HttpServer(string url)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            stream = ObservableHttpContext();
        }

        private IObservable<RequestContext> ObservableHttpContext()
        {
            return Observable.Create<RequestContext>(obs =>
                                Observable.FromAsyncPattern<HttpListenerContext>(listener.BeginGetContext, 
                                                                                 listener.EndGetContext)()
                                          .Select(c => new RequestContext(c.Request, c.Response))
                                          .Subscribe(obs))
                             .Repeat().Retry()
                             .Publish().RefCount();
        }
        public void Dispose()
        {
            listener.Stop();
        }

        public IDisposable Subscribe(IObserver<RequestContext> observer)
        {
            return stream.Subscribe(observer);
        }


    }

    public static class  HttpServerExtensions
    {
        public static IObservable<RequestContext> OnGET(this IObservable<RequestContext> context)
        {
            return context.Where(ctx => ctx.Request.HttpMethod == "GET");
        }

        public static IObservable<RequestContextWithArgs> OnGET(this IObservable<RequestContext> context, string uri)
        {
            return context.Where(ctx => ctx.Request.HttpMethod == "GET").OnUri(uri);
        }

        public static IObservable<RequestContextWithArgs> OnUri(this IObservable<RequestContext> context, string uri)
        {
            var uriTemplate = new UriTemplate(uri);
            return Observable.Create<RequestContextWithArgs>(obs => context.Subscribe(ctx =>
                  {
                      var serverPath = ctx.Request.Url.AbsoluteUri
                                .Substring(0, ctx.Request.Url.AbsoluteUri.Length - ctx.Request.Url.AbsolutePath.Length);
                      var uriTemplateMatch = uriTemplate.Match(new Uri(serverPath), ctx.Request.Url);
                      if (uriTemplateMatch != null)
                      {
                          obs.OnNext(new RequestContextWithArgs(ctx, uriTemplateMatch.BoundVariables));
                      }
                  }, obs.OnError, obs.OnCompleted));
        }
    }
}