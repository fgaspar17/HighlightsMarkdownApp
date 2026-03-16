using System.Net.Http.Headers;

namespace HighlightsMarkdown.Core.OAuth;

public interface IOAuthSigner
{
    AuthenticationHeaderValue CreateAuthorizationHeader(HttpMethod method, string url, OAuthOptions options, string? token, string? tokenSecret, IEnumerable<KeyValuePair<string, string>>? bodyParams);
}
