using System.Collections.Generic;
using System.Text;
using Anna.Request;

namespace Anna.Responses
{
    public class StringResponse : BinaryResponse
    {
        public StringResponse(RequestContext context, string message, int statusCode = 200, IDictionary<string, string> headers = null)
            : base(context, Encoding.UTF8.GetBytes(message), statusCode, headers)
        {
        }
    }
}