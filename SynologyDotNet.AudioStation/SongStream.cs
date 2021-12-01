using System;
using System.IO;
using System.Threading;

namespace SynologyDotNet.AudioStation
{
    public class SongStream
    {
        public Stream Stream { get; }
        public string ContentType { get; }
        public long ContentLength { get; set; }
        public CancellationToken CancellationToken { get; }

        public SongStream(Stream stream, string contentType, long contentLength, CancellationToken cancellationToken)
        {
            Stream = stream;
            ContentType = contentType;
            ContentLength = contentLength;
            CancellationToken = cancellationToken;

            //// Set timeout to infinite can throw an exception if it's not supported.
            //try
            //{
            //    //stream.ReadTimeout = Timeout.Infinite;
            //    stream.ReadTimeout = 10000;
            //}
            //catch { }
        }
    }
}
