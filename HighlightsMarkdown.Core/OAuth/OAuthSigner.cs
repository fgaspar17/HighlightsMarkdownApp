using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace HighlightsMarkdown.Core.OAuth;

// TODO: Extract query params from the URL
public class OAuthSigner : IOAuthSigner
{
    public AuthenticationHeaderValue CreateAuthorizationHeader(
        HttpMethod method,
        string url,
        OAuthOptions oAuthOptions,
        string? token,
        string? tokenSecret,
        IEnumerable<KeyValuePair<string, string>>? bodyParams)
    {
        var oauthParams = new List<KeyValuePair<string, string>>
        {
            new("oauth_consumer_key", oAuthOptions.ConsumerKey),
            new("oauth_nonce", GenerateNonce()),
            new("oauth_timestamp", GenerateTimeStamp()),
            new("oauth_signature_method", "HMAC-SHA1"),
            new("oauth_version", "1.0")
        };

        if (!string.IsNullOrEmpty(token))
            oauthParams.Add(new("oauth_token", token));


        string signatureBase = GenerateSignatureBase(method, url, oauthParams, bodyParams);
        string signature = GenerateSignature(oAuthOptions.ConsumerSecret, tokenSecret, signatureBase);
        var headerParams = oauthParams
            .Append(new KeyValuePair<string, string>("oauth_signature", signature));

        return new AuthenticationHeaderValue("OAuth", string.Join(",", headerParams.Select(p => p.Key + "=" + p.Value)));
    }

    private string GenerateNonce() => Guid.NewGuid().ToString("N");

    private string GenerateTimeStamp()
    {
        TimeSpan diffFrom1970 = DateTime.UtcNow - new DateTime(1970, 1, 1);
        return ((long)diffFrom1970.TotalSeconds).ToString();
    }

    private string GenerateSignatureBase(HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>> oAuthParameters, IEnumerable<KeyValuePair<string, string>>? bodyParams)
    {
        string methodStr = method.ToString();

        string urlNormalized = NormalizeUrl(url);

        IEnumerable<KeyValuePair<string, string>> parameters = oAuthParameters;
        if (bodyParams != null)
        {
            parameters = oAuthParameters.Concat(bodyParams);
        }

        parameters = parameters.OrderBy(o => o.Key).ThenBy(o => o.Value);

        string oAuthParametersNormalized = Uri.EscapeDataString(string.Join("&", parameters.Select(o => Uri.EscapeDataString(o.Key) + "=" + Uri.EscapeDataString(o.Value))));

        return methodStr + "&" + Uri.EscapeDataString(urlNormalized) + "&" + oAuthParametersNormalized;
    }

    private string GenerateSignature(string consumerSecret, string? tokenSecret, string signaturaBase)
    {
        HMACSHA1 hMACSHA1 = new HMACSHA1(Encoding.UTF8.GetBytes(Uri.EscapeDataString(consumerSecret) + "&" + Uri.EscapeDataString(tokenSecret ?? string.Empty)));

        byte[] hash = hMACSHA1.ComputeHash(Encoding.UTF8.GetBytes(signaturaBase));

        return Convert.ToBase64String(hash);
    }

    private string NormalizeUrl(string url)
    {
        Uri uri = new Uri(url);

        string normalizedUrl = $"{uri.Scheme}://{uri.Host}";

        if (!uri.IsDefaultPort)
            normalizedUrl += $":{uri.Port}";

        normalizedUrl += $"{uri.AbsolutePath}";

        return normalizedUrl;
    }
}
