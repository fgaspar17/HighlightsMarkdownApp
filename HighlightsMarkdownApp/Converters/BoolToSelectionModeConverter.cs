using Microsoft.UI.Xaml.Data;

namespace HighlightsMarkdownApp.Converters;

public class BoolToSelectionModeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => (bool)value ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
