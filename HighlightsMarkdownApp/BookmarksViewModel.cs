using System.Collections.ObjectModel;
using System.Text.Json;
using HighlightsMarkdown.Core.OAuth;
using HighlightsMarkdown.Core.OAuth.Security;
using HighlightsMarkdownApp.Models;
using HighlightsMarkdownApp.Services;
using Windows.Storage.Pickers;
using Windows.System;
using WinRT.Interop;

namespace HighlightsMarkdownApp;

public partial class BookmarksViewModel : ObservableObject
{
    public ObservableCollection<Bookmark> FilteredBookmarks { get; } = new();

    private List<Bookmark> _allBookmarks = new();

    private CancellationTokenSource? _searchCts;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private bool _isExporting;

    [ObservableProperty]
    private double _exportProgress;

    [ObservableProperty]
    private string _exportStatus = "";

    public bool ShowEmptyState =>
    !IsLoading && FilteredBookmarks.Count == 0;

    private readonly OAuthClient _oauthClient;
    private readonly ITokenStore _tokenStore;

    private readonly MarkdownExportService _mdExportService;
    private readonly FolderPickerService _folderPickerService;
    private readonly IDialogService _dialogService;

    public BookmarksViewModel(
        OAuthClient oauthClient,
        ITokenStore tokenStore,
        MarkdownExportService mdExportService,
        FolderPickerService folderPickerService,
        IDialogService dialogService)
    {
        _oauthClient = oauthClient;
        _tokenStore = tokenStore;

        FilteredBookmarks.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(ShowEmptyState));
        };

        _mdExportService = mdExportService;
        _folderPickerService = folderPickerService;
        _dialogService = dialogService;
    }


    [RelayCommand]
    private async Task OpenBookmark(Bookmark bookmark)
    {
        await Launcher.LaunchUriAsync(new Uri(bookmark.Url));
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadBookmarks();
    }


    public async Task InitializeAsync()
    {
        await Refresh();
    }

    private async Task LoadBookmarks()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            var bookmarks = new List<Bookmark>();

            var stored = _tokenStore.Get();
            if (stored == null)
            {
                ErrorMessage = "You're not authenticated in.";
                return;
            }

            var response = await _oauthClient.SendAsync(
                HttpMethod.Get,
                "https://www.instapaper.com/api/1.1/bookmarks/list",
                bodyParams: null,
                token: stored.Value.Token,
                tokenSecret: stored.Value.Secret,
                CancellationToken.None);

            var content = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(content);


            Dictionary<long, Bookmark> bookmarksDict = new Dictionary<long, Bookmark>();
            foreach (var element in doc.RootElement.GetProperty("bookmarks").EnumerateArray())
            {
                if (element.GetProperty("type").GetString() == "bookmark")
                {
                    var bookmark = element.Deserialize<Bookmark>();
                    bookmarks.Add(bookmark!);
                    bookmarksDict.Add(bookmark!.BookmarkId, bookmark);
                }
            }

            foreach (var element in doc.RootElement.GetProperty("highlights").EnumerateArray())
            {
                if (element.GetProperty("type").GetString() == "highlight")
                {
                    var highlight = element.Deserialize<Highlight>();
                    bookmarksDict[highlight!.BookmarkId].Highlights.Add(highlight);
                }
            }

            _allBookmarks = bookmarks.OrderByDescending(b => b.Starred).ThenByDescending(b => b.Time).ToList();
            FilterBookmarks();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load bookmarks.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string? value)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();

        _ = Task.Delay(300, _searchCts.Token)
        .ContinueWith(t =>
        {
            if (!t.IsCanceled)
                FilterBookmarks();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void FilterBookmarks()
    {
        FilteredBookmarks.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var item in _allBookmarks)
                FilteredBookmarks.Add(item);

            return;
        }

        var filtered = _allBookmarks
            .Where(b =>
                b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                b.Url.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        foreach (var item in filtered)
            FilteredBookmarks.Add(item);
    }

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var bm in _allBookmarks)
        {
            bm.IsChecked = true;
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        foreach (var bm in _allBookmarks)
        {
            bm.IsChecked = false;
        }
    }


    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task ExportSelected(Window window)
    {
        var checkedBookmarks = _allBookmarks.Where(b => b.IsChecked);
        string? folder = await _folderPickerService.PickFolderAsync();
        if (!string.IsNullOrEmpty(folder))
        {
            IsExporting = true;
            ExportProgress = 0;
            ExportStatus = "Starting export...";

            var progress = new Progress<double>(value =>
            {
                ExportProgress = value;
                ExportStatus = $"Exporting... {Math.Round(value)}%";
            });

            await _mdExportService.ExportAsync(checkedBookmarks, folder, progress);
            await _dialogService.ShowMessageAsync("Notification", $"Successfully exported {checkedBookmarks.Count()} bookmarks.");
        }

        ExportStatus = "Export completed!";
        IsExporting = false;
    }

    private bool CanExport()
    {
        return true;
        return _allBookmarks.Any(bm => bm.IsChecked);
    }

    partial void OnIsLoadingChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowEmptyState));
    }
}
