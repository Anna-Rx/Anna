using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace Anna.Request
{
    public class Request
    {
        public string HttpMethod { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public Stream InputStream { get; set; }
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

        public dynamic UriArguments { get; private set; }
    }
}