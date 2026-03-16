namespace HighlightsMarkdown.Core.OAuth.Security;

public interface ITokenStore
{
    void Save(string token, string secret);
    (string Token, string Secret)? Get();
    void Clear();
}
