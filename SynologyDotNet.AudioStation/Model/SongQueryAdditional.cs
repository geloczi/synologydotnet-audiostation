using System;

namespace SynologyDotNet.AudioStation.Model
{
    [Flags]
    public enum SongQueryAdditional : short
    {
        None = 0,
        song_tag = 1,
        song_audio = 2,
        song_rating = 4,
        All = ~None
    }
}
