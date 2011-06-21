using System;
using System.IO;
using System.Reactive.Linq;
using System.Text;

namespace Anna.Responses
{
    public class StringResponse : Response
    {
        private readonly string message;

        public StringResponse(string message)
        {
            this.message = message;
        }

        public override IObservable<Stream> WriteStream(Stream stream)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            return Observable.FromAsyncPattern<byte[], int, int>(stream.BeginWrite, stream.EndWrite)(bytes, 0, bytes.Length)
                .Select(u => stream);
        }
    }
}