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
        None,

        /// <summary>
        /// Transcode to MP3 320Kbps
        /// </summary>
        MP3_320Kbps,

        /// <summary>
        /// Transcode to MP3 256Kbps
        /// </summary>
        MP3_256Kbps,

        /// <summary>
        /// Transcode to MP3 192Kbps
        /// </summary>
        MP3_192Kbps,

        /// <summary>
        /// Transcode to MP3 128Kbps
        /// </summary>
        MP3_128Kbps,

        /// <summary>
        /// Transcode to WAV
        /// </summary>
        WAV
    }
}
