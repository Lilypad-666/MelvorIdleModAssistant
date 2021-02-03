using ReactiveUI;
using System;
using System.ComponentModel;

namespace MelvorIdleModAssistant.ViewModels {
    public class InfoViewModel : ReactiveObject, IRoutableViewModel {
        public MainWindowViewModel MainWindowVM { get; }

        // Reference to IScreen that owns the routable view model.
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

        public InfoViewModel(MainWindowViewModel MainWindowVM) {
            this.MainWindowVM = MainWindowVM;
            HostScreen = MainWindowVM;
        }

        public void AgreeToTerms() {
            MainWindowVM.AcceptedTerms = true;
            Console.Log("Accepted Terms");
        }

        public void DisagreeToTerms() {
            MainWindowVM.AcceptedTerms = false;
            Console.Log("Rejected Terms");
        }
    }
}
