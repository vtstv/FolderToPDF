﻿namespace FolderToPDF
{
    partial class MainForm
    {
        private Button aboutButton;
        private Button btnGenerateTxt;
        private TextBox txtOutputPathTxt;
        private Label lblOutputPathTxt;

        // Control declarations
        private TextBox txtDirectory;
        private TextBox txtFileTypes;
        private TextBox txtExcludeFolders;
        private TextBox txtExcludeFiles;
        private TextBox txtOutputPath;
        private Button btnBrowse;
        private Button btnGenerate;
        private Label lblDirectory;
        private Label lblFileTypes;
        private Label lblOutputPath;
        private Label lblExcludeFolders;
        private Label lblExcludeFiles;
        private CheckBox chkRemoveComments;
        private CheckBox chkReplaceSensitiveInfo;
        private Panel panel1;
        private Label label1;
        private Button btnDarkMode; // Add this line
        private Button btnSettings; // Add this line

        private void InitializeComponent()
        {
            txtDirectory = new TextBox();
            txtFileTypes = new TextBox();
            txtOutputPath = new TextBox();
            txtExcludeFolders = new TextBox();
            txtExcludeFiles = new TextBox();
            btnBrowse = new Button();
            btnGenerate = new Button();
            lblDirectory = new Label();
            lblFileTypes = new Label();
            lblOutputPath = new Label();
            lblExcludeFolders = new Label();
            lblExcludeFiles = new Label();
            aboutButton = new Button();
            lblOutputPathTxt = new Label();
            txtOutputPathTxt = new TextBox();
            btnGenerateTxt = new Button();
            chkRemoveComments = new CheckBox();
            chkReplaceSensitiveInfo = new CheckBox();
            panel1 = new Panel();
            label1 = new Label();
            btnDarkMode = new Button(); // Add this line
            btnSettings = new Button(); // Add this line
            panel1.SuspendLayout();
            //
            // txtDirectory
            //
            txtDirectory.AllowDrop = true;
            txtDirectory.Location = new Point(120, 12);
            txtDirectory.Name = "txtDirectory";
            txtDirectory.Size = new Size(350, 27);
            txtDirectory.TabIndex = 5;
            txtDirectory.DragDrop += TxtDirectory_DragDrop;
            txtDirectory.DragEnter += TxtDirectory_DragEnter;
            //
            // txtFileTypes
            //
            txtFileTypes.Location = new Point(120, 42);
            txtFileTypes.Name = "txtFileTypes";
            txtFileTypes.Size = new Size(350, 27);
            txtFileTypes.TabIndex = 8;
            //
            // txtOutputPath
            //
            txtOutputPath.Location = new Point(120, 72);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new Size(350, 27);
            txtOutputPath.TabIndex = 10;
            //
            // txtExcludeFolders
            //
            txtExcludeFolders.Location = new Point(120, 131);
            txtExcludeFolders.Name = "txtExcludeFolders";
            txtExcludeFolders.Size = new Size(350, 27);
            txtExcludeFolders.TabIndex = 1;
            //
            // txtExcludeFiles
            //
            txtExcludeFiles.Location = new Point(120, 161);
            txtExcludeFiles.Name = "txtExcludeFiles";
            txtExcludeFiles.Size = new Size(350, 27);
            txtExcludeFiles.TabIndex = 3;
            //
            // btnBrowse
            //
            btnBrowse.Location = new Point(480, 10);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 6;
            btnBrowse.Text = "Browse";
            btnBrowse.Click += BtnBrowse_Click;
            //
            // btnGenerate
            //
            btnGenerate.Location = new Point(181, 197);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(147, 30);
            btnGenerate.TabIndex = 11;
            btnGenerate.Text = "Generate PDF";
            btnGenerate.Click += BtnGenerate_Click;
            //
            // lblDirectory
            //
            lblDirectory.AutoSize = true;
            lblDirectory.Location = new Point(10, 15);
            lblDirectory.Name = "lblDirectory";
            lblDirectory.Size = new Size(117, 20);
            lblDirectory.TabIndex = 4;
            lblDirectory.Text = "Select Directory:";
            //
            // lblFileTypes
            //
            lblFileTypes.AutoSize = true;
            lblFileTypes.Location = new Point(10, 45);
            lblFileTypes.Name = "lblFileTypes";
            lblFileTypes.Size = new Size(76, 20);
            lblFileTypes.TabIndex = 7;
            lblFileTypes.Text = "File Types:";
            //
            // lblOutputPath
            //
            lblOutputPath.AutoSize = true;
            lblOutputPath.Location = new Point(10, 75);
            lblOutputPath.Name = "lblOutputPath";
            lblOutputPath.Size = new Size(88, 20);
            lblOutputPath.TabIndex = 9;
            lblOutputPath.Text = "Output PDF:";
            //
            // lblExcludeFolders
            //
            lblExcludeFolders.AutoSize = true;
            lblExcludeFolders.Location = new Point(10, 134);
            lblExcludeFolders.Name = "lblExcludeFolders";
            lblExcludeFolders.Size = new Size(115, 20);
            lblExcludeFolders.TabIndex = 0;
            lblExcludeFolders.Text = "Exclude Folders:";
            //
            // lblExcludeFiles
            //
            lblExcludeFiles.AutoSize = true;
            lblExcludeFiles.Location = new Point(10, 164);
            lblExcludeFiles.Name = "lblExcludeFiles";
            lblExcludeFiles.Size = new Size(96, 20);
            lblExcludeFiles.TabIndex = 2;
            lblExcludeFiles.Text = "Exclude Files:";
            //
            // aboutButton
            //
            aboutButton.Location = new Point(18, 197);
            aboutButton.Name = "aboutButton";
            aboutButton.Size = new Size(65, 30);
            aboutButton.TabIndex = 12;
            aboutButton.Text = "About";
            aboutButton.Click += AboutButton_Click;
            //
            // lblOutputPathTxt
            //
            lblOutputPathTxt.AutoSize = true;
            lblOutputPathTxt.Location = new Point(10, 104);
            lblOutputPathTxt.Name = "lblOutputPathTxt";
            lblOutputPathTxt.Size = new Size(87, 20);
            lblOutputPathTxt.TabIndex = 0;
            lblOutputPathTxt.Text = "Output TXT:";
            lblOutputPathTxt.Click += lblOutputPathTxt_Click;
            //
            // txtOutputPathTxt
            //
            txtOutputPathTxt.Location = new Point(120, 101);
            txtOutputPathTxt.Name = "txtOutputPathTxt";
            txtOutputPathTxt.Size = new Size(350, 27);
            txtOutputPathTxt.TabIndex = 0;
            //
            // btnGenerateTxt
            //
            btnGenerateTxt.Location = new Point(337, 197);
            btnGenerateTxt.Name = "btnGenerateTxt";
            btnGenerateTxt.Size = new Size(72, 30);
            btnGenerateTxt.TabIndex = 0;
            btnGenerateTxt.Text = "TXT";
            btnGenerateTxt.Click += BtnGenerateTxt_Click;
            //
            // chkRemoveComments
            //
            chkRemoveComments.Location = new Point(0, 20);
            chkRemoveComments.Name = "chkRemoveComments";
            chkRemoveComments.Size = new Size(100, 20);
            chkRemoveComments.TabIndex = 0;
            chkRemoveComments.Text = "Comments";
            //
            // chkReplaceSensitiveInfo
            //
            chkReplaceSensitiveInfo.Location = new Point(0, 43);
            chkReplaceSensitiveInfo.Name = "chkReplaceSensitiveInfo";
            chkReplaceSensitiveInfo.Size = new Size(100, 20);
            chkReplaceSensitiveInfo.TabIndex = 1;
            chkReplaceSensitiveInfo.Text = "Sensitive";
            //
            // panel1
            //
            panel1.Controls.Add(label1);
            panel1.Controls.Add(chkReplaceSensitiveInfo);
            panel1.Controls.Add(chkRemoveComments);
            panel1.Location = new Point(480, 63);
            panel1.Name = "panel1";
            panel1.Size = new Size(94, 125);
            panel1.TabIndex = 13;
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            label1.Location = new Point(17, -4);
            label1.Name = "label1";
            label1.Size = new Size(72, 23);
            label1.TabIndex = 14;
            label1.Text = "Striping:";
            //
            // btnDarkMode
            //
            btnDarkMode.Location = new Point(480, 200);
            btnDarkMode.Name = "btnDarkMode";
            btnDarkMode.Size = new Size(94, 30);
            btnDarkMode.TabIndex = 14;
            btnDarkMode.Text = "Switch Theme";
            btnDarkMode.UseVisualStyleBackColor = true;
            btnDarkMode.Click += BtnDarkMode_Click; // Add this line
            //
            // btnSettings
            //
            btnSettings.Location = new Point(480, 235);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(94, 30);
            btnSettings.TabIndex = 15;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += BtnSettings_Click; // Add this line
            //
            // MainForm
            //
            ClientSize = new Size(584, 275); // Adjust the height to accommodate new buttons
            Controls.Add(btnSettings); // Add this line
            Controls.Add(btnDarkMode); // Add this line
            Controls.Add(panel1);
            Controls.Add(lblExcludeFolders);
            Controls.Add(txtExcludeFolders);
            Controls.Add(lblExcludeFiles);
            Controls.Add(txtExcludeFiles);
            Controls.Add(lblDirectory);
            Controls.Add(txtDirectory);
            Controls.Add(btnBrowse);
            Controls.Add(lblFileTypes);
            Controls.Add(txtFileTypes);
            Controls.Add(lblOutputPath);
            Controls.Add(txtOutputPath);
            Controls.Add(lblOutputPathTxt);
            Controls.Add(txtOutputPathTxt);
            Controls.Add(btnGenerate);
            Controls.Add(btnGenerateTxt);
            Controls.Add(aboutButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Directory to PDF Converter";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}



