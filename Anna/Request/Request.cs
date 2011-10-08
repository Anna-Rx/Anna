using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using System.Reactive.Linq;

namespace Anna.Request
{
    public class Request
    {
        
        public string HttpMethod { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public Stream InputStream { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string RawUrl { get { return Url.ToString(); } }
        
        public int ContentLength
        {
            get { return int.Parse(Headers["Content-Length"].First()); }
        }

        private Uri url;
        public Uri Url
        {
            get { return url; }
            set
            {
                url = value;
                QueryString = new ArgumentsDynamic(HttpUtility.ParseQueryString(url.Query));
            }
        }

        public dynamic QueryString { get; private set; }
        internal void LoadArguments(NameValueCollection nameValueCollection)
        {
            UriArguments = new ArgumentsDynamic(nameValueCollection);
        }

        /// <summary>
        /// Returns a single-element observable sequence of the request body (typically only present for POST and PUT requests) as a string, 
        /// using the encoding specified in the HTTP request. Note: this method closes the <c>InputStream</c> can thus can only be 
        /// called once per request. Also, if the Content-Length HTTP header is not correctly set, the wrong amount of data may be read. 
        /// </summary>
        /// <param name="MaxContentLength">The maximum amount of bytes that will be read, to avoid memory issues with large uploads. Defaults to 50 kB. Use InputStream directly to read chunked data if you expect large uploads.</param>
        /// <returns>A single-element observable that contains the request body. Subscribe to it to asynchronously read the </returns>
        public IObservable<string> GetBody(int MaxContentLength = 50000)
        {
            int bufferSize = Math.Min(MaxContentLength, ContentLength);
            
            var reader = new StreamReader(InputStream, ContentEncoding);
            byte[] buffer = new byte[bufferSize];

            return Observable.FromAsyncPattern<byte[], int, int, int>(InputStream.BeginRead, InputStream.EndRead)(buffer, 0, bufferSize)
                .Select(bytesRead => ContentEncoding.GetString(buffer, 0, bytesRead));            
        }

        public dynamic UriArguments { get; private set; }
    }
}