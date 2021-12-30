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
        const int TestPageSize = 100;
        //const string SessionName = "AudioStation";
        static SynoClient SynoClient;
        static AudioStationClient AudioStation;
        static Song TestSong;
        static Playlist TestPlaylist;
        static string TestArtist;
        static string TestAlbum;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Connect to the server and login
            AudioStation = new AudioStationClient();
            SynoClient = new SynoClient(new Uri(CoreConfig.Server), true)
                .Add(AudioStation);
            var session = SynoClient.LoginAsync(CoreConfig.Username, CoreConfig.Password/*, SessionName*/).Result;

            Assert.IsNotNull(session);
            Assert.IsNotNull(session.Cookie);
            Assert.IsTrue(session.Cookie.Length > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(session.Id));
            Assert.IsFalse(string.IsNullOrWhiteSpace(session.Token));
            //Assert.AreEqual(SessionName, session.Name);

            LoadTestSong();
            CreateTestPlaylist();
        }

        private static void LoadTestSong()
        {
            if (string.IsNullOrEmpty(Config.TestSongTitle))
            {
                // Just get the first song
                var songListResponse = AudioStation.ListSongsAsync(1, 0, SongQueryAdditional.All).Result;
                Assert.IsTrue(songListResponse.Success);
                TestSong = songListResponse.Data.Songs.FirstOrDefault();
            }
            else
            {
                // Search test song by title
                var searchResponse = AudioStation.SearchSongsByTitleAsync(1, 0, Config.TestSongTitle, SongQueryAdditional.All).Result;
                Assert.IsTrue(searchResponse.Success);
                TestSong = searchResponse.Data.Songs.FirstOrDefault();
            }
            Assert.IsNotNull(TestSong, "Please make sure that your '/music' folder contains music files.");

            AssertSong(TestSong, SongQueryAdditional.All);
            TestArtist = TestSong.Additional.Tag.Artist;
            TestAlbum = TestSong.Additional.Tag.Album;
        }

        private static void CreateTestPlaylist()
        {
            var listResponse = AudioStation.ListPlaylistsAsync(10000, 0).Result;
            Assert.IsTrue(listResponse.Success);
            TestPlaylist = listResponse.Data.Playlists.FirstOrDefault(x => x.Name.Equals(Config.TestPlaylistName, StringComparison.OrdinalIgnoreCase));
            if (TestPlaylist is null)
            {
                // Create test playlist if does not exist
                var createResult = AudioStation.CreatePlaylistAsync(Config.TestPlaylistName, false).Result;
                listResponse = AudioStation.ListPlaylistsAsync(10000, 0).Result;
                TestPlaylist = listResponse.Data.Playlists.FirstOrDefault(x => x.ID.Equals(createResult.Data.ID, StringComparison.OrdinalIgnoreCase));
                Assert.IsNotNull(TestPlaylist);

                // Add a few songs to the test playlist
                var songsResponse = AudioStation.ListSongsAsync(5, 0, SongQueryAdditional.None).Result;
                Assert.IsTrue(songsResponse.Success);
                var addSongsResponse = AudioStation.AddSongsToPlaylist(TestPlaylist.ID, songsResponse.Data.Songs.Select(x => x.ID).ToArray()).Result;
                Assert.IsTrue(addSongsResponse.Success);
            }
        }

        [TestMethod]
        public async Task Artist_List()
        {
            var response = await AudioStation.ListArtistsAsync(TestPageSize, 0);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Artists.Length > 0);
            Assert.IsTrue(response.Data.Artists.Length <= TestPageSize);
        }

        [TestMethod]
        public async Task Album_List()
        {
            var response = await AudioStation.ListAlbumsAsync(TestPageSize, 0);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Albums.Length > 0);
            Assert.IsTrue(response.Data.Albums.Length <= TestPageSize);
        }

        [TestMethod]
        public async Task Album_ListLatest()
        {
            var response = await AudioStation.ListAlbumsAsync(TestPageSize, 0, null, (AlbumQueryParameter.sort_by, AlbumSortBy.Time), (AlbumQueryParameter.sort_direction, SortDirection.Descending));
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Albums.Length > 0);
            Assert.IsTrue(response.Data.Albums.Length <= TestPageSize);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Albums[0].Name));
        }

        [TestMethod]
        public async Task Composer_List()
        {
            var response = await AudioStation.ListComposersAsync(TestPageSize, 0);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Composers.Length > 0);
            Assert.IsTrue(response.Data.Composers.Length <= TestPageSize);
        }

        [TestMethod]
        public async Task Genre_List()
        {
            var response = await AudioStation.ListGenresAsync(TestPageSize, 0);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Genres.Length > 0);
            Assert.IsTrue(response.Data.Genres.Length <= TestPageSize);
        }

        [TestMethod]
        public async Task Song_ListWithAll() => await ListSongs(SongQueryAdditional.All);
        [TestMethod]
        public async Task Song_List() => await ListSongs(SongQueryAdditional.None);
        [TestMethod]
        public async Task Song_ListWith_song_audio() => await ListSongs(SongQueryAdditional.song_audio);
        [TestMethod]
        public async Task Song_ListWith_song_rating() => await ListSongs(SongQueryAdditional.song_rating);
        [TestMethod]
        public async Task Song_ListWith_song_tag() => await ListSongs(SongQueryAdditional.song_tag);

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
        public async Task Song_ListWithPagination()
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
        public async Task Song_FilterByArtistAlbum()
        {
            var response = await AudioStation.ListSongsAsync(TestPageSize, 0, SongQueryAdditional.All,
                (SongQueryParameter.artist, TestArtist),
                (SongQueryParameter.album_artist, TestArtist),
                (SongQueryParameter.album, TestAlbum),
                (SongQueryParameter.sort_by, SongSortBy.Track),
                (SongQueryParameter.sort_direction, SortDirection.Descending));
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
                (SongQueryParameter.artist, TestArtist),
                (SongQueryParameter.album_artist, TestArtist),
                (SongQueryParameter.album, TestAlbum),
                (SongQueryParameter.sort_by, SongSortBy.Track),
                (SongQueryParameter.sort_direction, SortDirection.Ascending)
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
            LoadTestSong();
            Assert.IsTrue(TestSong.Additional.Rating.Value == 4);
            Assert.IsTrue((await AudioStation.RateSongAsync(TestSong.ID, 5)).Success);
            LoadTestSong();
            Assert.IsTrue(TestSong.Additional.Rating.Value == 5);
        }

        [TestMethod]
        public async Task Folder_List()
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
        public async Task Artist_GetCover()
        {
            var response = await AudioStation.GetArtistCoverAsync(TestArtist);
            Assert.IsFalse(response is null);
            Assert.IsFalse(response.Data is null);
            Assert.IsTrue(response.Data.Length > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Type));
        }

        [TestMethod]
        public async Task Artist_GetCover_NotFound()
        {
            Exception exception = null;
            try
            {
                var response = await AudioStation.GetArtistCoverAsync("This artist does not even exist 123456789");
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(SynologyDotNet.Core.Exceptions.SynoHttpException));
            var synoHttpException = (SynologyDotNet.Core.Exceptions.SynoHttpException)exception;
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, synoHttpException.StatusCode);
        }

        [TestMethod]
        public async Task Album_GetCover()
        {
            var response = await AudioStation.GetAlbumCoverAsync(TestArtist, TestAlbum);
            Assert.IsFalse(response is null);
            Assert.IsFalse(response.Data is null);
            Assert.IsTrue(response.Data.Length > 5000);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Type));
        }

        [TestMethod]
        public async Task Album_GetCover_NotFound()
        {
            Exception exception = null;
            try
            {
                var response = await AudioStation.GetAlbumCoverAsync("This artist does not even exist 123456789", "This album does not even exist 123456789");
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(SynologyDotNet.Core.Exceptions.SynoHttpException));
            var synoHttpException = (SynologyDotNet.Core.Exceptions.SynoHttpException)exception;
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, synoHttpException.StatusCode);
        }

        [TestMethod]
        public async Task Song_GetById()
        {
            var response = await AudioStation.GetSongByIdAsync(TestSong.ID);
            Assert.IsTrue(response.Data.Songs.Length > 0);
            Assert.AreEqual(response.Data.Songs[0].ID, TestSong.ID);
        }

        [TestMethod]
        public void Song_Stream_MP3_320Kbps()
        {
            Song_StreamInternal(TranscodeMode.MP3_320Kbps, "audio/mpeg");
        }

        [TestMethod]
        public void Song_Stream_WAV()
        {
            Song_StreamInternal(TranscodeMode.WAV, "audio/x-wav");
        }

        [TestMethod]
        public void Song_Stream_Original()
        {
            Song_StreamInternal(TranscodeMode.None, null);
        }

        private void Song_StreamInternal(TranscodeMode transcodeMode, string mediaType)
        {
            var tokenSource = new CancellationTokenSource();
            var downloadTask = AudioStation.StreamSongAsync(
                tokenSource.Token,
                transcodeMode,
                TestSong.ID,
                0,
                streamArgs =>
                {
                    if (!string.IsNullOrEmpty(mediaType))
                        Assert.AreEqual(streamArgs.ContentType, mediaType);
                    Assert.IsTrue(streamArgs.ContentLength > 0);

                    // Read 4KB just to test the download
                    var buffer = new byte[4096];
                    streamArgs.Stream.Read(buffer, 0, buffer.Length);
                    // There must be at least one non-zero byte in a 4KB chunk
                    Assert.IsTrue(buffer.Any(b => b > 0));
                }
            );
            downloadTask.Wait();
        }

        [TestMethod]
        public async Task Search()
        {
            var response = await AudioStation.SearchAsync(TestArtist);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Data.Artists);
            Assert.IsTrue(response.Data.ArtistTotal > 0);

            response = await AudioStation.SearchAsync(TestAlbum);
            Assert.IsNotNull(response.Data.Albums);
            Assert.IsTrue(response.Data.AlbumTotal > 0);

            response = await AudioStation.SearchAsync(Config.TestSongTitle);
            Assert.IsNotNull(response.Data.Songs);
            Assert.IsTrue(response.Data.SongTotal > 0);
        }

        [TestMethod]
        public async Task SearchArtistsByName()
        {
            var response = await AudioStation.SearchArtistsByNameAsync(TestPageSize, 0, TestSong.Additional.Tag.Artist);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Artists.Any(x => x.Name.Contains(TestSong.Additional.Tag.Artist, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task SearchArtistsByGenre()
        {
            var response = await AudioStation.SearchArtistsByGenreAsync(TestPageSize, 0, TestSong.Additional.Tag.Genre);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Artists.Any(x => x.Name.Equals(TestSong.Additional.Tag.Artist, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task SearchAlbumsByName()
        {
            var response = await AudioStation.SearchAlbumsByNameAsync(TestPageSize, 0, TestSong.Additional.Tag.Album);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Albums.Any(x => x.Name.Contains(TestSong.Additional.Tag.Album, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task SearchComposersByName()
        {
            var response = await AudioStation.SearchComposersByNameAsync(TestPageSize, 0, TestSong.Additional.Tag.Composer);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Composers.Any(x => x.Name.Contains(TestSong.Additional.Tag.Composer, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task SearchGenresByNameAsync()
        {
            var response = await AudioStation.SearchGenresByNameAsync(TestPageSize, 0, TestSong.Additional.Tag.Genre);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Genres.Any(x => x.Name.Contains(TestSong.Additional.Tag.Genre, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task SearchSongsByTitleAsync()
        {
            var response = await AudioStation.SearchSongsByTitleAsync(TestPageSize, 0, TestSong.Title, SongQueryAdditional.All);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Songs.Any(x => x.Title.Contains(TestSong.Title, StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task Playlist_CreateAndDelete()
        {
            var createResult = await AudioStation.CreatePlaylistAsync("Playlist_Create_Test1", false);
            Assert.IsTrue(createResult.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(createResult.Data.ID));

            var deleteResult = await AudioStation.DeletePlaylistAsync(createResult.Data.ID);
            Assert.IsTrue(deleteResult.Success);
        }

        [TestMethod]
        public async Task Playlist_List()
        {
            var response = await AudioStation.ListPlaylistsAsync(TestPageSize, 0);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Data.Total > 0);
            Assert.IsTrue(response.Data.Playlists.Length > 0);
        }

        [TestMethod]
        public async Task Playlist_Get()
        {
            var playlist = await AudioStation.GetPlaylistAsync(TestPageSize, 0, TestPlaylist.ID);
            Assert.IsTrue(playlist.Success);
            Assert.IsFalse(string.IsNullOrEmpty(playlist.Data.ID));
            Assert.IsTrue(playlist.Data.Additional.Songs.Length > 0);
            foreach (var song in playlist.Data.Additional.Songs)
            {
                AssertSong(song, SongQueryAdditional.None);
            }
        }

        [TestMethod]
        public async Task Playlist_GetWithSongDetails()
        {
            var playlist = await AudioStation.GetPlaylistAsync(TestPageSize, 0, TestPlaylist.ID, SongQueryAdditional.All);
            Assert.IsTrue(playlist.Success);
            Assert.IsFalse(string.IsNullOrEmpty(playlist.Data.ID));
            Assert.IsTrue(playlist.Data.Additional.Songs.Length > 0);
            foreach (var song in playlist.Data.Additional.Songs)
            {
                AssertSong(song, SongQueryAdditional.All);
            }
        }

        [TestMethod]
        public async Task Playlist_AddAndRemoveSong()
        {
            // Add test song to playlist
            var response = await AudioStation.AddSongsToPlaylist(TestPlaylist.ID, TestSong.ID);
            Assert.IsTrue(response.Success);

            // Get all songs on playlist
            var playlist = await AudioStation.GetPlaylistAsync(TestPageSize, 0, TestPlaylist.ID);
            Assert.IsTrue(playlist.Success);
            Assert.IsTrue(playlist.Data.Additional.Songs.Any(x => x.ID == TestSong.ID));

            // Remove the test song from the playlist
            var testSongPlaylistIndex = playlist.Data.Additional.Songs.Select((song, index) => (song, index)).First(x => x.song.ID == TestSong.ID).index;
            response = await AudioStation.RemoveSongsFromPlaylist(TestPlaylist.ID, testSongPlaylistIndex, 1);

            // Check
            playlist = await AudioStation.GetPlaylistAsync(TestPageSize, 0, TestPlaylist.ID);
            Assert.IsTrue(playlist.Success);
            Assert.IsFalse(playlist.Data.Additional.Songs.Any(x => x.ID == TestSong.ID));
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

        // This is tested manually
        //[TestMethod]
        //public async Task TestReIndex()
        //{
        //    // Start reindexing
        //    var startReIndexResponse = await AudioStation.StartReIndex();
        //    Assert.IsTrue(startReIndexResponse.Success);

        //    // Check reindexing state
        //    Thread.Sleep(100);
        //    var getReIndexStateResponse = await AudioStation.GetReIndexState();
        //    Assert.IsTrue(getReIndexStateResponse.Success);
        //    Assert.IsTrue(getReIndexStateResponse.ReIndexing);
        //}

        #region Private Methods
        private static void AssertSong(Song song, SongQueryAdditional additional)
        {
            Assert.IsNotNull(song);
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
