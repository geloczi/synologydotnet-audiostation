using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.IntegrationTest
{
    [TestClass]
    public class AudioStationConnector_Tests : MyTestBase
    {
        const int TestPageSize = 10;
        //const string SessionName = "AudioStation";
        static SynoClient SynoClient;
        static AudioStationClient AudioStation;
        static Song TestSong;
        static Playlist TestPlaylist;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Connect to the server and login
            AudioStation = new AudioStationClient();
            SynoClient = new SynoClient(new Uri(CoreConfig.Server), true, AudioStation);
            var session = SynoClient.LoginAsync(CoreConfig.Username, CoreConfig.Password/*, SessionName*/).Result;

            Assert.IsNotNull(session);
            Assert.IsNotNull(session.Cookie);
            Assert.IsTrue(session.Cookie.Length > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(session.Id));
            Assert.IsFalse(string.IsNullOrWhiteSpace(session.Token));
            //Assert.AreEqual(SessionName, session.Name);

            UpdateTestSong();
            UpdateTestPlaylist();
        }

        // This is not a test method
        private static void UpdateTestSong()
        {
            var response = AudioStation.ListSongsAsync(1, 0, SongQueryAdditional.All,
                (SongQueryParameters.artist, Config.TestArtist),
                (SongQueryParameters.album_artist, Config.TestArtist),
                (SongQueryParameters.album, Config.TestAlbum)).Result;
            Assert.AreNotEqual(response.Data.Songs.Length, 0, "Please make sure that your '/music' folder contains music files.");
            TestSong = response.Data.Songs.First();
            AssertSong(TestSong, SongQueryAdditional.All);
        }

        private static void UpdateTestPlaylist()
        {
            var response = AudioStation.ListPlaylistsAsync(1000, 0).Result;
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0, $"Please create a playlist in Audio Station with this name: {Config.TestPlaylistName}");
            Assert.IsTrue(response.Data.playlists.Length > 0);
            TestPlaylist = response.Data.playlists.First(x => x.name == Config.TestPlaylistName);
        }

        [TestMethod]
        public async Task ListArtists()
        {
            var response = await AudioStation.ListArtistsAsync(TestPageSize, 0);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Artists.Length > 0);
            Assert.IsTrue(response.Data.Artists.Length <= TestPageSize);
        }

        [TestMethod]
        public async Task ListAlbums()
        {
            var response = await AudioStation.ListAlbumsAsync(TestPageSize, 0);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Albums.Length > 0);
            Assert.IsTrue(response.Data.Albums.Length <= TestPageSize);
        }

        [TestMethod]
        public async Task ListLatestAlbums()
        {
            var response = await AudioStation.ListAlbumsAsync(TestPageSize, 0, null, (AlbumQueryParameters.sort_by, AlbumSortBy.Time), (AlbumQueryParameters.sort_direction, SortDirection.Descending));
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Albums.Length > 0);
            Assert.IsTrue(response.Data.Albums.Length <= TestPageSize);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Albums[0].Name));
        }

        [TestMethod]
        public async Task ListSongs_All() => await ListSongs(SongQueryAdditional.All);
        [TestMethod]
        public async Task ListSongs_None() => await ListSongs(SongQueryAdditional.None);
        [TestMethod]
        public async Task ListSongs_song_audio() => await ListSongs(SongQueryAdditional.song_audio);
        [TestMethod]
        public async Task ListSongs_song_rating() => await ListSongs(SongQueryAdditional.song_rating);
        [TestMethod]
        public async Task ListSongs_song_tag() => await ListSongs(SongQueryAdditional.song_tag);
        private async Task ListSongs(SongQueryAdditional queryAdditional)
        {
            var response = await AudioStation.ListSongsAsync(TestPageSize, 0, queryAdditional);
            AssertIListResult(response);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Songs.Length > 0);
            Assert.IsTrue(response.Data.Songs.Length <= TestPageSize);
            foreach (var song in response.Data.Songs)
            {
                AssertSong(song, queryAdditional);
            }
        }

        private static void AssertIListResult(IApiListResponse lr)
        {
            Assert.IsNotNull(lr);
            Assert.IsTrue(lr.Success);
            Assert.IsNotNull(lr.Data);
            Assert.IsNull(lr.Error);
        }

        [TestMethod]
        public async Task ListSongsPagination()
        {
            var bigPage = await AudioStation.ListSongsAsync(100, 0, SongQueryAdditional.None);
            AssertIListResult(bigPage);
            Assert.IsTrue(bigPage.Data.Songs.Length == 100, "You need at least 100 songs to run this test.");
            foreach (var song in bigPage.Data.Songs)
            {
                AssertSong(song, SongQueryAdditional.None);
            }

            // Test with page size = 10
            // Total items = 100
            for (int page = 0; page < 10; page++)
            {
                // Get items
                var offset = 10 * page;
                var smallPage = await AudioStation.ListSongsAsync(10, offset, SongQueryAdditional.None);
                AssertIListResult(smallPage);

                // Check items
                for (int i = 0; i < smallPage.Data.Songs.Length; i++)
                {
                    AssertSong(smallPage.Data.Songs[i], SongQueryAdditional.None);
                    Assert.AreEqual(smallPage.Data.Songs[i].ID, bigPage.Data.Songs[offset + i].ID);
                }
            }
        }

        [TestMethod]
        public async Task ListArtistAlbumSongs()
        {
            var response = await AudioStation.ListSongsAsync(TestPageSize, 0, SongQueryAdditional.All,
                (SongQueryParameters.artist, Config.TestArtist),
                (SongQueryParameters.album_artist, Config.TestArtist),
                (SongQueryParameters.album, Config.TestAlbum),
                (SongQueryParameters.sort_by, SongSortBy.track),
                (SongQueryParameters.sort_direction, SortDirection.Descending));
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Songs.Length >= 2);
            Assert.IsTrue(response.Data.Songs.Length <= TestPageSize);
            foreach (var song in response.Data.Songs)
            {
                AssertSong(song, SongQueryAdditional.All);
            }

            // List sort check (descending order)
            for (int i = 1; i < response.Data.Songs.Length; i++)
                Assert.IsTrue(response.Data.Songs[i - 1].Additional.Tag.Track > response.Data.Songs[i].Additional.Tag.Track);

            response = AudioStation.ListSongsAsync(TestPageSize, 0, SongQueryAdditional.All,
                (SongQueryParameters.artist, Config.TestArtist),
                (SongQueryParameters.album_artist, Config.TestArtist),
                (SongQueryParameters.album, Config.TestAlbum),
                (SongQueryParameters.sort_by, SongSortBy.track),
                (SongQueryParameters.sort_direction, SortDirection.Ascending)
            ).Result;
            foreach (var song in response.Data.Songs)
            {
                AssertSong(song, SongQueryAdditional.All);
            }

            // List sort check (ascending order)
            for (int i = 1; i < response.Data.Songs.Length; i++)
                Assert.IsTrue(response.Data.Songs[i - 1].Additional.Tag.Track < response.Data.Songs[i].Additional.Tag.Track);
        }

        [TestMethod]
        public async Task RateSong()
        {
            Assert.IsTrue((await AudioStation.RateSongAsync(TestSong.ID, 4)).Success);
            UpdateTestSong();
            Assert.IsTrue(TestSong.Additional.Rating.Value == 4);
            Assert.IsTrue((await AudioStation.RateSongAsync(TestSong.ID, 5)).Success);
            UpdateTestSong();
            Assert.IsTrue(TestSong.Additional.Rating.Value == 5);
        }

        [TestMethod]
        public async Task ListFolders()
        {
            // Get top level folders
            var response = await AudioStation.ListFoldersAsync(TestPageSize, 0);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Items.Length > 0);
            Assert.IsTrue(response.Data.Items.Length <= TestPageSize);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Items[0].Title));

            // Subfolder test
            var folderToList = response.Data.Items[0].ID;
            response = await AudioStation.ListFoldersAsync(TestPageSize, 0, folderToList);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Items.Length > 0, $"Make sure that {folderToList} contains at least one subfolder!");
            Assert.IsTrue(response.Data.Items.Length <= TestPageSize);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Items[0].Title));
        }

        [TestMethod]
        public async Task GetArtistCover()
        {
            var response = await AudioStation.GetArtistCoverAsync(Config.TestArtist);
            Assert.IsFalse(response is null);
            Assert.IsFalse(response.Data is null);
            Assert.IsTrue(response.Data.Length > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Type));
        }

        [TestMethod]
        public async Task GetArtistCover_NotFound()
        {
            var response = await AudioStation.GetArtistCoverAsync("This artist does not even exist 123456789");
            Assert.IsTrue(response is null);
        }

        [TestMethod]
        public async Task GetAlbumCover()
        {
            var response = await AudioStation.GetAlbumCoverAsync(Config.TestArtist, Config.TestAlbum);
            Assert.IsFalse(response is null);
            Assert.IsFalse(response.Data is null);
            Assert.IsTrue(response.Data.Length > 5000);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Type));
        }

        [TestMethod]
        public async Task GetAlbumCover_NotFound()
        {
            var response = await AudioStation.GetAlbumCoverAsync("This artist does not even exist 123456789", "This album does not even exist 123456789");
            Assert.IsTrue(response is null);
        }

        [TestMethod]
        public void TestStreamSong_MP3_320Kbps()
        {
            TestStreamSongInternal(TranscodeMode.MP3_320Kbps, "audio/mpeg");
        }

        [TestMethod]
        public void TestStreamSong_WAV()
        {
            TestStreamSongInternal(TranscodeMode.WAV, "audio/x-wav");
        }

        [TestMethod]
        public void TestStreamSong_None()
        {
            TestStreamSongInternal(TranscodeMode.None, null);
        }

        private void TestStreamSongInternal(TranscodeMode transcodeMode, string mediaType)
        {
            var tokenSource = new CancellationTokenSource();
            var downloadTask = AudioStation.StreamSongAsync(
                tokenSource.Token,
                transcodeMode,
                TestSong.ID,
                0,
                songStream =>
                {
                    if (!string.IsNullOrEmpty(mediaType))
                        Assert.AreEqual(songStream.ContentType, mediaType);
                    Assert.IsTrue(songStream.ContentLength > 0);

                    // Read 4KB just to test the download
                    var buffer = new byte[4096];
                    songStream.Stream.Read(buffer, 0, buffer.Length);
                    // There must be at least one non-zero byte in a 4KB chunk
                    Assert.IsTrue(buffer.Any(b => b > 0));
                }
            );
            downloadTask.Wait();
        }

        [TestMethod]
        public async Task Search()
        {
            var response = await AudioStation.SearchAsync(Config.TestArtist);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Data.Artists);
            Assert.IsTrue(response.Data.ArtistTotal > 0);

            response = await AudioStation.SearchAsync(Config.TestAlbum);
            Assert.IsNotNull(response.Data.Albums);
            Assert.IsTrue(response.Data.AlbumTotal > 0);

            response = await AudioStation.SearchAsync(Config.TestSongTitle);
            Assert.IsNotNull(response.Data.Songs);
            Assert.IsTrue(response.Data.SongTotal > 0);
        }

        [TestMethod]
        public async Task GetSongById()
        {
            var response = await AudioStation.GetSongByIdAsync(TestSong.ID);
            Assert.IsTrue(response.Data.Songs.Length > 0);
            Assert.AreEqual(response.Data.Songs[0].ID, TestSong.ID);
        }

        [TestMethod]
        public async Task ListPlaylists()
        {
            var response = await AudioStation.ListPlaylistsAsync(TestPageSize, 0);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.playlists.Length > 0);
        }

        [TestMethod]
        public async Task GetPlaylist()
        {
            var playlist = await AudioStation.GetPlaylistAsync(TestPageSize, 0, TestPlaylist.id);
            Assert.IsTrue(playlist.Success);
            Assert.IsFalse(string.IsNullOrEmpty(playlist.Data.id));
            Assert.IsTrue(playlist.Data.additional.songs.Length > 0);
            foreach (var song in playlist.Data.additional.songs)
            {
                Assert.IsFalse(string.IsNullOrEmpty(song.id));
                Assert.IsFalse(string.IsNullOrEmpty(song.title));
                Assert.IsFalse(string.IsNullOrEmpty(song.path));
                Assert.IsFalse(string.IsNullOrEmpty(song.type));
            }
        }

        [TestMethod]
        public async Task GetFileTags()
        {
            var response = await AudioStation.GetSongFileTags(TestSong.Path);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Files?.Length > 0);
            var file = response.Files[0];
            Assert.AreEqual(file.Path, TestSong.Path);
            Assert.AreEqual(file.Title, TestSong.Title);
            Assert.AreEqual(file.Album, TestSong.Additional.Tag.Album);
            Assert.AreEqual(file.Artist, TestSong.Additional.Tag.Artist);
        }

        [TestMethod]
        public async Task SetFileTags()
        {
            // Get
            var tag = (await AudioStation.GetSongFileTags(TestSong.Path)).Files.First();

            // Change
            var change = new FileTagChange(tag);
            change.AudioInfos.Add(tag);
            change.Comment = "Hell yeah!!! " + DateTime.Now.ToString();
            Assert.IsTrue((await AudioStation.SetSongFileTags(change)).Success);
            var tagUpdated = (await AudioStation.GetSongFileTags(TestSong.Path)).Files.First();
            Assert.AreEqual(change.Comment, tagUpdated.Comment);

            // Revert
            change.Comment = tag.Comment;
            Assert.IsTrue((await AudioStation.SetSongFileTags(change)).Success);
        }

        #region Private Methods
        private static void AssertSong(Song song, SongQueryAdditional additional)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(song.ID));
            Assert.IsFalse(string.IsNullOrWhiteSpace(song.Path));
            Assert.IsFalse(string.IsNullOrWhiteSpace(song.Title));
            Assert.IsFalse(string.IsNullOrWhiteSpace(song.Type));
            if (additional.HasFlag(SongQueryAdditional.song_audio))
            {
                Assert.IsTrue(song.Additional.Audio.Bitrate > 0);
                Assert.IsTrue(song.Additional.Audio.Channels > 0);
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.Additional.Audio.Codec));
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.Additional.Audio.Container));
                Assert.IsTrue(song.Additional.Audio.Duration > 0);
                Assert.IsTrue(song.Additional.Audio.FileSize > 0);
                Assert.IsTrue(song.Additional.Audio.Frequency > 0);
            }
            else
            {
                Assert.AreEqual(song.Additional.Audio.Bitrate, 0);
                Assert.AreEqual(song.Additional.Audio.Channels, 0);
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Audio.Codec));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Audio.Container));
                Assert.AreEqual(song.Additional.Audio.Duration, 0);
                Assert.AreEqual(song.Additional.Audio.FileSize, 0);
                Assert.AreEqual(song.Additional.Audio.Frequency, 0);
            }

            if (additional.HasFlag(SongQueryAdditional.song_rating))
            {
                Assert.IsTrue(song.Additional.Rating.Value >= 0);
                Assert.IsTrue(song.Additional.Rating.Value <= 5);
            }
            else
            {
                Assert.AreEqual(song.Additional.Rating.Value, 0);
            }

            if (additional.HasFlag(SongQueryAdditional.song_tag))
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.Additional.Tag.Album));
                Assert.IsFalse(string.IsNullOrWhiteSpace(song.Additional.Tag.AlbumArtist) && string.IsNullOrWhiteSpace(song.Additional.Tag.Artist));
                // Makes no sense to validate more metadata
            }
            else
            {
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.Album));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.AlbumArtist));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.Artist));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.Comment));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.Composer));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.Genre));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.AlbumGain));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.AlbumPeak));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.TrackGain));
                Assert.IsTrue(string.IsNullOrEmpty(song.Additional.Tag.TrackPeak));
                Assert.AreEqual(song.Additional.Tag.Disc, 0);
                Assert.AreEqual(song.Additional.Tag.Track, 0);
            }
        }
        #endregion
    }
}
