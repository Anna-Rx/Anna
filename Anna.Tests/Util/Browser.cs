using System.IO;
using System.Net;

namespace Anna.Tests.Util
{
    public class Browser
    {
        public static HttpWebResponse ExecuteGet(string url)
        {
            var request = WebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            return (HttpWebResponse) request.GetResponse();
        }

        public static HttpWebResponse ExecutePost(string url)
        {
            var request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentLength = 0;
            return (HttpWebResponse)request.GetResponse();
        }
    }

    public static class WebResponseHelpers
    {
        public static string ReadAllContent(this WebResponse response)
        {
            using(var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
    }
}