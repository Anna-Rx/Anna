using System.Text;
using Anna.Request;

namespace Anna.Responses
{
    public class StringResponse : BinaryResponse
    {
        public StringResponse(RequestContext context, string message, int statusCode = 200)
            : base(context, Encoding.UTF8.GetBytes(message), statusCode)
        {
        }
    }
}