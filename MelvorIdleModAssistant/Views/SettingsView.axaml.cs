using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MelvorIdleModAssistant.ViewModels;

namespace MelvorIdleModAssistant.Views {
    public class SettingsView : ReactiveUserControl<SettingsViewModel> {
        public SettingsView() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
