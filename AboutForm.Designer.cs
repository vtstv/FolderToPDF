using System.Drawing;
using System.Windows.Forms;

namespace FolderToPDF
{
    partial class AboutForm
    {

        private System.ComponentModel.IContainer components = null;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

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
            Infolabel.Location = new Point(14, 12);
            Infolabel.Name = "Infolabel";
            Infolabel.Size = new Size(370, 213);
            Infolabel.TabIndex = 0;
            Infolabel.Text = "Loading...";
            Infolabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // closeButton
            // 
            closeButton.Location = new Point(158, 311);
            closeButton.Margin = new Padding(3, 4, 3, 4);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(69, 40);
            closeButton.TabIndex = 1;
            closeButton.Text = "Close";
            closeButton.Click += CloseButton_Click;
            // 
            // btnCheckForUpdates
            // 
            btnCheckForUpdates.Location = new Point(106, 256);
            btnCheckForUpdates.Margin = new Padding(3, 4, 3, 4);
            btnCheckForUpdates.Name = "btnCheckForUpdates";
            btnCheckForUpdates.Size = new Size(171, 31);
            btnCheckForUpdates.TabIndex = 7;
            btnCheckForUpdates.Text = "Check for Updates";
            btnCheckForUpdates.UseVisualStyleBackColor = true;
            btnCheckForUpdates.Click += btnCheckForUpdates_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.Location = new Point(135, 212);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(143, 27);
            linkLabel1.TabIndex = 8;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://pdf.murr.li";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(397, 377);
            Controls.Add(linkLabel1);
            Controls.Add(btnCheckForUpdates);
            Controls.Add(Infolabel);
            Controls.Add(closeButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About FolderToPDF";
            ResumeLayout(false);
        }

        #endregion

        private Label Infolabel;
        private Button btnCheckForUpdates;
        private LinkLabel linkLabel1;
        private Button closeButton;
    }
}