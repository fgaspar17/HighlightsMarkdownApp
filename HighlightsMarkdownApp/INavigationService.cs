namespace HighlightsMarkdownApp;

internal interface INavigationService
{
    void Navigate<TPage>() where TPage : Page;
}
