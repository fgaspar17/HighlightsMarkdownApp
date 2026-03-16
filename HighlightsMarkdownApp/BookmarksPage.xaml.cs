using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using HighlightsMarkdownApp.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HighlightsMarkdownApp;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class BookmarksPage : Page
{
    private BookmarksViewModel ViewModel =>
        (BookmarksViewModel)DataContext;

    public BookmarksPage()
    {
        this.InitializeComponent();

        DataContext = App.Host!
            .Services
            .GetRequiredService<BookmarksViewModel>();

        this.Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitializeAsync();
    }

    private async void OnRefreshRequested(
    RefreshContainer sender,
    RefreshRequestedEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            if (DataContext is BookmarksViewModel vm)
            {
                await vm.InitializeAsync(); // or LoadBookmarks()
            }
        }
        finally
        {
            deferral.Complete();
        }
    }

    private async void OnUrlClicked(object sender, RoutedEventArgs e)
    {
        if (sender is HyperlinkButton btn &&
            btn.DataContext is Bookmark bookmark)
        {
            await ViewModel.OpenBookmarkCommand.ExecuteAsync(bookmark);
        }
    }
}
