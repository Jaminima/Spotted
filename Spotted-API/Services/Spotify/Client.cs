using SpotifyAPI.Web;

namespace Spotted_API.Services.Spotify
{
    public class Client
    {
        private string accessToken, refreshToken;
        public DateTime _lastUpdate
        {
            get; private set;
        }
        private SpotifyClient _spotifyClient;
        public PlayBackState _currentPlayBackState
        {
            get; private set; 
        }
        public CurrentlyPlayingContext _currentlyPlayingContext
        {
            get; private set;
        }

        public FullTrack? currentTrack
        {
            get { return _currentlyPlayingContext != null ? (FullTrack)_currentlyPlayingContext.Item : null; }
        }

        public SpotifyClient spotifyClient
        {
            get
            {
                if (_spotifyClient == null)
                {
                    _spotifyClient = new SpotifyClient(accessToken);
                }
                return _spotifyClient;
            }
        }

        public Client(string accessToken, string refreshToken)
        {
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
        }

        public async Task<PlayBackState> CheckPlayState()
        {
            DateTime now = DateTime.Now;
            PlayBackState playBackState = new PlayBackState();
            var playback = await spotifyClient.Player.GetCurrentPlayback();

            if (_currentlyPlayingContext!= null && playback.Item != null)
            {
                if (_currentlyPlayingContext.Item !=null && ((FullTrack)playback.Item).Id != ((FullTrack)_currentlyPlayingContext.Item).Id)
                {
                    playBackState.songChange = SongChange.Skipped;
                }
                else if (_currentlyPlayingContext.IsPlaying && playback.IsPlaying)
                {
                    var timePassed = now - _lastUpdate;

                    int msProgressed = playback.ProgressMs - _currentlyPlayingContext.ProgressMs;

                    if (timePassed.TotalMilliseconds >= msProgressed * 1.05)
                    {
                        playBackState.songChange = playback.ProgressMs <= timePassed.TotalMilliseconds ? SongChange.Replayed : SongChange.Rewound;
                    }
                    else if (timePassed.TotalMilliseconds <= msProgressed * 0.95)
                    {
                        playBackState.songChange = SongChange.Fastforward;
                    }
                }
            }

            if (_currentlyPlayingContext!=null && playback.IsPlaying != _currentlyPlayingContext.IsPlaying)
            {
                playBackState.playingState = playback.IsPlaying ? PlayingState.Started : PlayingState.Stopped;
            }
            else
            {
                playBackState.playingState = playback.IsPlaying ? PlayingState.Playing : PlayingState.Haulted;
            }

            playBackState.isDifferentToPreviousState = _currentPlayBackState==null || playBackState.playingState != _currentPlayBackState.playingState || (playBackState.songChange != _currentPlayBackState.songChange && playBackState.songChange != SongChange.NoChange);

            _currentlyPlayingContext = playback;
            _currentPlayBackState = playBackState;
            _lastUpdate = now;

            return playBackState;
        }
    }
}
