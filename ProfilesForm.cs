using System;
using System.Windows.Forms;

namespace FolderToPDF
{
    public partial class ProfilesForm : Form
    {
        private ProfileManager profileManager;
        private MainForm mainForm;

        public ProfilesForm(ProfileManager profileManager, MainForm mainForm)
        {
            InitializeComponent();
            this.profileManager = profileManager;
            this.mainForm = mainForm;
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            lstProfiles.Items.Clear();
            foreach (var profile in profileManager.Profiles)
            {
                lstProfiles.Items.Add(profile.Name);
            }
        }

        private void BtnSaveProfile_Click(object sender, EventArgs e)
        {
            var profileName = txtProfileName.Text;
            if (string.IsNullOrEmpty(profileName))
            {
                MessageBox.Show("Please enter a profile name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var profile = mainForm.GetCurrentProfile();
            profile.Name = profileName;
            profileManager.AddProfile(profile);
            LoadProfiles();
        }

        private void BtnLoadProfile_Click(object sender, EventArgs e)
        {
            var selectedProfileName = lstProfiles.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedProfileName))
            {
                MessageBox.Show("Please select a profile to load.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var profile = profileManager.GetProfile(selectedProfileName);
            if (profile != null)
            {
                mainForm.LoadProfile(profile);
            }
        }

        private void BtnDeleteProfile_Click(object sender, EventArgs e)
        {
            var selectedProfileName = lstProfiles.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedProfileName))
            {
                MessageBox.Show("Please select a profile to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var profile = profileManager.GetProfile(selectedProfileName);
            if (profile != null)
            {
                profileManager.Profiles.Remove(profile);
                profileManager.SaveProfiles();
                LoadProfiles();
            }
        }
    }
}
