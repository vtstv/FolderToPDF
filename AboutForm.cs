using System;
using System.Drawing;
using System.Windows.Forms;

public class AboutForm : Form
{
    private Label Infolabel;
    private Button closeButton;

    public AboutForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Infolabel = new Label();
        this.closeButton = new Button();
        this.SuspendLayout();
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "About FolderToPDF";
        this.ClientSize = new Size(300, 200);

        this.Infolabel.AutoSize = true;
        this.Infolabel.Location = new Point(20, 20);
        this.Infolabel.Text = "FolderToPDF\n\n" +
            "Version 0.2.1\n\n" +
            "A simple tool to convert folder contents to PDF.\n\n" +
            "Developed by Murr \n" +
            "@ 2024 All rights reserved.";
        this.Infolabel.TextAlign = ContentAlignment.MiddleCenter;

        this.closeButton.Text = "Close";
        this.closeButton.Size = new Size(60, 30);
        this.closeButton.Location = new Point(110, 100);
        this.closeButton.Click += new EventHandler(CloseButton_Click);

        this.Controls.Add(this.Infolabel);
        this.Controls.Add(this.closeButton);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
