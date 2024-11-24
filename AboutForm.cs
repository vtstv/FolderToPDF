using System;
using System.Net.Http;
using System.Windows.Forms;

namespace FolderToPDF
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            
            Infolabel.Text = $"FolderToPDF\n\nVersion {AppInfo.Version}\n\nA simple tool to convert folder contents to PDF.\n\nDeveloped by Murr \n@ 2024 All rights reserved.";
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            const string UpdateUrl = "https://pdf.murr.li/version.txt";

            try
            {
                using (var client = new HttpClient())
                {
                    string latestVersionString = await client.GetStringAsync(UpdateUrl);
                    latestVersionString = latestVersionString.Trim();

                    Version currentVersion = Version.Parse(AppInfo.Version);
                    Version latestVersion = Version.Parse(latestVersionString);

                    if (currentVersion < latestVersion)
                    {
                        MessageBox.Show(
                            $"A new version ({latestVersion}) is available. Please visit the download page.",
                            "Update Available",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            "You are using the latest version.",
                            "No Updates",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not check for updates: {ex.Message}",
                    "Update Check Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://pdf.murr.li",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}