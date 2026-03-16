using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using HighlightsMarkdown.Core.OAuth;
using HighlightsMarkdown.Core.OAuth.Security;
using HighlightsMarkdownApp.Models;

namespace HighlightsMarkdownApp;

internal partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    private readonly OAuthClient _oauthClient;

    private readonly ITokenStore _tokenStore;
    private readonly INavigationService _navigationService;

    private const string AccessTokenUrl =
        "https://www.instapaper.com/api/1/oauth/access_token";

    public MainViewModel(OAuthClient oauthClient, ITokenStore tokenStore, INavigationService navigationService)
    {
        _oauthClient = oauthClient;
        _tokenStore = tokenStore;
        _navigationService = navigationService;
    }

    public void Initialize()
    {
        var stored = _tokenStore.Get();
        if (stored != null)
        {
            _navigationService.Navigate<BookmarksPage>();
        }
    }

    [RelayCommand]
    private async Task Login()
    {
        Debug.WriteLine($"Email: {_email}");
        List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("x_auth_username", _email),
            new KeyValuePair<string, string>("x_auth_password", _password),
            new KeyValuePair<string, string>("x_auth_mode", "client_auth"),
        };

        var response = await _oauthClient.SendAsync(HttpMethod.Post, AccessTokenUrl, parameters, token: null, tokenSecret: null, CancellationToken.None);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"Login failed: {content}");
            return;
        }

        Debug.WriteLine($"Success: {content}");

        var values = ParseForm(content);

        string token = values["oauth_token"];
        string tokenSecret = values["oauth_token_secret"];

        _tokenStore.Save(token, tokenSecret);

        _password = string.Empty;

        _navigationService.Navigate<BookmarksPage>();

    }

    private static Dictionary<string, string> ParseForm(string input)
    {
        return input
            .Split('&')
            .Select(p => p.Split('='))
            .ToDictionary(
                p => Uri.UnescapeDataString(p[0]),
                p => Uri.UnescapeDataString(p[1]));
    }
}
