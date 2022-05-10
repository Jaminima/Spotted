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

        public ClientRegistration(Services.Spotify.ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        // GET: ClientRegistration
        [HttpGet("callback")]
        public async Task<ActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://accounts.spotify.com/api/token"))
                {
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Configuration.client_id}:{Configuration.client_secret}")));
                    request.Content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "code", code }, { "redirect_uri", Configuration.callback_url }, { "grant_type", "authorization_code" } });
                    var response = await httpClient.SendAsync(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                        string content = await response.Content.ReadAsStringAsync();
                        var json = JsonSerializer.Deserialize<JsonElement>(content);

                        if (json.TryGetProperty("access_token", out JsonElement access) && json.TryGetProperty("refresh_token", out JsonElement refresh))
                        {
                            var userName = await _clientManager.RegisterClient(access.GetString(), refresh.GetString());

                            if (userName?.Length > 0) return Redirect("/success");
                        }
                        return Redirect("/fail");
                    }
                    catch (HttpRequestException e)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(error);
                        return Redirect("error");
                    }
                    catch (JsonException e)
                    {
                        return Redirect("error");
                    }
                }
            }
        }

        [HttpGet("grant")]
        public ActionResult Grant()
        {
            return Redirect("https://accounts.spotify.com/authorize?response_type=code&client_id=" + Configuration.client_id + "&scope="+Configuration.grant_scope+"&redirect_uri=" + Configuration.callback_url + "&state=");
        }
    }
}
