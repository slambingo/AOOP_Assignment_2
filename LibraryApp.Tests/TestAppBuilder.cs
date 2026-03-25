using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(LibraryApp.Tests.TestAppBuilder))]

namespace LibraryApp.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
