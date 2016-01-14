using System;
using Anna.Util;

namespace Anna.Request
{
    class UrlMatcher : IUrlMatcher
    {
        private readonly string uri;
        private readonly string method;

        public UrlMatcher(string uri, string method)
        {
            this.uri = uri;
            this.method = method;
        }

        public bool Matches(RequestContext ctx)
        {
            if (!String.IsNullOrEmpty(method) && ctx.Request.HttpMethod != method) return false;

            var serverPath = ctx.Request.Url.GetServerBaseUri();
            return new Uri(serverPath + uri).Equals(ctx.Request.Url);
        }
    }
}