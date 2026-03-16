using Microsoft.UI.Xaml.Data;

namespace HighlightsMarkdownApp.Converters;

public class UnixToDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is long unixTime)
        {
            var date = DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
            var span = DateTimeOffset.Now - date;

            if (span.TotalDays >= 1)
                return $"{(int)span.TotalDays} days ago";

            if (span.TotalHours >= 1)
                return $"{(int)span.TotalHours} hours ago";

            if (span.TotalMinutes >= 1)
                return $"{(int)span.TotalMinutes} minutes ago";

            return "Just now";
        }

        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
