using MelvorIdleModAssistant.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace MelvorIdleModAssistant.ViewModels {
    public class ModsViewModel : ReactiveObject, IRoutableViewModel {
        public MainWindowViewModel MainWindowVM { get; }

        // Reference to IScreen that owns the routable view model.
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

        public ModsViewModel(MainWindowViewModel MainWindowVM) {
            this.MainWindowVM = MainWindowVM;
            HostScreen = MainWindowVM;

            ModModel.LoadModList();

        }
    }
}
