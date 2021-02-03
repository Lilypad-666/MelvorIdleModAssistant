using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MelvorIdleModAssistant.ViewModels;

namespace MelvorIdleModAssistant.Views {
    public class ModsView : ReactiveUserControl<ModsViewModel> {
        public ModsView() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
