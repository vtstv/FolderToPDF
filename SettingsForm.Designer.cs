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
        private Button btnSave;
        private Button btnCheckForUpdates;
        private Label lblTruncatedContentLength;
        private Label lblTitleFont;
        private Label lblTitleFontSize;
        private Label lblContentFont;
        private Label lblContentFontSize;

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
            this.txtTruncatedContentLength = new System.Windows.Forms.TextBox();
            this.txtTitleFont = new System.Windows.Forms.TextBox();
            this.txtTitleFontSize = new System.Windows.Forms.TextBox();
            this.txtContentFont = new System.Windows.Forms.TextBox();
            this.txtContentFontSize = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCheckForUpdates = new System.Windows.Forms.Button();
            this.lblTruncatedContentLength = new System.Windows.Forms.Label();
            this.lblTitleFont = new System.Windows.Forms.Label();
            this.lblTitleFontSize = new System.Windows.Forms.Label();
            this.lblContentFont = new System.Windows.Forms.Label();
            this.lblContentFontSize = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // txtTruncatedContentLength
            //
            this.txtTruncatedContentLength.Location = new System.Drawing.Point(150, 20);
            this.txtTruncatedContentLength.Name = "txtTruncatedContentLength";
            this.txtTruncatedContentLength.Size = new System.Drawing.Size(100, 20);
            this.txtTruncatedContentLength.TabIndex = 0;
            //
            // txtTitleFont
            //
            this.txtTitleFont.Location = new System.Drawing.Point(150, 50);
            this.txtTitleFont.Name = "txtTitleFont";
            this.txtTitleFont.Size = new System.Drawing.Size(100, 20);
            this.txtTitleFont.TabIndex = 1;
            //
            // txtTitleFontSize
            //
            this.txtTitleFontSize.Location = new System.Drawing.Point(150, 80);
            this.txtTitleFontSize.Name = "txtTitleFontSize";
            this.txtTitleFontSize.Size = new System.Drawing.Size(100, 20);
            this.txtTitleFontSize.TabIndex = 2;
            //
            // txtContentFont
            //
            this.txtContentFont.Location = new System.Drawing.Point(150, 110);
            this.txtContentFont.Name = "txtContentFont";
            this.txtContentFont.Size = new System.Drawing.Size(100, 20);
            this.txtContentFont.TabIndex = 3;
            //
            // txtContentFontSize
            //
            this.txtContentFontSize.Location = new System.Drawing.Point(150, 140);
            this.txtContentFontSize.Name = "txtContentFontSize";
            this.txtContentFontSize.Size = new System.Drawing.Size(100, 20);
            this.txtContentFontSize.TabIndex = 4;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(150, 170);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            //
            // btnCheckForUpdates
            //
            this.btnCheckForUpdates.Location = new System.Drawing.Point(150, 200);
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            this.btnCheckForUpdates.Size = new System.Drawing.Size(100, 23);
            this.btnCheckForUpdates.TabIndex = 6;
            this.btnCheckForUpdates.Text = "Check for Updates";
            this.btnCheckForUpdates.UseVisualStyleBackColor = true;
            this.btnCheckForUpdates.Click += new System.EventHandler(this.BtnCheckForUpdates_Click);
            //
            // lblTruncatedContentLength
            //
            this.lblTruncatedContentLength.AutoSize = true;
            this.lblTruncatedContentLength.Location = new System.Drawing.Point(20, 23);
            this.lblTruncatedContentLength.Name = "lblTruncatedContentLength";
            this.lblTruncatedContentLength.Size = new System.Drawing.Size(124, 13);
            this.lblTruncatedContentLength.TabIndex = 7;
            this.lblTruncatedContentLength.Text = "Truncated Content Length:";
            //
            // lblTitleFont
            //
            this.lblTitleFont.AutoSize = true;
            this.lblTitleFont.Location = new System.Drawing.Point(20, 53);
            this.lblTitleFont.Name = "lblTitleFont";
            this.lblTitleFont.Size = new System.Drawing.Size(58, 13);
            this.lblTitleFont.TabIndex = 8;
            this.lblTitleFont.Text = "Title Font:";
            //
            // lblTitleFontSize
            //
            this.lblTitleFontSize.AutoSize = true;
            this.lblTitleFontSize.Location = new System.Drawing.Point(20, 83);
            this.lblTitleFontSize.Name = "lblTitleFontSize";
            this.lblTitleFontSize.Size = new System.Drawing.Size(71, 13);
            this.lblTitleFontSize.TabIndex = 9;
            this.lblTitleFontSize.Text = "Title Font Size:";
            //
            // lblContentFont
            //
            this.lblContentFont.AutoSize = true;
            this.lblContentFont.Location = new System.Drawing.Point(20, 113);
            this.lblContentFont.Name = "lblContentFont";
            this.lblContentFont.Size = new System.Drawing.Size(65, 13);
            this.lblContentFont.TabIndex = 10;
            this.lblContentFont.Text = "Content Font:";
            //
            // lblContentFontSize
            //
            this.lblContentFontSize.AutoSize = true;
            this.lblContentFontSize.Location = new System.Drawing.Point(20, 143);
            this.lblContentFontSize.Name = "lblContentFontSize";
            this.lblContentFontSize.Size = new System.Drawing.Size(84, 13);
            this.lblContentFontSize.TabIndex = 11;
            this.lblContentFontSize.Text = "Content Font Size:";
            //
            // SettingsForm
            //
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.lblContentFontSize);
            this.Controls.Add(this.lblContentFont);
            this.Controls.Add(this.lblTitleFontSize);
            this.Controls.Add(this.lblTitleFont);
            this.Controls.Add(this.lblTruncatedContentLength);
            this.Controls.Add(this.btnCheckForUpdates);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtContentFontSize);
            this.Controls.Add(this.txtContentFont);
            this.Controls.Add(this.txtTitleFontSize);
            this.Controls.Add(this.txtTitleFont);
            this.Controls.Add(this.txtTruncatedContentLength);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
