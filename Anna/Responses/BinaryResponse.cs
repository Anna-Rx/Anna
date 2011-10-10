using System;
using System.IO;
using System.Reactive.Linq;
using Anna.Request;

namespace Anna.Responses
{
    public class BinaryResponse : Response
    {
        private readonly byte[] binary;

        public BinaryResponse(RequestContext context, byte[] binary, int statusCode = 200)
            : base(context, statusCode)
        {
            this.binary = binary;
        }

        public override IObservable<Stream> WriteStream(Stream stream)
        {
            return Observable.FromAsyncPattern<byte[], int, int>(stream.BeginWrite, stream.EndWrite)(binary, 0, binary.Length)
                .Select(u => stream);
        }
    }
}