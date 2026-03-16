using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace HighlightsMarkdownApp.Services;

public class FolderPickerService
{
    public async Task<string?> PickFolderAsync()
    {
        var picker = new FolderPicker();

        // Necesario en WinUI / Uno Desktop
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeFilter.Add("*");

        StorageFolder folder = await picker.PickSingleFolderAsync();

        return folder?.Path;
    }
}
