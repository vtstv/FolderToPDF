using System;
using System.Windows.Forms;

namespace FolderToPDF
{
    public partial class SettingsForm : Form
    {
        private SettingsManager settingsManager;

        public SettingsForm()
        {
            InitializeComponent();
            settingsManager = new SettingsManager();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = settingsManager.Settings;
            txtTruncatedContentLength.Text = settings.TruncatedContentLength.ToString();
            txtTitleFont.Text = settings.TitleFont;
            txtTitleFontSize.Text = settings.TitleFontSize.ToString();
            txtContentFont.Text = settings.ContentFont;
            txtContentFontSize.Text = settings.ContentFontSize.ToString();
        }

        private void SaveSettings()
        {
            var settings = settingsManager.Settings;
            settings.TruncatedContentLength = int.Parse(txtTruncatedContentLength.Text);
            settings.TitleFont = txtTitleFont.Text;
            settings.TitleFontSize = int.Parse(txtTitleFontSize.Text);
            settings.ContentFont = txtContentFont.Text;
            settings.ContentFontSize = int.Parse(txtContentFontSize.Text);
            settingsManager.SaveSettings();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
            MessageBox.Show("Settings saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void BtnCheckForUpdates_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Check for updates feature is not implemented yet.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
