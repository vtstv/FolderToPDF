using System;
using System.Drawing;
using System.Windows.Forms;

public class AboutForm : Form
{
    private Label Infolabel;
    private Button btnCheckForUpdates;
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
        SuspendLayout();
        // 
        // Infolabel
        // 
        Infolabel.AutoSize = true;
        Infolabel.Location = new Point(20, 20);
        Infolabel.Name = "Infolabel";
        Infolabel.Size = new Size(324, 160);
        Infolabel.TabIndex = 0;
        Infolabel.Text = "FolderToPDF\n\nVersion 1.0.2\n\nA simple tool to convert folder contents to PDF.\n\nDeveloped by Murr \n@ 2024 All rights reserved.";
        Infolabel.TextAlign = ContentAlignment.MiddleCenter;
        Infolabel.Click += Infolabel_Click;
        // 
        // closeButton
        // 
        closeButton.Location = new Point(110, 100);
        closeButton.Name = "closeButton";
        closeButton.Size = new Size(60, 30);
        closeButton.TabIndex = 1;
        closeButton.Text = "Close";
        closeButton.Click += CloseButton_Click;
        // 
        // btnCheckForUpdates
        // 
        btnCheckForUpdates.Location = new Point(75, 183);
        btnCheckForUpdates.Name = "btnCheckForUpdates";
        btnCheckForUpdates.Size = new Size(150, 23);
        btnCheckForUpdates.TabIndex = 7;
        btnCheckForUpdates.Text = "Check for Updates";
        btnCheckForUpdates.UseVisualStyleBackColor = true;
        btnCheckForUpdates.Click += btnCheckForUpdates_Click;
        // 
        // AboutForm
        // 
        ClientSize = new Size(300, 247);
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
        PerformLayout();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void btnCheckForUpdates_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void Infolabel_Click(object sender, EventArgs e)
    {

    }
}
