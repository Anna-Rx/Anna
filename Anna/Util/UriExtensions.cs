using System;

namespace Anna.Util
{
    public static class UriExtensions
    {
        public static string GetServerBaseUri(this Uri uri)
        {
            var serverPath = uri.AbsoluteUri
                            .Substring(0, uri.AbsoluteUri.Length 
                                            - (uri.AbsolutePath.Length + uri.Query.Length));
            return serverPath;
        }
    }
}