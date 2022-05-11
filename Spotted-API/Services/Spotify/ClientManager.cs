using System.Collections.Concurrent;
using SpotifyAPI.Web;
namespace Spotted_API.Services.Spotify
{
    public class ClientManager
    {
        public ConcurrentDictionary<string,Client> clients = new ConcurrentDictionary<string, Client>();

        public async Task<Client> RegisterClient(string accessToken, string refreshToken)
        {
            var cli = new Client(accessToken, refreshToken);
            PrivateUser user;
            try
            {
                user = await cli.spotifyClient.UserProfile.Current();
                cli.currentUser = user;
            }
            catch (APIUnauthorizedException e)
            {
                return null;
            }

            clients.Remove(user.DisplayName, out _);
            if (clients.TryAdd(user.DisplayName, cli)) { 
                return cli;
            }

            return null;
        }
    }
}
