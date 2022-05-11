using System.Text;
using System.Text.Json;

namespace Spotted_API.Services.Spotify
{
    public class Token
    {
        public string? accessToken, refreshToken;
    }

    public static class TokenManager
    {
        public static async Task<Token?> TokenFromCodeGrant(string code)
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
                            return new Token() { accessToken= access.GetString(), refreshToken=refresh.GetString() };
                        }
                        return null;
                    }
                    catch (HttpRequestException e)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(error);
                        return null;
                    }
                    catch (JsonException e)
                    {
                        return null;
                    }
                }
            }
        }
    }
}
