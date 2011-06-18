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
            WriteStream = s => Observable.Return(s);
            StatusCode = 200;
            Headers = new Dictionary<string, string>{{"Content-Type", "text/html"}};
        }

        public int StatusCode { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public Func<Stream, IObservable<Stream>> WriteStream { get; set; }

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