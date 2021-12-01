
namespace SynologyDotNet.AudioStation.Model
{
    public class SongData
    {
        public byte[] Data { get; set; }
        public string Type { get; set; }
        public long Length { get; set; }
        public SongDataRange Range { get; set; }
    }
}
