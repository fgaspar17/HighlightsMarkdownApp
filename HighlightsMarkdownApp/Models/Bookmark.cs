using System.Text.Json.Serialization;

namespace HighlightsMarkdownApp.Models;

public class Bookmark : ObservableObject
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("bookmark_id")]
    public long BookmarkId { get; set; }
    [JsonPropertyName("time")]
    public long Time { get; set; }
    [JsonPropertyName("starred")]
    public string? Starred { get; set; }
    public bool IsStarred { get => Starred == "1"; }
    public List<Highlight> Highlights { get; set; } = new();
    public string HighlightsCountText => $"{Highlights?.Count ?? 0} highlights";
    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }
}
