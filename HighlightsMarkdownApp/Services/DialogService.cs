namespace HighlightsMarkdownApp.Services;

internal class DialogService : IDialogService
{
    private readonly Frame _frame;

    public DialogService(Frame frame)
    {
        _frame = frame;
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = _frame.XamlRoot
        };

        await dialog.ShowAsync();
    }
}
