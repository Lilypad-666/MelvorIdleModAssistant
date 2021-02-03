using Avalonia;
using Avalonia.ReactiveUI;
using MelvorIdleModAssistant.ViewModels;
using MelvorIdleModAssistant.Views;
using ReactiveUI;
using Splat;

namespace MelvorIdleModAssistant {
    class Program {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) {

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp() {
            Locator.CurrentMutable.Register(() => new InfoView(), typeof(IViewFor<InfoViewModel>));
            Locator.CurrentMutable.Register(() => new ModsView(), typeof(IViewFor<ModsViewModel>));
            Locator.CurrentMutable.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));

            return AppBuilder.Configure<App>()
              .UsePlatformDetect()
              .LogToTrace()
              .UseReactiveUI();

        }
    }
}
