using System.Text;
using Anna.Request;

namespace Anna.Responses
{
    public class StringResponse : BinaryResponse
    {
        public StringResponse(RequestContext context, string message)
            : base(context, Encoding.UTF8.GetBytes(message))
        {
        }
    }
}