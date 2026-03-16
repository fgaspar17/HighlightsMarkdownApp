using System.Text.Json.Serialization;

namespace HighlightsMarkdownApp.Models;

public class Highlight
{
    [JsonPropertyName("highlight_id")]
    public long HighlightId { get; set; }
    [JsonPropertyName("bookmark_id")]
    public long BookmarkId { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
