using System;
using System.Collections.Generic;
using System.Text;
using HighlightsMarkdown.Core.OAuth.Security;
using Windows.Security.Credentials;

namespace HighlightsMarkdownApp.Security;

internal class TokenStore : ITokenStore
{
    private const string ResourceName = "InstapaperOAuth";

    public void Save(string token, string tokenSecret)
    {
        var vault = new PasswordVault();

        // Combine secret + token into password
        var credential = new PasswordCredential(
            ResourceName,
            token,
            tokenSecret);

        vault.Add(credential);
    }

    public (string Token, string Secret)? Get()
    {
        var vault = new PasswordVault();

        try
        {
            var credential = vault.RetrieveAll()
                .FirstOrDefault(c => c.Resource == ResourceName);

            if (credential == null)
                return null;

            credential.RetrievePassword();

            return (credential.UserName, credential.Password);
        }
        catch
        {
            return null;
        }
    }

    public void Clear()
    {
        var vault = new PasswordVault();

        foreach (var credential in vault.RetrieveAll()
                     .Where(c => c.Resource == ResourceName))
        {
            vault.Remove(credential);
        }
    }
}
