using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FolderToPDF
{
    public class SettingsManager
    {
        private const string SETTINGS_FILE = "settings.json";
        private Settings _settings;

        public SettingsManager()
        {
            LoadSettings();
        }

        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public void LoadSettings()
        {
            try
            {
                if (File.Exists(SETTINGS_FILE))
                {
                    _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_FILE));
                    _settings.DirectoryPath = _settings.DirectoryPath?.Trim();
                    _settings.OutputPathTxt = _settings.OutputPathTxt?.Trim();
                    _settings.OutputPathPdf = _settings.OutputPathPdf?.Trim();
                    _settings.FileTypes = _settings.FileTypes?.Select(type => type.Trim()).ToList();
                    _settings.ExcludeFolders = _settings.ExcludeFolders?.Select(folder => folder.Trim()).ToList();
                    _settings.ExcludeFiles = _settings.ExcludeFiles?.Select(file => file.Trim()).ToList();
                    _settings.IncludeFiles = _settings.IncludeFiles?.Select(file => file.Trim()).ToList();
                }
                else
                {
                    _settings = new Settings
                    {
                        FileTypes = new List<string> { "js", "py", "cs" },
                        DarkMode = IsDarkModeEnabled()
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }





        public void SaveSettings()
        {
            try
            {
                File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(_settings, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool IsDarkModeEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
            {
                if (key != null)
                {
                    Object o = key.GetValue("AppsUseLightTheme");
                    if (o != null)
                    {
                        return (int)o == 0;
                    }
                }
            }
            return true;
        }
    }

    public class Settings
    {
        public string DirectoryPath { get; set; }
        public List<string> FileTypes { get; set; } = new List<string>();
        public List<string> ExcludeFolders { get; set; } = new List<string>();
        public List<string> ExcludeFiles { get; set; } = new List<string>();
        public List<string> IncludeFiles { get; set; } = new List<string>();
        public string OutputPathTxt { get; set; }
        public string OutputPathPdf { get; set; }
        public bool RemoveComments { get; set; }
        public bool ReplaceSensitiveInfo { get; set; }
        public bool DarkMode { get; set; }
        public int TruncatedContentLength { get; set; } = 100000;
        public string TitleFont { get; set; } = "Helvetica";
        public int TitleFontSize { get; set; } = 9;
        public string ContentFont { get; set; } = "Courier";
        public int ContentFontSize { get; set; } = 6;
    }

}
