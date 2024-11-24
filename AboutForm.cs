using System;
using System.Drawing;
using System.Windows.Forms;
using FolderToPDF;


public class AboutForm : Form
{
    private Label Infolabel;
    private Button btnCheckForUpdates;
    private LinkLabel linkLabel1;
    private Button closeButton;

    public AboutForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Infolabel = new Label();
        closeButton = new Button();
        btnCheckForUpdates = new Button();
        linkLabel1 = new LinkLabel();
        SuspendLayout();
        // 
        // Infolabel
        // 
        Infolabel.Location = new Point(12, 9);
        Infolabel.Name = "Infolabel";
        Infolabel.Size = new Size(324, 160);
        Infolabel.TabIndex = 0;
        Infolabel.Text = $"FolderToPDF\n\nVersion {AppInfo.Version}\n\nA simple tool to convert folder contents to PDF.\n\nDeveloped by Murr \n@ 2024 All rights reserved.";
        Infolabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // closeButton
        // 
        closeButton.Location = new Point(138, 233);
        closeButton.Name = "closeButton";
        closeButton.Size = new Size(60, 30);
        closeButton.TabIndex = 1;
        closeButton.Text = "Close";
        closeButton.Click += CloseButton_Click;
        // 
        // btnCheckForUpdates
        // 
        btnCheckForUpdates.Location = new Point(93, 192);
        btnCheckForUpdates.Name = "btnCheckForUpdates";
        btnCheckForUpdates.Size = new Size(150, 23);
        btnCheckForUpdates.TabIndex = 7;
        btnCheckForUpdates.Text = "Check for Updates";
        btnCheckForUpdates.UseVisualStyleBackColor = true;
        btnCheckForUpdates.Click += btnCheckForUpdates_Click;
        // 
        // linkLabel1
        // 
        linkLabel1.Location = new Point(118, 159);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Size = new Size(125, 20);
        linkLabel1.TabIndex = 8;
        linkLabel1.TabStop = true;
        linkLabel1.Text = "https://pdf.murr.li";
        linkLabel1.LinkClicked += linkLabel1_LinkClicked;
        // 
        // AboutForm
        // 
        ClientSize = new Size(347, 283);
        Controls.Add(linkLabel1);
        Controls.Add(btnCheckForUpdates);
        Controls.Add(Infolabel);
        Controls.Add(closeButton);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "About FolderToPDF";
        ResumeLayout(false);
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
            using (HttpClient client = new HttpClient())
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