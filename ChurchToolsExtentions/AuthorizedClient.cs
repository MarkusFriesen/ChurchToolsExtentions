using ChurchToolsExtentions.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ChurchToolsExtentions;

public abstract class AuthorizedClient
{
    internal readonly HttpClient Client;
    private readonly string _churchtoolsInstance;
    private readonly string _username;
    private readonly string _password;
    private DateTimeOffset TokenExpires = DateTimeOffset.UtcNow;

    private bool ReLogin
    {
        get
        {
            return TokenExpires <= DateTimeOffset.UtcNow.AddSeconds(15);
        }
    }

    public AuthorizedClient(IOptions<ConnectionSettings> settings)
    {
        var handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };

        _churchtoolsInstance = settings.Value.Instance;
        _username = settings.Value.Username;
        _password = settings.Value.Password;

        Client = new HttpClient(handler)
        {
            BaseAddress = new Uri($"https://{_churchtoolsInstance}.church.tools/api/")
        };
    }


    public async Task GetLatestCookie()
    {
        if (!ReLogin) return;

        var content = JsonSerializer.Serialize(new
        {
            password = _password,
            rememberMe = true,
            username = _username
        });
        var result = await Client.PostAsync("login", new StringContent(content, Encoding.UTF8, "application/json"));
        var success = result.EnsureSuccessStatusCode();

        var cookies = result.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value;
        var longestCookie = GetLongestCookie(cookies);
        TokenExpires = longestCookie.Item1;

        if (Client.DefaultRequestHeaders.Contains("Cookie"))
        {
            Client.DefaultRequestHeaders.Remove("Cookie");
        }
        Client.DefaultRequestHeaders.Add("Cookie", longestCookie.Item2);
    }

    private static (DateTimeOffset, string) GetLongestCookie(IEnumerable<string> cookies)
    {
        var result = DateTimeOffset.UtcNow;
        var resultCookie = string.Empty;
        foreach (var cookie in cookies ?? Array.Empty<string>())
        {
            var expires = cookie.Split(";")
                .FirstOrDefault(c => c.Trim()
                    .StartsWith("expires"))?
                    .Split("=")?
                    .LastOrDefault();
            if (expires == null) continue;
            if (!DateTimeOffset.TryParse(expires, out var newDate) || newDate < result) continue;

            result = newDate;
            resultCookie = cookie;
        }

        return (result, resultCookie);
    }
}

