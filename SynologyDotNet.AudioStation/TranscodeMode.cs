using System.ComponentModel;

namespace SynologyDotNet.AudioStation
{
    /// <summary>
    /// Transcode mode
    /// </summary>
    public enum TranscodeMode
    {
        /// <summary>
        /// No transcoding, the original data will be returned by the server.
        /// </summary>
        [Description("None")]
        None,

        /// <summary>
        /// Transcode to MP3 320Kbps
        /// </summary>
        [Description("MP3 320Kbps")]
        MP3_320Kbps,

        /// <summary>
        /// Transcode to MP3 256Kbps
        /// </summary>
        [Description("MP3 256Kbps")]
        MP3_256Kbps,

        /// <summary>
        /// Transcode to MP3 192Kbps
        /// </summary>
        [Description("MP3 192Kbps")]
        MP3_192Kbps,

        /// <summary>
        /// Transcode to MP3 128Kbps
        /// </summary>
        [Description("MP3 128Kbps")]
        MP3_128Kbps,

        /// <summary>
        /// Transcode to WAV
        /// </summary>
        [Description("WAV")]
        WAV
    }
}
