using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Anna.Request;
using System.Net;

namespace Anna.Responses
{
    public class Response
    {
        private readonly HttpListenerResponse listenerResponse;

        public Response(RequestContext context, int statusCode = 200)
        {
            StatusCode = statusCode;
            Headers = new Dictionary<string, string>{{"Content-Type", "text/html"}};
            this.listenerResponse = context.ListenerResponse;
        }
       
        public int StatusCode { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public virtual IObservable<Stream> WriteStream(Stream s)
        {
            return Observable.Return(s);
        }

        //public static implicit operator Response(int statusCode)
        //{
        //    return new EmptyResponse(statusCode);
        //}

        //public static implicit operator Response(string content)
        //{
        //    return new StringResponse(content);
        //}

        public void Send()
        {
            foreach (var header in Headers.Where(r => r.Key != "Content-Type"))
            {
                listenerResponse.AddHeader(header.Key, header.Value);
            }

            listenerResponse.ContentType = Headers["Content-Type"];
            listenerResponse.StatusCode = StatusCode;
            WriteStream(listenerResponse.OutputStream)
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
                            }
                            catch { } //swallow exceptions
                        });    
            
        }
    }
}