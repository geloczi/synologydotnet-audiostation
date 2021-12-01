using System.Net.Http.Headers;

namespace SynologyDotNet.AudioStation.Model
{
    public class SongDataRange
    {
        public string Unit { get; set; }
        public long? From { get; set; }
        public long? To { get; set; }
        public long? Length { get; set; }
        public SongDataRange(ContentRangeHeaderValue r)
        {
            Unit = r.Unit;
            From = r.From;
            To = r.To;
            Length = r.Length;
        }
    }
}
