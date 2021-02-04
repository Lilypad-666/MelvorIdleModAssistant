using Microsoft.Win32;
using System;
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
        public static string GetGamePath() {
            return GetGamePathWindows();
        }

        private static string GetGamePathWindows() {
            RegistryKey fileKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1267910");

            if (fileKey != null) {   
                return (string)fileKey.GetValue("InstallLocation");
            }

            return string.Empty;
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

        private string gamePath = SettingsModel.GetGamePath();
        [XmlElement("GamePath")]
        public string GamePath
        {
            get
            {
                return gamePath;
            }
            set
            {
                gamePath = value;
                SettingsModel.SaveSettings(this);
            }
        }
    }
}
