namespace FolderToPDF
{
    partial class ProfilesForm
    {
        private System.ComponentModel.IContainer components = null;
        private ListBox lstProfiles;
        private TextBox txtProfileName;
        private Button btnSaveProfile;
        private Button btnLoadProfile;
        private Button btnDeleteProfile;

        private void InitializeComponent()
        {
            lstProfiles = new ListBox();
            txtProfileName = new TextBox();
            btnSaveProfile = new Button();
            btnLoadProfile = new Button();
            btnDeleteProfile = new Button();
            SuspendLayout();
            // 
            // lstProfiles
            // 
            lstProfiles.FormattingEnabled = true;
            lstProfiles.Location = new Point(12, 12);
            lstProfiles.Name = "lstProfiles";
            lstProfiles.Size = new Size(260, 184);
            lstProfiles.TabIndex = 0;
            // 
            // txtProfileName
            // 
            txtProfileName.Location = new Point(12, 217);
            txtProfileName.Name = "txtProfileName";
            txtProfileName.Size = new Size(260, 27);
            txtProfileName.TabIndex = 1;
            // 
            // btnSaveProfile
            // 
            btnSaveProfile.Location = new Point(12, 250);
            btnSaveProfile.Name = "btnSaveProfile";
            btnSaveProfile.Size = new Size(75, 23);
            btnSaveProfile.TabIndex = 2;
            btnSaveProfile.Text = "Save Profile";
            btnSaveProfile.UseVisualStyleBackColor = true;
            btnSaveProfile.Click += BtnSaveProfile_Click;
            // 
            // btnLoadProfile
            // 
            btnLoadProfile.Location = new Point(93, 250);
            btnLoadProfile.Name = "btnLoadProfile";
            btnLoadProfile.Size = new Size(75, 23);
            btnLoadProfile.TabIndex = 3;
            btnLoadProfile.Text = "Load Profile";
            btnLoadProfile.UseVisualStyleBackColor = true;
            btnLoadProfile.Click += BtnLoadProfile_Click;
            // 
            // btnDeleteProfile
            // 
            btnDeleteProfile.Location = new Point(174, 250);
            btnDeleteProfile.Name = "btnDeleteProfile";
            btnDeleteProfile.Size = new Size(75, 23);
            btnDeleteProfile.TabIndex = 4;
            btnDeleteProfile.Text = "Delete Profile";
            btnDeleteProfile.UseVisualStyleBackColor = true;
            btnDeleteProfile.Click += BtnDeleteProfile_Click;
            // 
            // ProfilesForm
            // 
            ClientSize = new Size(284, 285);
            Controls.Add(btnDeleteProfile);
            Controls.Add(btnLoadProfile);
            Controls.Add(btnSaveProfile);
            Controls.Add(txtProfileName);
            Controls.Add(lstProfiles);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProfilesForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Profiles";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
