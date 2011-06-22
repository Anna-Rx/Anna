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
                        .Subscribe(s =>
                        {
                            s.Close();
                            s.Dispose();
                        }, e =>
                               {
                                   try
                                   {
                                       listenerResponse.StatusCode = 500;
                                       listenerResponse.OutputStream.Close();
                                   }catch{} //swallow exceptions
                               });    
            
        }
    }
}