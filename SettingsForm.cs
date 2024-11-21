using FolderToPDF;
using System;
using System.Windows.Forms;

namespace FolderToFOF
{
    public partial class SettingsForm : Form
    {
        private SettingsManager settingsManager;

        public SettingsForm(SettingsManager settingsManager)
        {
            InitializeComponent();
            this.settingsManager = settingsManager;
            LoadSettings();
            ApplyTheme();
        }

        private void LoadSettings()
        {
            var settings = settingsManager.Settings;
            txtTruncatedContentLength.Text = settings.TruncatedContentLength.ToString();
            txtTitleFont.Text = settings.TitleFont;
            txtTitleFontSize.Text = settings.TitleFontSize.ToString();
            txtContentFont.Text = settings.ContentFont;
            txtContentFontSize.Text = settings.ContentFontSize.ToString();
            txtIncludeFiles.Text = settings.IncludeFiles != null ? string.Join(", ", settings.IncludeFiles) : string.Empty;
        }

        private void SaveSettings()
        {
            var settings = settingsManager.Settings;
            settings.TruncatedContentLength = int.Parse(txtTruncatedContentLength.Text);
            settings.TitleFont = txtTitleFont.Text;
            settings.TitleFontSize = int.Parse(txtTitleFontSize.Text);
            settings.ContentFont = txtContentFont.Text;
            settings.ContentFontSize = int.Parse(txtContentFontSize.Text);
            settings.IncludeFiles = txtIncludeFiles.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            settingsManager.SaveSettings();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            SaveSettings();
        }

        private void BtnCheckForUpdates_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Check for updates feature is not implemented yet.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(this, settingsManager.Settings.DarkMode);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
