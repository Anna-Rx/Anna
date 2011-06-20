using System;
using System.Collections.Generic;
using System.Linq;
using Anna.Request;

namespace Anna.Observers
{
    public class UnhandledRouteObserver : IObserver<RequestContext>
    {
        private readonly IList<Tuple<string, string>> handledRoutes;

        public UnhandledRouteObserver(IList<Tuple<string,string>> handledRoutes)
        {
            this.handledRoutes = handledRoutes;
        }

        public void OnNext(RequestContext value)
        {
            var isHandled = handledRoutes.Any(r =>
                                                  {
                                                      if (r.Item2 != value.Request.HttpMethod) return false;

                                                      var serverPath = value.Request.Url.AbsoluteUri
                                                          .Substring(0, value.Request.Url.AbsoluteUri.Length - value.Request.Url.AbsolutePath.Length);
                                                      
                                                      var uriTemplate = new UriTemplate(r.Item1);
                                                      var uriTemplateMatch = uriTemplate.Match(new Uri(serverPath), value.Request.Url);
                                                      return uriTemplateMatch != null;
                                                  });

            if (isHandled) return;

            value.Respond(404);
        }

        public void OnError(Exception error)
        {}

        public void OnCompleted()
        {}
    }
}