using System;
using System.Collections.Generic;
using System.Text;

namespace HighlightsMarkdownApp;

internal class NavigationService : INavigationService
{
    private readonly Frame _frame;

    public NavigationService(Frame frame)
    {
        _frame = frame;
    }

    public void Navigate<TPage>() where TPage : Page
    {
        _frame.Navigate(typeof(TPage));
    }
}
