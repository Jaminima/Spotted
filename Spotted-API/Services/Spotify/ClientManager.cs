using System.Collections.Concurrent;
using SpotifyAPI.Web;
namespace Spotted_API.Services.Spotify
{
    public class ClientManager
    {
        public ConcurrentDictionary<string,Client> clients = new ConcurrentDictionary<string, Client>();

        public async Task<string?> RegisterClient(string accessToken, string refreshToken)
        {
            var cli = new Client(accessToken, refreshToken);
            PrivateUser user;
            try
            {
                user = await cli.spotifyClient.UserProfile.Current();
            }
            catch (APIUnauthorizedException e)
            {
                return null;
            }

            if (clients.TryAdd(user.DisplayName, cli)) { 
                return user.DisplayName;
            }
            return null;
        }
    }
}
