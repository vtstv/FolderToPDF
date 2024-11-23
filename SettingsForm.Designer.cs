namespace FolderToPDF
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtTruncatedContentLength;
        private TextBox txtTitleFont;
        private TextBox txtTitleFontSize;
        private TextBox txtContentFont;
        private TextBox txtContentFontSize;
        private Label lblTruncatedContentLength;
        private Label lblTitleFont;
        private Label lblTitleFontSize;
        private Label lblContentFont;
        private Label lblContentFontSize;
        private TextBox txtIncludeFiles;
        private Label lblIncludeFiles;
        private Button Save;

        private void InitializeComponent()
        {
            txtTruncatedContentLength = new TextBox();
            txtTitleFont = new TextBox();
            txtTitleFontSize = new TextBox();
            txtContentFont = new TextBox();
            txtContentFontSize = new TextBox();
            lblTruncatedContentLength = new Label();
            lblTitleFont = new Label();
            lblTitleFontSize = new Label();
            lblContentFont = new Label();
            lblContentFontSize = new Label();
            txtIncludeFiles = new TextBox();
            lblIncludeFiles = new Label();
            Save = new Button();
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
            // lblTruncatedContentLength
            // 
            lblTruncatedContentLength.AutoSize = true;
            lblTruncatedContentLength.Location = new Point(20, 23);
            lblTruncatedContentLength.Name = "lblTruncatedContentLength";
            lblTruncatedContentLength.Size = new Size(117, 20);
            lblTruncatedContentLength.TabIndex = 7;
            lblTruncatedContentLength.Text = "Input file length:";
            // 
            // lblTitleFont
            // 
            lblTitleFont.AutoSize = true;
            lblTitleFont.Location = new Point(20, 53);
            lblTitleFont.Name = "lblTitleFont";
            lblTitleFont.Size = new Size(74, 20);
            lblTitleFont.TabIndex = 9;
            lblTitleFont.Text = "Title Font:";
            // 
            // lblTitleFontSize
            // 
            lblTitleFontSize.AutoSize = true;
            lblTitleFontSize.Location = new Point(20, 83);
            lblTitleFontSize.Name = "lblTitleFontSize";
            lblTitleFontSize.Size = new Size(105, 20);
            lblTitleFontSize.TabIndex = 10;
            lblTitleFontSize.Text = "Title Font Size:";
            // 
            // lblContentFont
            // 
            lblContentFont.AutoSize = true;
            lblContentFont.Location = new Point(20, 113);
            lblContentFont.Name = "lblContentFont";
            lblContentFont.Size = new Size(97, 20);
            lblContentFont.TabIndex = 11;
            lblContentFont.Text = "Content Font:";
            // 
            // lblContentFontSize
            // 
            lblContentFontSize.AutoSize = true;
            lblContentFontSize.Location = new Point(20, 143);
            lblContentFontSize.Name = "lblContentFontSize";
            lblContentFontSize.Size = new Size(72, 20);
            lblContentFontSize.TabIndex = 12;
            lblContentFontSize.Text = "Font Size:";
            // 
            // txtIncludeFiles
            // 
            txtIncludeFiles.Location = new Point(150, 170);
            txtIncludeFiles.Name = "txtIncludeFiles";
            txtIncludeFiles.Size = new Size(100, 27);
            txtIncludeFiles.TabIndex = 13;
            txtIncludeFiles.Visible = false;
            // 
            // lblIncludeFiles
            // 
            lblIncludeFiles.AutoSize = true;
            lblIncludeFiles.Location = new Point(20, 173);
            lblIncludeFiles.Name = "lblIncludeFiles";
            lblIncludeFiles.Size = new Size(93, 20);
            lblIncludeFiles.TabIndex = 14;
            lblIncludeFiles.Text = "Include Files:";
            lblIncludeFiles.Visible = false;
            // 
            // Save
            // 
            Save.Location = new Point(92, 196);
            Save.Name = "Save";
            Save.Size = new Size(94, 29);
            Save.TabIndex = 15;
            Save.Text = "Save";
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // SettingsForm
            // 
            AcceptButton = Save;
            ClientSize = new Size(284, 265);
            Controls.Add(Save);
            Controls.Add(lblIncludeFiles);
            Controls.Add(txtIncludeFiles);
            Controls.Add(lblContentFontSize);
            Controls.Add(lblContentFont);
            Controls.Add(lblTitleFontSize);
            Controls.Add(lblTitleFont);
            Controls.Add(lblTruncatedContentLength);
            Controls.Add(txtContentFontSize);
            Controls.Add(txtContentFont);
            Controls.Add(txtTitleFontSize);
            Controls.Add(txtTitleFont);
            Controls.Add(txtTruncatedContentLength);
            Name = "SettingsForm";
            Text = "Settings";
            Load += SettingsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
