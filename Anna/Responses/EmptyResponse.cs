using Anna.Request;
namespace Anna.Responses
{
    public class EmptyResponse : Response
    {
        public EmptyResponse(RequestContext context, int statusCode = 204) : base(context)
        {
            StatusCode = statusCode;
        }
    }
}