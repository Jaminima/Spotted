using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using System.Text;
using System.Text.Json;

namespace Spotted_API.Controllers
{
    [Route("/api/spotify/auth")]
    public class ClientRegistration : Controller
    {
        private Services.Spotify.ClientManager _clientManager;
        private Services.SessionManager _sessionManager;

        public ClientRegistration(Services.Spotify.ClientManager clientManager, Services.SessionManager sessionManager)
        {
            _clientManager = clientManager;
            _sessionManager = sessionManager;
        }

        // GET: ClientRegistration
        [HttpGet("callback")]
        public async Task<ActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var token = await Services.Spotify.TokenManager.TokenFromCodeGrant(code);
            if (token != null)
            {
                var cli = await _clientManager.RegisterClient(token.accessToken, token.refreshToken);

                if (cli!=null)
                {
                    if (_sessionManager.CreateSession(cli, out var grant))
                    {
                        Response.Cookies.Append("id", grant.id.ToString());
                        Response.Cookies.Append("key", grant.key);
                        Response.Cookies.Append("displayName", grant.displayName);
                        return Ok(grant);
                    }
                    else
                    {
                        return Problem("Session Initialisation Failed", statusCode: 500);
                    }
                }
                else
                {
                    return Problem("Client Initialisation Failed", statusCode: 500);
                }
            }
            return Problem("Token Grant From Code Failed", statusCode: 403);
        }

        [HttpGet("grant")]
        public ActionResult Grant()
        {
            return Redirect("https://accounts.spotify.com/authorize?response_type=code&client_id=" + Configuration.client_id + "&scope="+Configuration.grant_scope+"&redirect_uri=" + Configuration.callback_url + "&state=");
        }
    }
}
