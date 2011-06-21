using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;

namespace Anna.Responses
{
    public class Response
    {
        public Response()
        {
            StatusCode = 200;
            Headers = new Dictionary<string, string>{{"Content-Type", "text/html"}};
        }
       

        public int StatusCode { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public virtual IObservable<Stream> WriteStream(Stream s)
        {
            return Observable.Return(s);
        }

        public static implicit operator Response(int statusCode)
        {
            return new EmptyResponse(statusCode);
        }

        public static implicit operator Response(string content)
        {
            return new StringResponse(content);
        }
    }
}