using Microsoft.Extensions.Options;

namespace HighlightsMarkdown.Core.OAuth;

public class OAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly IOAuthSigner _signer;
    private readonly OAuthOptions _options;

    public OAuthClient(HttpClient httpClient, IOAuthSigner signer, IOptions<OAuthOptions> options)
    {
        _httpClient = httpClient;
        _signer = signer;
        _options = options.Value;
    }

    public async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        IEnumerable<KeyValuePair<string, string>>? bodyParams,
        string? token,
        string? tokenSecret,
        CancellationToken ct)
    {
        var bodyList = bodyParams?.ToList();
        HttpRequestMessage request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = _signer.CreateAuthorizationHeader(method, url, _options, token, tokenSecret, bodyList);


        if (bodyParams != null)
            request.Content = new FormUrlEncodedContent(bodyParams);

        var response = await _httpClient.SendAsync(request, ct);
        return response;
    }
}
