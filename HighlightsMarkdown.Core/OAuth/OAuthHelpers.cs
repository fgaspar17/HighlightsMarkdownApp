using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace HighlightsMarkdown.Core.OAuth;

public class OAuthHelpers
{

    public async static Task SendOAuth(string url, string consumerKey, string consumerSecret, string tokenSecret)
    {
        Dictionary<string, string> oAuthParameters = new Dictionary<string, string>()
        {
            {"oauth_timestamp", GenerateTimeStamp() },
            {"oauth_signature_method", "HMAC-SHA1" },
            {"oauth_nonce", GenerateNonce() },
            {"oauth_version", "1.0" },
            { "oauth_consumer_key", consumerKey }
        };

        var formData = new List<KeyValuePair<string, string>>();
        formData.Add(new KeyValuePair<string, string>("x_auth_username", "nectryk@gmail.com"));
        formData.Add(new KeyValuePair<string, string>("x_auth_password", "el_imperio_final"));
        formData.Add(new KeyValuePair<string, string>("x_auth_mode", "client_auth"));

        string signatureBase = GenerateSignatureBase(HttpMethod.Post, url, oAuthParameters.Concat(formData).ToDictionary());
        string signature = GenerateSignature(consumerSecret, tokenSecret, signatureBase);
        oAuthParameters.Add("oauth_signature", signature);

        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", string.Join(",", oAuthParameters.Select(p => p.Key + "=" + p.Value)));


        request.Content = new FormUrlEncodedContent(formData);

        var response = await client.SendAsync(request, CancellationToken.None);
        if (response.IsSuccessStatusCode)
            Console.WriteLine(response.Content);
    }
    internal static string GenerateNonce() => Guid.NewGuid().ToString("N");
    internal static string GenerateTimeStamp()
    {
        TimeSpan diffFrom1970 = DateTime.UtcNow - new DateTime(1970, 1, 1);
        return ((long)diffFrom1970.TotalSeconds).ToString();
    }
    internal static string GenerateSignatureBase(HttpMethod method, string url, Dictionary<string, string> oAuthParameters)
    {
        string methodStr = method.ToString();
        string urlNormalized = NormalizeUrl(url);
        string oAuthParametersNormalized = Uri.EscapeDataString(string.Join("&", oAuthParameters.OrderBy(o => o.Key).ThenBy(o => o.Value).Select(o => Uri.EscapeDataString(o.Key) + "=" + Uri.EscapeDataString(o.Value))));

        return methodStr + "&" + Uri.EscapeDataString(urlNormalized) + "&" + oAuthParametersNormalized;
    }
    internal static string GenerateSignature(string consumerSecret, string tokenSecret, string signaturaBase)
    {
        HMACSHA1 hMACSHA1 = new HMACSHA1(Encoding.UTF8.GetBytes(Uri.EscapeDataString(consumerSecret) + "&" + Uri.EscapeDataString(tokenSecret)));

        byte[] hash = hMACSHA1.ComputeHash(Encoding.UTF8.GetBytes(signaturaBase));

        return Convert.ToBase64String(hash);
    }

    internal static string NormalizeUrl(string url)
    {
        Uri uri = new Uri(url);

        string normalizedUrl = $"{uri.Scheme}://{uri.Host}";

        if (!uri.IsDefaultPort)
            normalizedUrl += $"{normalizedUrl}:{uri.Port}";

        normalizedUrl += $"{uri.AbsolutePath}";

        return normalizedUrl;
    }
}
