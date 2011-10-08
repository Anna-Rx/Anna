using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Anna.Responses;
using AsyncHttp.Server;

namespace Anna.Request
{
    public class RequestContext
    {
        public readonly HttpListenerResponse ListenerResponse;

        public RequestContext(HttpListenerRequest request, HttpListenerResponse response)
        {
            ListenerResponse = response;
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

        public virtual EmptyResponse Response(int statusCode = 204)
        {
            return new EmptyResponse(this, statusCode);
        }

        public virtual StringResponse Response(string body, int statusCode = 204)
        {
            return new StringResponse(this, body);
        }

        public virtual StaticFileResponse StaticFileResponse(string fileName, int chunkSize = 1024)
        {
            return new StaticFileResponse(this, fileName, chunkSize);
        }

        public virtual void Respond(int statusCode = 204)
        {
            Response(statusCode).Send();
        }

        public virtual void Respond(string body, int statusCode = 204)
        {
            Response(body, statusCode).Send();
        }

        //public virtual void Respond(Response response)
        //{
        //    response.Send();
        //}
    }
}