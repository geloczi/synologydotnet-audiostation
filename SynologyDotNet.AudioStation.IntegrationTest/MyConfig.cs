namespace SynologyDotNet.AudioStation.IntegrationTest
{
    public class MyConfig
    {
        /// <summary>
        /// Song title
        /// </summary>
        public string TestSongTitle { get; set; } = string.Empty;

        /// <summary>
        /// Playlist name
        /// </summary>
        public string TestPlaylistName { get; set; } = $"{nameof(SynologyDotNet)}_{nameof(AudioStation)}_Test";
    }
}
