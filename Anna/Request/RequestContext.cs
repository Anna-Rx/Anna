using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Anna.Responses;
using AsyncHttp.Server;

namespace Anna.Request
{
    public class RequestContext
    {
        public readonly HttpListenerResponse ListenerResponse;

        public RequestContext(HttpListenerRequest request, HttpListenerResponse response)
        {
            ListenerResponse = response;
            Request = MapRequest(request);
        }

        private static Request MapRequest(HttpListenerRequest request)
        {
            var mapRequest = new Request
                                 {
                                     Headers = request.Headers.ToDictionary(),
                                     HttpMethod = request.HttpMethod,
                                     InputStream = request.InputStream,
                                     Url = request.Url
                                 };
            return mapRequest;
        }

        public virtual Request Request { get; private set; }

        /// <summary>
        /// Create an empty response with the given status code
        /// </summary>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>An empty response object that can be further tuned. </returns>
        public virtual EmptyResponse Response(int statusCode = 204)
        {
            return new EmptyResponse(this, statusCode);
        }

        /// <summary>
        /// Create a response that can send a UTF-8 encoded string message to the client. Defaults the Content-Type header to text/html.
        /// </summary>
        /// <param name="body">String message to send.</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>A response object that can be further tuned. Call Send() to send the response.</returns>
        public virtual StringResponse Response(string body, int statusCode = 200)
        {
            return new StringResponse(this, body, statusCode);
        }

        /// <summary>
        /// Create a response that can send arbitrary binary data.
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public virtual BinaryResponse Response(byte[] binary, int statusCode = 200)
        {
            return new BinaryResponse(this, binary, statusCode);
        }

        /// <summary>
        /// Create a response that can stream a static file to the client. Be sure to correctly set the Content-Type header!
        /// </summary>
        /// <param name="fileName">Location of the file to stream.</param>
        /// <param name="chunkSize"></param>
        /// <returns>A response object that can be further tuned. Call Send() to send the response.</returns>
        public virtual StaticFileResponse StaticFileResponse(string fileName, int chunkSize = 1024)
        {
            return new StaticFileResponse(this, fileName, chunkSize);
        }

        /// <summary>
        /// Send an empty response with only the specified HTTP status code. Use <c>Response()</c> to change the content type or to set additional headers.
        /// </summary>
        /// <param name="statusCode">HTTP status code</param>
        public virtual void Respond(int statusCode = 204)
        {
            Response(statusCode).Send();
        }

        /// <summary>
        /// Send a text/html response with the given HTTP status code. Use <c>Response()</c> to change the content type or to set additional headers
        /// </summary>
        /// <param name="body">HTML body of the HTTP response</param>
        /// <param name="statusCode">HTTP status code</param>
        public virtual void Respond(string body, int statusCode = 200)
        {
            Response(body, statusCode).Send();
        }
    }
}