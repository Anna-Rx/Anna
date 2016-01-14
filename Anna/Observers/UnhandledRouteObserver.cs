using System;
using System.Collections.Generic;
using System.Linq;
using Anna.Request;

namespace Anna.Observers
{
    class UnhandledRouteObserver : IObserver<RequestContext>
    {
        private List<IUrlMatcher> routeMatchers;
        
        public UnhandledRouteObserver(List<IUrlMatcher> routeMatchers)
        {
            this.routeMatchers = routeMatchers;
        }

        public void OnNext(RequestContext value)
        {
            var isHandled = routeMatchers.Any(r => r.Matches(value));

            if (isHandled) return;

            value.Response(statusCode: 404).Send();
        }

        public void OnError(Exception error)
        {}

        public void OnCompleted()
        {}
    }
}