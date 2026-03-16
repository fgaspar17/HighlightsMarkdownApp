using System.Text;
using HighlightsMarkdownApp.Models;

namespace HighlightsMarkdownApp.Services;

public class MarkdownExportService
{
    public async Task ExportAsync(IEnumerable<Bookmark> bookmarks, string folder, 
        IProgress<double>? progress = null)
    {
        Directory.CreateDirectory(folder);
        int total = bookmarks.Count();
        int current = 0;

        foreach (var bookmark in bookmarks)
        {
            string fileName = $"{bookmark.Title}_{bookmark.BookmarkId}.md";
            string fileNameSanitized = SanitizeFileName(fileName);
            string filePath = Path.Combine(folder, fileNameSanitized);

            await using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

            await writer.WriteLineAsync($"# {bookmark.Title}");
            await writer.WriteLineAsync();

            foreach (var hl in bookmark.Highlights ?? Enumerable.Empty<Highlight>())
            {
                await writer.WriteLineAsync($"> {hl.Text}");
                await writer.WriteLineAsync();
            }

            current++;
            progress?.Report((double)current / total * 100);
        }
    }

    private string SanitizeFileName(string fileName)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar, '_');
        }

        return fileName;
    }
}
