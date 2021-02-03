using Avalonia.Media;
using MelvorIdleModAssistant.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MelvorIdleModAssistant.ViewModels {
    public class ModsViewModel : ReactiveObject, IRoutableViewModel, INotifyPropertyChanged {

        //Event handler for when properties are changed
        public event PropertyChangedEventHandler PropertyChanged;
        public MainWindowViewModel MainWindowVM { get; }

        // Reference to IScreen that owns the routable view model.
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

        public List<Mod> UtilityModList
        {
            get
            {
                return ModModel.ModList.FindAll((mod) => {
                    return mod.Category == Mod.ModCategories.Utility;
                });
            }
        }

        public ModsViewModel(MainWindowViewModel MainWindowVM) {
            this.MainWindowVM = MainWindowVM;
            HostScreen = MainWindowVM;

            ModModel.LoadModList(this);
        }

        public void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
