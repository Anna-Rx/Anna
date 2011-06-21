//using System;
//using System.IO;
//using System.Reactive.Linq;
//using System.Reactive.Subjects;
//using System.Text;

//namespace Anna.Responses
//{
//    public class StaticFileResponse : Response
//    {
        
//        public StaticFileResponse(string file)
//        {
//            WriteStream = Write;
//        }

//        public IObservable<Stream> Write(Stream stream )
//        {
//            //var bytes = Encoding.UTF8.GetBytes(message);
            
//            return Observable.FromAsyncPattern<byte[], int, int>(stream.BeginWrite, stream.EndWrite)(bytes, 0, bytes.Length)
//                .Select(u => stream);
//        }

//    }


//}