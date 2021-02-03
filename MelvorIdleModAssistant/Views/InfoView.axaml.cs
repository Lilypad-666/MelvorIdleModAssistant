using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MelvorIdleModAssistant.ViewModels;

namespace MelvorIdleModAssistant.Views {
    public class InfoView : ReactiveUserControl<InfoViewModel> {
        public InfoView() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
