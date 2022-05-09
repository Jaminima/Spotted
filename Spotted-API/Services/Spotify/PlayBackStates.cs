namespace Spotted_API.Services.Spotify
{
    public enum SongChange
    {
        NoChange,
        Skipped,
        Fastforward,
        Rewound,
        Replayed
    }
    public enum PlayingState
    {
        Playing,
        Haulted,
        Started,
        Stopped
    }

    public class PlayBackState
    {
        public SongChange songChange = SongChange.NoChange;
        public PlayingState playingState = PlayingState.Haulted;

        public bool isDifferentToPreviousState = false;
    }
}
