using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MelvorIdleModAssistant.Models {
    public class SettingsModel {

        //The path to the settings file
        private const string SettingsFile = @".\Data\settings.xml";

        public static void SaveSettings(Settings settings) {
            EnsureDataDirectoryExists();

            string settingsXML = XmlModel.XmlSerializeToString(settings);
            File.WriteAllText(SettingsFile, settingsXML);
        }

        public static Settings LoadSettings() {
            if (File.Exists(SettingsFile)) {
                string settingsXML = File.ReadAllText(SettingsFile);
                return XmlModel.XmlDeserializeFromString<Settings>(settingsXML);
            }

            return new Settings();
        }

        private static void EnsureDataDirectoryExists() {
            DirectoryInfo dataFolder = new DirectoryInfo(@".\Data");
            if (!dataFolder.Exists) {
                Directory.CreateDirectory(@".\Data");
            }
        }
    }

    public class Settings {
        private bool acceptedTerms = false;
        [XmlElement("AcceptedTerms")]
        public bool AcceptedTerms
        {
            get
            {
                return acceptedTerms;
            }
            set
            {
                acceptedTerms = value;
                SettingsModel.SaveSettings(this);
            }
        }

        private List<string> installedMods = new List<string>();
        [XmlElement("InstalledMods")]
        public List<string> InstalledMods
        {
            get
            {
                return installedMods;
            }
            set
            {
                installedMods = value;
                SettingsModel.SaveSettings(this);
            }
        }
    }
}
