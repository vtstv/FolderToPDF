namespace FolderToPDF
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button aboutButton;
        private Button btnGenerateTxt;
        private TextBox txtOutputPathTxt;
        private Label lblOutputPathTxt;
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
        private Label label1;
        private Button btnDarkMode;
        private Button btnSettings;
        private TextBox txtIncludeFiles;
        private Label lblIncludeFiles;
        private Button btnShowFolder;
        private Button btnProfiles;

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
            label1 = new Label();
            btnDarkMode = new Button();
            btnSettings = new Button();
            txtIncludeFiles = new TextBox();
            lblIncludeFiles = new Label();
            btnShowFolder = new Button();
            btnProfiles = new Button();
            label2 = new Label();
            SuspendLayout();
            // 
            // txtDirectory
            // 
            txtDirectory.AllowDrop = true;
            txtDirectory.Location = new Point(120, 12);
            txtDirectory.Name = "txtDirectory";
            txtDirectory.Size = new Size(350, 27);
            txtDirectory.TabIndex = 5;
            txtDirectory.DragDrop += txtDirectory_DragDrop;
            txtDirectory.DragEnter += txtDirectory_DragEnter;
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
            txtOutputPath.TextChanged += txtOutputPath_TextChanged;
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
            btnBrowse.Location = new Point(480, 12);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(94, 27);
            btnBrowse.TabIndex = 6;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += BtnBrowse_Click;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(119, 232);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(133, 30);
            btnGenerate.TabIndex = 11;
            btnGenerate.Text = "Generate PDF";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += BtnGenerate_Click;
            // 
            // lblDirectory
            // 
            lblDirectory.AutoSize = true;
            lblDirectory.Location = new Point(10, 15);
            lblDirectory.Name = "lblDirectory";
            lblDirectory.Size = new Size(73, 20);
            lblDirectory.TabIndex = 0;
            lblDirectory.Text = "Directory:";
            // 
            // lblFileTypes
            // 
            lblFileTypes.AutoSize = true;
            lblFileTypes.Location = new Point(10, 45);
            lblFileTypes.Name = "lblFileTypes";
            lblFileTypes.Size = new Size(76, 20);
            lblFileTypes.TabIndex = 0;
            lblFileTypes.Text = "File Types:";
            // 
            // lblOutputPath
            // 
            lblOutputPath.AutoSize = true;
            lblOutputPath.Location = new Point(10, 75);
            lblOutputPath.Name = "lblOutputPath";
            lblOutputPath.Size = new Size(88, 20);
            lblOutputPath.TabIndex = 0;
            lblOutputPath.Text = "Output PDF:";
            // 
            // lblExcludeFolders
            // 
            lblExcludeFolders.AutoSize = true;
            lblExcludeFolders.Location = new Point(10, 134);
            lblExcludeFolders.Name = "lblExcludeFolders";
            lblExcludeFolders.Size = new Size(96, 20);
            lblExcludeFolders.TabIndex = 0;
            lblExcludeFolders.Text = "Exclude Folders:";
            // 
            // lblExcludeFiles
            // 
            lblExcludeFiles.AutoSize = true;
            lblExcludeFiles.Location = new Point(10, 164);
            lblExcludeFiles.Name = "lblExcludeFiles";
            lblExcludeFiles.Size = new Size(96, 20);
            lblExcludeFiles.TabIndex = 0;
            lblExcludeFiles.Text = "Exclude Files:";
            // 
            // aboutButton
            // 
            aboutButton.Location = new Point(18, 232);
            aboutButton.Name = "aboutButton";
            aboutButton.Size = new Size(65, 31);
            aboutButton.TabIndex = 12;
            aboutButton.Text = "About";
            aboutButton.UseVisualStyleBackColor = true;
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
            txtOutputPathTxt.TextChanged += txtOutputPathTxt_TextChanged;
            // 
            // btnGenerateTxt
            // 
            btnGenerateTxt.Location = new Point(258, 232);
            btnGenerateTxt.Name = "btnGenerateTxt";
            btnGenerateTxt.Size = new Size(72, 30);
            btnGenerateTxt.TabIndex = 0;
            btnGenerateTxt.Text = "TXT";
            btnGenerateTxt.UseVisualStyleBackColor = true;
            btnGenerateTxt.Click += BtnGenerateTxt_Click;
            // 
            // chkRemoveComments
            // 
            chkRemoveComments.Location = new Point(480, 79);
            chkRemoveComments.Name = "chkRemoveComments";
            chkRemoveComments.Size = new Size(100, 20);
            chkRemoveComments.TabIndex = 0;
            chkRemoveComments.Text = "Comments";
            chkRemoveComments.UseVisualStyleBackColor = true;
            // 
            // chkReplaceSensitiveInfo
            // 
            chkReplaceSensitiveInfo.Location = new Point(480, 102);
            chkReplaceSensitiveInfo.Name = "chkReplaceSensitiveInfo";
            chkReplaceSensitiveInfo.Size = new Size(100, 20);
            chkReplaceSensitiveInfo.TabIndex = 1;
            chkReplaceSensitiveInfo.Text = "Sensitive";
            chkReplaceSensitiveInfo.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Arial", 10F);
            label1.Location = new Point(497, 55);
            label1.Name = "label1";
            label1.Size = new Size(51, 19);
            label1.TabIndex = 14;
            label1.Text = "Clean";
            // 
            // btnDarkMode
            // 
            btnDarkMode.Location = new Point(480, 232);
            btnDarkMode.Name = "btnDarkMode";
            btnDarkMode.Size = new Size(94, 30);
            btnDarkMode.TabIndex = 14;
            btnDarkMode.Text = "Theme";
            btnDarkMode.UseVisualStyleBackColor = true;
            btnDarkMode.Click += BtnDarkMode_Click;
            // 
            // btnSettings
            // 
            btnSettings.Location = new Point(480, 202);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(94, 30);
            btnSettings.TabIndex = 15;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += BtnSettings_Click;
            // 
            // txtIncludeFiles
            // 
            txtIncludeFiles.Location = new Point(120, 191);
            txtIncludeFiles.Name = "txtIncludeFiles";
            txtIncludeFiles.Size = new Size(350, 27);
            txtIncludeFiles.TabIndex = 16;
            // 
            // lblIncludeFiles
            // 
            lblIncludeFiles.AutoSize = true;
            lblIncludeFiles.Location = new Point(10, 191);
            lblIncludeFiles.Name = "lblIncludeFiles";
            lblIncludeFiles.Size = new Size(93, 20);
            lblIncludeFiles.TabIndex = 17;
            lblIncludeFiles.Text = "Include Files:";
            // 
            // btnShowFolder
            // 
            btnShowFolder.Location = new Point(334, 232);
            btnShowFolder.Name = "btnShowFolder";
            btnShowFolder.Size = new Size(147, 30);
            btnShowFolder.TabIndex = 16;
            btnShowFolder.Text = "Output Files";
            btnShowFolder.UseVisualStyleBackColor = true;
            btnShowFolder.Visible = false;
            btnShowFolder.Click += BtnShowFolder_Click;
            // 
            // btnProfiles
            // 
            btnProfiles.Location = new Point(480, 154);
            btnProfiles.Name = "btnProfiles";
            btnProfiles.Size = new Size(94, 30);
            btnProfiles.TabIndex = 17;
            btnProfiles.Text = "Profiles";
            btnProfiles.UseVisualStyleBackColor = true;
            btnProfiles.Click += BtnProfiles_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 134);
            label2.Name = "label2";
            label2.Size = new Size(92, 20);
            label2.TabIndex = 18;
            label2.Text = "Skip Folders:";
            // 
            // MainForm
            // 
            ClientSize = new Size(589, 274);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(chkReplaceSensitiveInfo);
            Controls.Add(btnProfiles);
            Controls.Add(chkRemoveComments);
            Controls.Add(btnShowFolder);
            Controls.Add(lblIncludeFiles);
            Controls.Add(txtIncludeFiles);
            Controls.Add(btnSettings);
            Controls.Add(btnDarkMode);
            Controls.Add(lblExcludeFiles);
            Controls.Add(txtExcludeFiles);
            Controls.Add(txtExcludeFolders);
            Controls.Add(lblDirectory);
            Controls.Add(txtDirectory);
            Controls.Add(btnBrowse);
            Controls.Add(lblFileTypes);
            Controls.Add(txtFileTypes);
            Controls.Add(lblOutputPath);
            Controls.Add(txtOutputPath);
            Controls.Add(lblOutputPathTxt);
            Controls.Add(txtOutputPathTxt);
            Controls.Add(btnGenerateTxt);
            Controls.Add(btnGenerate);
            Controls.Add(aboutButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Directory to PDF Converter";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label label2;
    }
}
