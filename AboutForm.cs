using System;
using System.Drawing;
using System.Windows.Forms;

public class AboutForm : Form
{
    private Label infoLabel;
    private Button closeButton;

    public AboutForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.infoLabel = new Label();
        this.closeButton = new Button();
        this.SuspendLayout();

        // Configure form
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "About FolderToPDF";
        this.ClientSize = new Size(300, 200);

        // Configure info label
        this.infoLabel.AutoSize = true;
        this.infoLabel.Location = new Point(20, 20);
        this.infoLabel.Text = "FolderToPDF\n\n" +
                            "Version 0.2.1\n\n" +
                            "A simple tool to convert folder contents to PDF.\n\n" +
                            "Developed by Murr \n" +
                            "© 2024 All rights reserved.";
        this.infoLabel.TextAlign = ContentAlignment.MiddleCenter;

        // Configure close button
        this.closeButton.Text = "Close";
        this.closeButton.Size = new Size(80, 30);
        this.closeButton.Location = new Point(110, 150);
        this.closeButton.Click += new EventHandler(CloseButton_Click);

        // Add controls to form
        this.Controls.Add(this.infoLabel);
        this.Controls.Add(this.closeButton);

        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}