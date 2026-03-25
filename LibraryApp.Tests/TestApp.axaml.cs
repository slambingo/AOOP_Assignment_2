using Avalonia;
using Avalonia.Markup.Xaml;

namespace LibraryApp.Tests;

public class TestApp : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
