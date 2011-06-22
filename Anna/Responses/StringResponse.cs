using System.Text;

namespace Anna.Responses
{
    public class StringResponse : BinaryResponse
    {
        public StringResponse(string message)
            : base(Encoding.UTF8.GetBytes(message))
        {
        }
    }
}