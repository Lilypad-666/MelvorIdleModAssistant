using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Serialization;

namespace MelvorIdleModAssistant.Models {
    public class ModModel {
        public static List<Mod> ModList = new List<Mod>();

        public static void LoadModList() {
#if DEBUG
            CreateModListFile();
#else
            
#endif
        }

        private static async void DownloadGitRepository(string repo) {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MelvorIdleModAssistant", "1.0.0"));
            var contentsUrl = $"https://api.github.com/repos/{repo}/contents";
            var contentsJson = await httpClient.GetStringAsync(contentsUrl);
            var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);
            foreach (var file in contents) {
                var fileType = (string)file["type"];
                if (fileType == "dir") {
                    var directoryContentsUrl = (string)file["url"];
                    // use this URL to list the contents of the folder
                    System.Console.WriteLine($"DIR: {directoryContentsUrl}");
                }
                else if (fileType == "file") {
                    var downloadUrl = (string)file["download_url"];
                    // use this URL to download the contents of the file
                    System.Console.WriteLine($"DOWNLOAD: {downloadUrl}");
                }
            }
        }

        private static void CreateModListFile() {
            List<Mod> NewModList = new List<Mod> {
                new Mod("Melvor-ETA", "Displays estimated times for skills", "GMiclotte", "https://github.com/gmiclotte/Melvor-ETA", Mod.ModCategories.Utility, "0.18.2", "time-remaining.js"),
                new Mod("XP/h", "Displays XP/h for farming and combat", "Visua#9999", "https://greasyfork.org/scripts/409902-melvor-idle-xp-h/code/Melvor%20Idle%20-%20XPh.user.js", Mod.ModCategories.Utility, "0.18.2", "Melvor%20Idle%20-%20XPh.user.js"),
                new Mod("Combat Simulator Reloaded", "Simulates combat", "GMiclotte", "https://github.com/visua0/Melvor-Idle-Combat-Simulator-Reloaded", Mod.ModCategories.Utility, "0.18.2", "Extension\\Sources\\contentScript.js", new List<string> { "$(document.head).append(`<link rel=\"stylesheet\" href=\"${chrome.runtime.getURL('styles/mainStyle.css')}\">`)" }),
            };

            string modListXML = XmlModel.XmlSerializeToString(NewModList);
            File.WriteAllText(@".\ModList.xml", modListXML);
        }
    }

    public class Mod {
        public enum ModCategories {
            Utility = 0
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
        public string LastValidVersion
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

        //Whether the mod is currently installed
        private bool installed;
        [XmlElement("Installed")]
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

        public Mod() { }

        public Mod(string Name, string Description, string Author, string Source, ModCategories Category, string LastValidVersion, string MainFile, List<string> ExtraCommands = null) {
            this.Name = Name;
            this.Description = Description;
            this.Author = Author;
            this.Source = Source;
            this.Category = Category;
            this.LastValidVersion = LastValidVersion;
            this.MainFile = MainFile;

            if (ExtraCommands == null) {
                this.ExtraCommands = new List<string>();
            }
            else {
                this.ExtraCommands = ExtraCommands;
            }
        }
    }
}
