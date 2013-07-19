using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Web;
using AsyncHttp.Server;

namespace Anna.Request
{
    public class Request
    {
        private readonly HttpListenerRequest request;

        public HttpListenerRequest ListenerRequest { get { return request; } }

        public string HttpMethod { get { return request.HttpMethod; } }

        public IDictionary<string, IEnumerable<string>> Headers { get; set; }

        public Stream InputStream { get { return request.InputStream; } }

        public Encoding ContentEncoding { get { return request.ContentEncoding; } }

        public string RawUrl { get { return request.Url.ToString(); } }

        public int ContentLength
        {
            get { return int.Parse(Headers["Content-Length"].First()); }
        }

        public Uri Url { get { return request.Url; } }

        public dynamic UriArguments { get; private set; }

        public dynamic QueryString { get; private set; }

        public Request(HttpListenerRequest request)
        {
            this.request = request;
            Headers = request.Headers.ToDictionary();
            QueryString = new ArgumentsDynamic(HttpUtility.ParseQueryString(request.Url.Query));
        }

        /// <summary>
        /// Returns a single-element observable sequence of the request body (typically only present for POST and PUT requests) as a string, 
        /// using the encoding specified in the HTTP request. Note: this method closes the <c>InputStream</c> can thus can only be 
        /// called once per request. Also, if the Content-Length HTTP header is not correctly set, the wrong amount of data may be read. 
        /// </summary>
        /// <param name="maxContentLength">The maximum amount of bytes that will be read, to avoid memory issues with large uploads. Defaults to 50 kB. Use InputStream directly to read chunked data if you expect large uploads.</param>
        /// <returns>A single-element observable that contains the request body. Subscribe to it to asynchronously read the </returns>
        public IObservable<string> GetBody(int maxContentLength = 50000)
        {
            int bufferSize = Math.Min(maxContentLength, ContentLength);

            var reader = new StreamReader(InputStream, ContentEncoding);
            byte[] buffer = new byte[bufferSize];

            return Observable.FromAsyncPattern<byte[], int, int, int>(InputStream.BeginRead, InputStream.EndRead)(buffer, 0, bufferSize)
                .Select(bytesRead => ContentEncoding.GetString(buffer, 0, bytesRead));
        }

        internal void LoadArguments(NameValueCollection nameValueCollection)
        {
            UriArguments = new ArgumentsDynamic(nameValueCollection);
        }
    }
}