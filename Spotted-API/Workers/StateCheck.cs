namespace Spotted_API.Workers
{
    public class StateCheck : BackgroundService
    {
        private Services.Spotify.ClientManager _clientManager;

        public StateCheck(Services.Spotify.ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var cli in _clientManager.clients)
                {
                    var playState = await cli.Value.CheckPlayState();

                    if (playState.isDifferentToPreviousState)
                    {
                        Console.WriteLine($"{cli.Key} - {playState.playingState.ToString()} - {playState.songChange.ToString()} > {cli.Value.currentTrack?.Name}");
                    }
                }
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
