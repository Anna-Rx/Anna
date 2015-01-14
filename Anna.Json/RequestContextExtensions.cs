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
        /// <summary>
        /// Get Request data as a particular type. UriArguements, QueryString, and POST data will be used to satisfy the model
        /// </summary>
        /// <typeparam name="T">type being requested</typeparam>
        /// <param name="ctx">context</param>
        /// <returns></returns>
        public static T GetAs<T>(this RequestContext ctx)
        {
            var request = ctx.Request;
            JObject values;

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

        /// <summary>
        /// Respond extension that serializes an instance using Json and responds
        /// </summary>
        /// <typeparam name="T">type of instance being sent</typeparam>
        /// <param name="requestContext">request context</param>
        /// <param name="tReturn">instance being returned</param>
        /// <param name="statusCode">status code</param>
        /// <param name="headers">headers for call</param>
        public static void RespondAs<T>(this RequestContext requestContext, T tReturn, int statusCode = 200, IDictionary<string, string> headers = null)
        {
            string body = JsonConvert.SerializeObject(tReturn);

            requestContext.Respond(body, statusCode, headers);
        }
    }
}
