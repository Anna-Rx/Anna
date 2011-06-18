using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Net;
using Anna.Responses;
using AsyncHttp.Server;

namespace Anna
{
    public class RequestContext
    {
        private readonly HttpListenerResponse listenerResponse;

        public RequestContext(HttpListenerRequest request, HttpListenerResponse response)
        {
            listenerResponse = response;
            Request = MapRequest(request);
        }

        public RequestContext()
        {
        }

        private static Request MapRequest(HttpListenerRequest request)
        {
            var mapRequest = new Request
                                 {
                                     Headers = request.Headers.ToDictionary(),
                                     HttpMethod = request.HttpMethod,
                                     InputStream = request.InputStream,
                                     Url = request.Url
                                 };
            return mapRequest;
        }

        public virtual Request Request { get; private set; }

        public virtual void Respond(Response response)
        {
            foreach (var header in response.Headers.Where(r => r.Key != "Content-Type"))
            {
                listenerResponse.AddHeader(header.Key, header.Value);
            }

            listenerResponse.ContentType = response.Headers["Content-Type"];
            listenerResponse.StatusCode = response.StatusCode;

            response.WriteStream(listenerResponse.OutputStream)
                    .Subscribe(s => s.Close());
        }
    }

    public class RequestContextWithArgs : RequestContext
    {
        public RequestContextWithArgs(RequestContext requestContext, NameValueCollection args)
        {
            this.requestContext = requestContext;
            var dictionary = args.Keys.OfType<string>()
                                 .ToDictionary(k => k.ToString(), 
                                                    k => args[k], 
                                                    StringComparer.InvariantCultureIgnoreCase);

            Args = new ArgumentsDynamic(dictionary);
        }

        private readonly RequestContext requestContext;

        public dynamic Args { get; private set; }

        public override Request Request
        {
            get { return requestContext.Request; }
        }

        public override void Respond(Response response)
        {
            requestContext.Respond(response);
        }

        public class ArgumentsDynamic : DynamicObject
        {
            private readonly IDictionary<string, string> args;

            public ArgumentsDynamic(IDictionary<string, string> args)
            {
                this.args = args;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {        // Converting the property name to lowercase
                // so that property names become case-insensitive.
                var name = binder.Name.ToLower();

                // If the property name is found in a dictionary,
                // set the result parameter to the property value and return true.
                // Otherwise, return false.
                string value;
                if (!args.TryGetValue(name, out value))
                {
                    result = null;
                    return false;
                }
                result = value;
                return true;
            }
        }
    }
}