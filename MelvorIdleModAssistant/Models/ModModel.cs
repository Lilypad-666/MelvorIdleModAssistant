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
        public static List<Mod> ModList = new List<Mod> {
            new Mod("Melvor-ETA", new Version("0.18.2"), "time-remaining.js", false)
        };

        //The path to the settings file
        private const string ModListFile = @".\Data\ModList.xml";

        public static void SaveMods() {
            string settingsXML = XmlModel.XmlSerializeToString(ModList);
            File.WriteAllText(ModListFile, settingsXML);
        }

        public static List<Mod> LoadActiveMods() {
            EnsureDataDirectoryExists();

            string[] files = Directory.GetFiles(ModListFile);

            foreach (string file in files) {
                Console.Log(file);
            }

            return null;
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

        private static void EnsureDataDirectoryExists() {
            FileInfo dataPath = new FileInfo(ModListFile);
            if (!dataPath.Directory.Exists) {
                Directory.CreateDirectory(dataPath.DirectoryName);
            }
        }
    }

    public class Mod {
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

        //The last known valid game version
        private Version lastValidGameVersion;
        [XmlElement("LastValidGameVersion")]
        public Version LastValidVersion
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

        //Version of the installed mod
        private bool enabled;
        [XmlElement("Enabled")]
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        public Mod(string Name, Version LastValidVersion, string MainFile, bool Enabled = false, List<string> ExtraCommands = null) {
            this.Name = Name;
            this.LastValidVersion = LastValidVersion;
            this.MainFile = MainFile;

            this.Enabled = Enabled;

            if (ExtraCommands == null) {
                this.ExtraCommands = new List<string>();
            }
            else {
                this.ExtraCommands = ExtraCommands;
            }
        }
    }
}
