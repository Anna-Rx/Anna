using System;
using Anna.Util;

namespace Anna.Request
{
    class UrlTemplateMatcher : IUrlMatcher
    {
        private readonly string uri;
        private readonly string method;

        public UrlTemplateMatcher(string uri, string method)
        {
            this.uri = uri;
            this.method = method;
        }

        public bool Matches(RequestContext ctx)
        {
            if (!String.IsNullOrEmpty(method) && ctx.Request.HttpMethod != method) return false;

            var uriTemplate = new UriTemplate(uri);
            var serverPath = ctx.Request.Url.GetServerBaseUri();
            var uriTemplateMatch = uriTemplate.Match(new Uri(serverPath), ctx.Request.Url);
            if (uriTemplateMatch == null) return false;

            ctx.Request.LoadArguments(uriTemplateMatch.BoundVariables);
            return true;
        }
    }
}