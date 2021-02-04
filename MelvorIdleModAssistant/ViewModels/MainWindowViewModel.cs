using HtmlAgilityPack;
using MelvorIdleModAssistant.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        public static ModsViewModel modsVM;
        public static InfoViewModel infoVM;
        public static SettingsViewModel settingsVM;

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
        private static Version programVersion;
        public static Version ProgramVersion
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
        private static Version gameVersion;
        public static Version GameVersion
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

            //ModModel.CreateModListFile();
        }

        public void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void InstallUpdateMods() {
            List<Mod> checkedMods = modsVM.UtilityModList.FindAll((mod) =>
            {
                return mod.IsChecked;
            });

            foreach (Mod mod in checkedMods) {
                InstallUpdateMod(mod);
            }

            UpdateIndexFile();
        }

        private void UpdateIndexFile() {
            string gameIndexFilePath = settingsVM.GamePath + @"\index.html";
            string defaultIndexFilePath = @".\index.html";

            File.Copy(defaultIndexFilePath, gameIndexFilePath, true);

            HtmlDocument index = new HtmlDocument();
            index.Load(gameIndexFilePath);

            //Add the onload to the iframe
            //index.GetElementbyId("game").SetAttributeValue("onload", "injectModLoader()");

            //HtmlNode injectModLoader = GetInjectModLoaderNode();

            index.GetElementbyId("injectModLoaderScript").InnerHtml = GetInjectModLoaderHtml();

            //index.DocumentNode.FirstChild.AppendChild(injectModLoader);

            index.Save(gameIndexFilePath);
        }

        private string GetInjectModLoaderHtml() {
            string injectModLoaderHtml = @"
            function injectModLoader() {
		        console.log(window.jQuery);
		        var script = document.createElement(""script"");


                script.textContent = `
		        var gameLoadedInterval;

                    function loadMods() {
                        gameLoadedInterval = setInterval(checkGameLoaded, 200);
                    };

                    function checkGameLoaded() {
                        if (typeof confirmedLoaded !== ""undefined"" && confirmedLoaded && !currentlyCatchingUp) {
                            try {
                                console.log(""Loading Melvor Idle Mod Assistant Mods"");

                                var fs = require(""fs"");

            ";

            foreach (string modName in settingsVM.InstalledMods) {
                Mod? mod = modsVM.UtilityModList.Find((mod) => {
                    return mod.Name == modName;
                });

                if (mod != null) {
                    string fileSafeModName = mod.Name;

                    foreach (var c in Path.GetInvalidFileNameChars()) {
                        fileSafeModName = fileSafeModName.Replace(c, '-');
                    }

                    //require('fs').readFile('extensions/melvor-eta/time-remaining.js', 'utf8', (err, data) => { eval(data); });

                    injectModLoaderHtml += @"//Loading for " + mod.Name + @"
                    fs.readFile(""mods/" + fileSafeModName + @"/" + mod.MainFile + @""", ""utf8"", (err, data) => { eval(data); });";

                    foreach (string extraCommand in mod.ExtraCommands) {
                        injectModLoaderHtml += extraCommand + @"
                        ";
                    }

                    injectModLoaderHtml += @"
                    ";
                }
            }

            injectModLoaderHtml += @"

            //Stops checking if the game is loaded
                            clearInterval(gameLoadedInterval);
                        }
                        catch (e) {
                            console.error(e);
                        }
                    }
                }
		    `;

                //Inject the function into the game
                document.getElementById(""game"").contentWindow.document.body.appendChild(script);

                //Starts the interval to check for the game loaded
                document.getElementById(""game"").contentWindow.loadMods();
            }
            ";

            return injectModLoaderHtml;
        }

        private void InstallUpdateMod(Mod mod) {
            if (!settingsVM.InstalledMods.Contains(mod.Name)) {
                settingsVM.InstalledMods.Add(mod.Name);
            }

            string fileSafeModName = mod.Name;

            foreach (var c in Path.GetInvalidFileNameChars()) {
                fileSafeModName = fileSafeModName.Replace(c, '-');
            }

            string path = Path.Combine(settingsVM.GamePath, "mods", fileSafeModName);

            DirectoryInfo dir = new DirectoryInfo(path);

            if (dir.Exists) {
                SetAttributesNormal(dir);
                dir.Delete(true);
            }

            Directory.CreateDirectory(path);

            ModModel.DownloadSource(mod.Source, path);

            mod.Installed = true;
        }

        public void SetAttributesNormal(DirectoryInfo dir) {
            foreach (DirectoryInfo subDir in dir.GetDirectories()) {
                SetAttributesNormal(subDir);

            }

            foreach (FileInfo file in dir.GetFiles()) {
                file.Attributes = FileAttributes.Normal;
            }
        }
    }
}
