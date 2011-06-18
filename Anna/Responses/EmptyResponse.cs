namespace Anna.Responses
{
    public class EmptyResponse : Response
    {
        public EmptyResponse(int statusCode = 204)
        {
            StatusCode = statusCode;
        }
    }
}