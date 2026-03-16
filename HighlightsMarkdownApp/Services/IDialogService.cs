namespace HighlightsMarkdownApp.Services;

public interface IDialogService
{
    Task ShowMessageAsync(string title, string message);
}