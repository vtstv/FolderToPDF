namespace FolderToFOF
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtTruncatedContentLength;
        private TextBox txtTitleFont;
        private TextBox txtTitleFontSize;
        private TextBox txtContentFont;
        private TextBox txtContentFontSize;
        private Button btnCheckForUpdates;
        private Label lblTruncatedContentLength;
        private Label lblTitleFont;
        private Label lblTitleFontSize;
        private Label lblContentFont;
        private Label lblContentFontSize;
        private TextBox txtIncludeFiles;
        private Label lblIncludeFiles;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtTruncatedContentLength = new TextBox();
            txtTitleFont = new TextBox();
            txtTitleFontSize = new TextBox();
            txtContentFont = new TextBox();
            txtContentFontSize = new TextBox();
            btnCheckForUpdates = new Button();
            lblTruncatedContentLength = new Label();
            lblTitleFont = new Label();
            lblTitleFontSize = new Label();
            lblContentFont = new Label();
            lblContentFontSize = new Label();
            txtIncludeFiles = new TextBox();
            lblIncludeFiles = new Label();
            SuspendLayout();
            // 
            // txtTruncatedContentLength
            // 
            txtTruncatedContentLength.Location = new Point(150, 20);
            txtTruncatedContentLength.Name = "txtTruncatedContentLength";
            txtTruncatedContentLength.Size = new Size(100, 27);
            txtTruncatedContentLength.TabIndex = 0;
            // 
            // txtTitleFont
            // 
            txtTitleFont.Location = new Point(150, 50);
            txtTitleFont.Name = "txtTitleFont";
            txtTitleFont.Size = new Size(100, 27);
            txtTitleFont.TabIndex = 1;
            // 
            // txtTitleFontSize
            // 
            txtTitleFontSize.Location = new Point(150, 80);
            txtTitleFontSize.Name = "txtTitleFontSize";
            txtTitleFontSize.Size = new Size(100, 27);
            txtTitleFontSize.TabIndex = 2;
            // 
            // txtContentFont
            // 
            txtContentFont.Location = new Point(150, 110);
            txtContentFont.Name = "txtContentFont";
            txtContentFont.Size = new Size(100, 27);
            txtContentFont.TabIndex = 3;
            // 
            // txtContentFontSize
            // 
            txtContentFontSize.Location = new Point(150, 140);
            txtContentFontSize.Name = "txtContentFontSize";
            txtContentFontSize.Size = new Size(100, 27);
            txtContentFontSize.TabIndex = 4;
            // 
            // btnCheckForUpdates
            // 
            btnCheckForUpdates.Location = new Point(100, 260);
            btnCheckForUpdates.Name = "btnCheckForUpdates";
            btnCheckForUpdates.Size = new Size(150, 23);
            btnCheckForUpdates.TabIndex = 6;
            btnCheckForUpdates.Text = "Check for Updates";
            btnCheckForUpdates.UseVisualStyleBackColor = true;
            btnCheckForUpdates.Click += BtnCheckForUpdates_Click;
            // 
            // lblTruncatedContentLength
            // 
            lblTruncatedContentLength.AutoSize = true;
            lblTruncatedContentLength.Location = new Point(20, 23);
            lblTruncatedContentLength.Name = "lblTruncatedContentLength";
            lblTruncatedContentLength.Size = new Size(182, 20);
            lblTruncatedContentLength.TabIndex = 7;
            lblTruncatedContentLength.Text = "Truncated Content Length:";
            // 
            // lblTitleFont
            // 
            lblTitleFont.AutoSize = true;
            lblTitleFont.Location = new Point(20, 53);
            lblTitleFont.Name = "lblTitleFont";
            lblTitleFont.Size = new Size(74, 20);
            lblTitleFont.TabIndex = 8;
            lblTitleFont.Text = "Title Font:";
            // 
            // lblTitleFontSize
            // 
            lblTitleFontSize.AutoSize = true;
            lblTitleFontSize.Location = new Point(25, 83);
            lblTitleFontSize.Name = "lblTitleFontSize";
            lblTitleFontSize.Size = new Size(105, 20);
            lblTitleFontSize.TabIndex = 9;
            lblTitleFontSize.Text = "Title Font Size:";
            // 
            // lblContentFont
            // 
            lblContentFont.AutoSize = true;
            lblContentFont.Location = new Point(25, 113);
            lblContentFont.Name = "lblContentFont";
            lblContentFont.Size = new Size(97, 20);
            lblContentFont.TabIndex = 10;
            lblContentFont.Text = "Content Font:";
            // 
            // lblContentFontSize
            // 
            lblContentFontSize.AutoSize = true;
            lblContentFontSize.Location = new Point(25, 143);
            lblContentFontSize.Name = "lblContentFontSize";
            lblContentFontSize.Size = new Size(128, 20);
            lblContentFontSize.TabIndex = 11;
            lblContentFontSize.Text = "Content Font Size:";
            // 
            // txtIncludeFiles
            // 
            txtIncludeFiles.Location = new Point(150, 170);
            txtIncludeFiles.Name = "txtIncludeFiles";
            txtIncludeFiles.Size = new Size(150, 27);
            txtIncludeFiles.TabIndex = 12;
            // 
            // lblIncludeFiles
            // 
            lblIncludeFiles.AutoSize = true;
            lblIncludeFiles.Location = new Point(25, 173);
            lblIncludeFiles.Name = "lblIncludeFiles";
            lblIncludeFiles.Size = new Size(93, 20);
            lblIncludeFiles.TabIndex = 13;
            lblIncludeFiles.Text = "Include Files:";
            // 
            // SettingsForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoSize = true;
            ClientSize = new Size(284, 293);
            Controls.Add(lblIncludeFiles);
            Controls.Add(txtIncludeFiles);
            Controls.Add(lblContentFontSize);
            Controls.Add(lblContentFont);
            Controls.Add(lblTitleFontSize);
            Controls.Add(lblTitleFont);
            Controls.Add(lblTruncatedContentLength);
            Controls.Add(btnCheckForUpdates);
            Controls.Add(txtContentFontSize);
            Controls.Add(txtContentFont);
            Controls.Add(txtTitleFontSize);
            Controls.Add(txtTitleFont);
            Controls.Add(txtTruncatedContentLength);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            Load += SettingsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
