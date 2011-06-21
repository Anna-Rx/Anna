using System;
using System.IO;
using System.Reactive.Linq;
using Anna.Observables;

namespace Anna.Responses
{
    public class StaticFileResponse : Response
    {
        private readonly string file;
        private readonly int chunkSize;

        public StaticFileResponse(string file, int chunkSize = 1024)
        {
            this.file = file;
            this.chunkSize = chunkSize;
        }

        public override IObservable<Stream> WriteStream(Stream stream)
        {
            var writer = Observable.FromAsyncPattern<byte[], int, int>(stream.BeginWrite, stream.EndWrite);
            return Observable.Create<Stream>(obs => new ObservableFromFile(file, chunkSize)
                                                        .Subscribe(b => writer(b, 0, b.Length), obs.OnError,
                                                                   () =>
                                                                       {
                                                                           obs.OnNext(stream);
                                                                           obs.OnCompleted();
                                                                       }));
        }
    }
}