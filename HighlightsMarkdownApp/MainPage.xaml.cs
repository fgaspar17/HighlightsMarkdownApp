using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;

namespace HighlightsMarkdownApp;

public sealed partial class MainPage : Page
{
    private MainViewModel ViewModel =>
        (MainViewModel)DataContext;
    public MainPage()
    {
        this.InitializeComponent();

        DataContext = App.Host!
        .Services
        .GetRequiredService<MainViewModel>();

        SignInButton.PointerEntered += OnPointerEntered;
        SignInButton.PointerExited += OnPointerExited;

        this.Loaded += OnLoaded;
    }


    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        var inputCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
        ProtectedCursor = inputCursor;
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        var inputCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
        ProtectedCursor = inputCursor;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Initialize();
    }
}
