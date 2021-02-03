using MelvorIdleModAssistant.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Timers;

namespace MelvorIdleModAssistant.ViewModels {
    public class MainWindowViewModel : ReactiveObject, IScreen, INotifyPropertyChanged {

        //Event handler for when properties are changed
        public event PropertyChangedEventHandler PropertyChanged;

        //Router to change pages
        public RoutingState Router { get; } = new RoutingState();

        //The command that navigates a user to the info page.
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToInfo { get; }

        //The command that navigates a user to the mod page.
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToMods { get; }

        //The command that navigates a user to the settings page.
        public ReactiveCommand<Unit, IRoutableViewModel> NavigateToSettings { get; }

        public ModsViewModel modsVM;
        public InfoViewModel infoVM;
        public SettingsViewModel settingsVM;

        //Whether the user has accepted the terms
        public bool AcceptedTerms
        {
            get
            {
                return settingsVM.AcceptedTerms;
            }
            set
            {
                settingsVM.AcceptedTerms = value;
                OnPropertyChanged(nameof(AcceptedTerms));
            }
        }

        //Gets the last line of the console
        public string LastConsoleLine
        {
            get
            {
                return Console.LastConsoleLine;
            }
        }

        //The program's current version
        private Version programVersion;
        public Version ProgramVersion
        {
            get
            {
                if (programVersion == null) {
                    string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    programVersion = new Version(version);
                }

                return programVersion;
            }
        }

        //The game's current version
        private Version gameVersion;
        public Version GameVersion
        {
            get
            {
                if (gameVersion == null) {

                    WebClient x = new WebClient();
                    string melvorWebpage = x.DownloadString("https://melvoridle.com/");
                    string title = Regex.Match(melvorWebpage, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                    string version = Regex.Replace(title.Split(' ')[^1], "[^0-9.]", "");
                    gameVersion = new Version(version);
                }

                return gameVersion;
            }
        }

        public MainWindowViewModel() {

            Console.MainWindowVM = this;

            infoVM = new InfoViewModel(this);
            modsVM = new ModsViewModel(this);
            settingsVM = new SettingsViewModel(this);

            NavigateToInfo = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(infoVM)
            );

            NavigateToMods = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(modsVM)
            );

            NavigateToSettings = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(settingsVM)
            );

            Router.Navigate.Execute(infoVM);
        }

        public void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
