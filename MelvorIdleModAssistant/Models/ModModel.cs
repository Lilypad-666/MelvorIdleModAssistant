using Avalonia.Media;
using MelvorIdleModAssistant.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace MelvorIdleModAssistant.Models {
    public class ModModel {
        public static List<Mod> ModList = new List<Mod>();

        public static void LoadModList(ModsViewModel ModsVM) {
            var modListXml = (new WebClient()).DownloadString("https://raw.githubusercontent.com/Lilypad-666/MelvorIdleModAssistant/main/MelvorIdleModAssistant/ModList.xml");
            ModList = XmlModel.XmlDeserializeFromString<List<Mod>>(modListXml);

            ModsVM.OnPropertyChanged("ModList");
        }

        public static void DownloadSource(string source, string path) {

            if (source.Contains("github", StringComparison.InvariantCultureIgnoreCase)) {
                DownloadGitHubRepository(source, path);
            }
        }

        private static async void DownloadGitHubRepository(string repo, string path) {
            //HttpClient httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MelvorIdleModAssistant", "1.0.0"));
            //string contentsUrl = "https://api.github.com/repos/" + repo + "/contents";
            //string contentsJson = await httpClient.GetStringAsync(contentsUrl);
            //JArray contents = (JArray)JsonConvert.DeserializeObject(contentsJson);

            //using (WebClient client = new WebClient()) {
            //    foreach (var file in contents) {
            //        string fileType = (string)file["type"];

            //        if (fileType == "dir") {
            //            var directoryContentsUrl = (string)file["url"];
            //            // use this URL to list the contents of the folder
            //            Console.Log($"DIR: {directoryContentsUrl}");
            //        }

            //        if (fileType == "file") {
            //            string downloadUrl = (string)file["download_url"];
            //            //https://raw.githubusercontent.com/ + repo + /fileName
            //            string fileName = downloadUrl.Replace("https://raw.githubusercontent.com/" + repo + "/", "");
            //            fileName = fileName.Substring(fileName.IndexOf('/'));
            //            Console.Log(fileName);
            //            client.DownloadFile(downloadUrl, path + fileName);
            //        }
            //    }
            //}


        }

        public static void CreateModListFile() {
            List<Mod> NewModList = new List<Mod> {
                new Mod("Melvor-ETA", "Displays estimated times for skills", "GMiclotte", "gmiclotte/Melvor-ETA", Mod.ModCategories.Utility, "0.18.2", "time-remaining.js"),
                new Mod("XP/h", "Displays XP/h for farming and combat", "Visua#9999", "https://greasyfork.org/scripts/409902-melvor-idle-xp-h/code/Melvor%20Idle%20-%20XPh.user.js", Mod.ModCategories.Utility, "0.18.2", "Melvor%20Idle%20-%20XPh.user.js"),
                new Mod("Combat Simulator Reloaded", "Simulates combat", "GMiclotte", "https://github.com/visua0/Melvor-Idle-Combat-Simulator-Reloaded", Mod.ModCategories.Utility, "0.18.2", "Extension\\Sources\\contentScript.js", new List<string> { "$(document.head).append(`<link rel=\"stylesheet\" href=\"${chrome.runtime.getURL('styles/mainStyle.css')}\">`)" }),
            };

            string modListXML = XmlModel.XmlSerializeToString(NewModList);
            File.WriteAllText(@".\ModList.xml", modListXML);
        }
    }

    public class Mod {
        public enum ModCategories {
            Utility = 0,
        }

        //The name of the mod
        private string name;
        [XmlElement("Name")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        //Description of the mod
        private string description;
        [XmlElement("Description")]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        //Author of the mod
        private string author;
        [XmlElement("Author")]
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                author = value;
            }
        }

        //Link to the mod
        private string source;
        [XmlElement("Source")]
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        //What category is the mod
        private ModCategories category;
        [XmlElement("Category")]
        public ModCategories Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
            }
        }

        //The last known valid game version
        private string lastValidGameVersion;
        [XmlElement("LastValidGameVersion")]
        public string LastValidGameVersion
        {
            get
            {
                return lastValidGameVersion;
            }
            set
            {
                lastValidGameVersion = value;
            }
        }

        //The main file to start
        private string mainFile;
        [XmlElement("MainFile")]
        public string MainFile
        {
            get
            {
                return mainFile;
            }
            set
            {
                mainFile = value;
            }
        }

        //Any extra commands that the extension needs to run
        private List<string> extraCommands;
        [XmlElement("ExtraCommands")]
        public List<string> ExtraCommands
        {
            get
            {
                return extraCommands;
            }
            set
            {
                extraCommands = value;
            }
        }

        //Whether the version text is green or red
        [XmlIgnore]
        public IBrush GameVersionColor
        {
            get
            {
                Version modValidVersion = new Version(LastValidGameVersion);
                if (modValidVersion >= MainWindowViewModel.GameVersion) {
                    return Brushes.LightGreen;
                }
                else {
                    return Brushes.Red;
                }
            }
        }

        //Whether the mod is currently installed or not
        private bool installed;
        [XmlIgnore]
        public bool Installed
        {
            get
            {
                return installed;
            }
            set
            {
                installed = value;
            }
        }

        //Gets the text for the install/uninstall button
        [XmlIgnore]
        public string InstallButtonText
        {
            get
            {
                if (Installed) {
                    return "Uninstall";
                }
                else {
                    return "Install";
                }
            }
        }

        //Whether the checkbox is checked
        private bool isChecked;
        [XmlIgnore]
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
            }
        }

        //Opens more info for the mod
        public void OpenMoreInfo() {
            OpenBrowser(Source);
        }

        public void UninstallMod() {
            Installed = false;
        }

        public Mod() { }

        public Mod(string Name, string Description, string Author, string Source, ModCategories Category, string LastValidGameVersion, string MainFile, List<string> ExtraCommands = null) {
            this.Name = Name;
            this.Description = Description;
            this.Author = Author;
            this.Source = Source;
            this.Category = Category;
            this.LastValidGameVersion = LastValidGameVersion;
            this.MainFile = MainFile;

            if (ExtraCommands == null) {
                this.ExtraCommands = new List<string>();
            }
            else {
                this.ExtraCommands = ExtraCommands;
            }

            Installed = MainWindowViewModel.settingsVM.InstalledMods.Contains(Name);
        }

        public static void OpenBrowser(string url) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                // If no associated application/json MimeType is found xdg-open opens retrun error
                // but it tries to open it anyway using the console editor (nano, vim, other..)
                ShellExec($"xdg-open {url}", waitForExit: false);
            }
            else {
                using (Process process = Process.Start(new ProcessStartInfo {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? url : "open",
                    Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"-e {url}" : "",
                    CreateNoWindow = true,
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                })) ;
            }
        }

        private static void ShellExec(string cmd, bool waitForExit = true) {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            using (var process = Process.Start(
                new ProcessStartInfo {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            )) {
                if (waitForExit) {
                    process.WaitForExit();
                }
            }
        }
    }
}
