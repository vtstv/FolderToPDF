using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FolderToPDF
{
    public class ProfileManager
    {
        private const string PROFILES_FILE = "profiles.json";
        private List<Profile> _profiles;

        public ProfileManager()
        {
            LoadProfiles();
        }

        public List<Profile> Profiles
        {
            get { return _profiles; }
            set { _profiles = value; }
        }

        public void LoadProfiles()
        {
            try
            {
                if (File.Exists(PROFILES_FILE))
                {
                    _profiles = JsonConvert.DeserializeObject<List<Profile>>(File.ReadAllText(PROFILES_FILE));
                }
                else
                {
                    _profiles = new List<Profile>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profiles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void SaveProfiles()
        {
            try
            {
                File.WriteAllText(PROFILES_FILE, JsonConvert.SerializeObject(_profiles, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving profiles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void AddProfile(Profile profile)
        {
            _profiles.Add(profile);
            SaveProfiles();
        }

        public void UpdateProfile(Profile profile)
        {
            var existingProfile = _profiles.Find(p => p.Name == profile.Name);
            if (existingProfile != null)
            {
                existingProfile.DirectoryPath = profile.DirectoryPath;
                existingProfile.FileTypes = profile.FileTypes;
                existingProfile.OutputPathPdf = profile.OutputPathPdf;
                existingProfile.OutputPathTxt = profile.OutputPathTxt;
                existingProfile.ExcludeFolders = profile.ExcludeFolders;
                existingProfile.ExcludeFiles = profile.ExcludeFiles;
                existingProfile.IncludeFiles = profile.IncludeFiles;
                SaveProfiles();
            }
        }

        public Profile GetProfile(string name)
        {
            return _profiles.Find(p => p.Name == name);
        }
    }

    public class Profile
    {
        public string Name { get; set; }
        public string DirectoryPath { get; set; }
        public List<string> FileTypes { get; set; }
        public string OutputPathPdf { get; set; }
        public string OutputPathTxt { get; set; }
        public List<string> ExcludeFolders { get; set; }
        public List<string> ExcludeFiles { get; set; }
        public List<string> IncludeFiles { get; set; }
    }
}
