using System.Collections.Generic;
using Anna.Request;

namespace Anna.Responses
{
    public class EmptyResponse : Response
    {
        public EmptyResponse(RequestContext context, int statusCode = 204, IEnumerable<KeyValuePair<string, string>> headers = null) : base(context, headers: headers)
        {
            StatusCode = statusCode;
        }
    }
}