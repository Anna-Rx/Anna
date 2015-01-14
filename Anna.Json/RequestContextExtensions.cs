using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anna.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Anna.Json
{
    public static class RequestContextExtensions
    {
        public static T Get<T>(this RequestContext ctx)
        {
            JObject values;
            var request = ctx.Request;

            if (request.HttpMethod == "POST")
            {
                var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);

                values = JObject.Parse(reader.ReadToEnd());
            }
            else
            {
                values = new JObject();
            }

            IEnumerable<KeyValuePair<string, string>> queryParams = request.QueryString;

            if (queryParams != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in queryParams)
                {
                    values.Add(keyValuePair.Key,keyValuePair.Value);
                }
            }

            IEnumerable<KeyValuePair<string, string>> urlArgs = request.UriArguments;

            if (urlArgs != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in urlArgs)
                {
                    values.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return values.ToObject<T>();
        }

        public static void Respond<T>(this RequestContext requestContext, T tReturn, int statusCode = 200, IDictionary<string, string> headers = null)
        {
            string body = JsonConvert.SerializeObject(tReturn);

            requestContext.Respond(body, statusCode, headers);
        }
    }
}
