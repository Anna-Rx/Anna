using System;
using System.IO;
using System.Reactive.Linq;

namespace Anna.Responses
{
    public class BinaryResponse : Response
    {
        private readonly byte[] binary;

        public BinaryResponse(byte[] binary)
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