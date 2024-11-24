using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Newtonsoft.Json;
using SharpToken;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FolderToPDF
{
    public partial class MainForm : Form
    {
        private SettingsManager settingsManager;
        private ProfileManager profileManager;
        private string outputDirectory;
        private TokenAnalysis lastTokenAnalysis;
        private readonly GptEncoding _encoder;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                _encoder = GptEncoding.GetEncoding("cl100k_base");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing token encoder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            settingsManager = new SettingsManager();
            profileManager = new ProfileManager();
            SetupForm();
            ApplySettings();
            ApplyTheme();

            // Initialize token controls
            InitializeTokenControls();
        }

        private void LogToFile(string message)
        {
            try
            {
                File.AppendAllText("debug_log.txt", $"{DateTime.Now}: {message}\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing to log file: {ex.Message}", "Logging Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (txtDirectory != null) txtDirectory.Dispose();
                if (txtFileTypes != null) txtFileTypes.Dispose();
                if (txtOutputPath != null) txtOutputPath.Dispose();
                if (txtOutputPathTxt != null) txtOutputPathTxt.Dispose();
                if (btnBrowse != null) btnBrowse.Dispose();
                if (btnGenerate != null) btnGenerate.Dispose();
                if (btnGenerateTxt != null) btnGenerateTxt.Dispose();
                if (lblDirectory != null) lblDirectory.Dispose();
                if (lblFileTypes != null) lblFileTypes.Dispose();
                if (lblOutputPath != null) lblOutputPath.Dispose();
                if (lblExcludeFolders != null) lblExcludeFolders.Dispose();
                if (lblExcludeFiles != null) lblExcludeFiles.Dispose();
                if (aboutButton != null) aboutButton.Dispose();
                if (chkRemoveComments != null) chkRemoveComments.Dispose();
                if (chkReplaceSensitiveInfo != null) chkReplaceSensitiveInfo.Dispose();
                if (btnDarkMode != null) btnDarkMode.Dispose();
                if (btnSettings != null) btnSettings.Dispose();
                if (txtIncludeFiles != null) txtIncludeFiles.Dispose();
                if (lblIncludeFiles != null) lblIncludeFiles.Dispose();
                if (btnShowFolder != null) btnShowFolder.Dispose();
                if (btnProfiles != null) btnProfiles.Dispose();
                if (lblTotalLines != null) lblTotalLines.Dispose();
                if (lblTotalTokens != null) lblTotalTokens.Dispose();
                if (lblMaxTokenLength != null) lblMaxTokenLength.Dispose();
                if (btnShowTokenDetails != null) btnShowTokenDetails.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            SaveSettings();
        }

        private void SetupForm()
        {
            AllowDrop = true;
            DragEnter += new DragEventHandler(MainForm_DragEnter);
            DragDrop += new DragEventHandler(MainForm_DragDrop);
        }

        private void ApplySettings()
        {
            var settings = settingsManager.Settings;
            txtDirectory.Text = settings.DirectoryPath;
            txtFileTypes.Text = string.Join(", ", settings.FileTypes);
            txtExcludeFolders.Text = string.Join(",", settings.ExcludeFolders);
            txtExcludeFiles.Text = string.Join(",", settings.ExcludeFiles); 
            txtOutputPathTxt.Text = settings.OutputPathTxt;
            txtOutputPath.Text = settings.OutputPathPdf;
            chkRemoveComments.Checked = settings.RemoveComments;
            chkReplaceSensitiveInfo.Checked = settings.ReplaceSensitiveInfo;
            txtIncludeFiles.Text = string.Join(",", settings.IncludeFiles); 
        }



        private void SaveSettings()
        {
            var settings = settingsManager.Settings;
            settings.DirectoryPath = txtDirectory.Text.Trim();
            settings.FileTypes = txtFileTypes.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(type => type.Trim())
                                             .ToList();
            settings.ExcludeFolders = txtExcludeFolders.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                                       .Select(folder => folder.Trim())
                                                       .ToList();
            settings.ExcludeFiles = txtExcludeFiles.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(file => file.Trim())
                                                   .ToList();
            settings.IncludeFiles = txtIncludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(file => file.Trim())
                                                   .ToList();
            settings.OutputPathTxt = txtOutputPathTxt.Text.Trim();
            settings.OutputPathPdf = txtOutputPath.Text.Trim();
            settings.RemoveComments = chkRemoveComments.Checked;
            settings.ReplaceSensitiveInfo = chkReplaceSensitiveInfo.Checked;

            if (!settings.FileTypes.Any())
            {
                MessageBox.Show("Please specify at least one file type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var types = txtFileTypes.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (!types.Any())
            {
                string folderName = new DirectoryInfo(settingsManager.Settings.DirectoryPath).Name;
                txtOutputPath.Text = $"{folderName}_contents.pdf";
                txtOutputPathTxt.Text = $"{folderName}_contents.txt";
            }

            settingsManager.SaveSettings();
        }




        private void AboutButton_Click(object sender, EventArgs e)
        {
            using (var aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog(this);
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDirectory.Text))
                {
                    MessageBox.Show("Please select a directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var types = txtFileTypes.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (!types.Any())
                {
                    MessageBox.Show("Please specify at least one file type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var excludeFolders = txtExcludeFolders.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var excludeFiles = txtExcludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var includeFiles = txtIncludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                settingsManager.Settings.FileTypes = types.ToList();
                settingsManager.Settings.IncludeFiles = includeFiles;
                SaveSettings();

                var contents = GetDirectoryContents(settingsManager.Settings.DirectoryPath, settingsManager.Settings.FileTypes, excludeFolders, excludeFiles, includeFiles);
                if (!contents.Any())
                {
                    MessageBox.Show("No matching files found in the directory.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string outputPath = txtOutputPath.Text;
                if (string.IsNullOrEmpty(Path.GetDirectoryName(outputPath)))
                {
                    outputPath = Path.Combine("output", Path.GetFileName(outputPath));
                }

                outputDirectory = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                CreatePDFWithContents(contents, outputPath);
                MessageBox.Show($"PDF created: {outputPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnShowFolder.Visible = true;

                // Calculate and display total lines and tokens
                var (totalLines, totalTokens) = TokensAndLines.Calculate(contents);
                lblTotalLines.Text = $"Lines: {totalLines}";
                lblTotalTokens.Text = $"Tokens: {totalTokens}";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGenerateTxt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDirectory.Text))
                {
                    MessageBox.Show("Please select a directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var types = txtFileTypes.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (!types.Any())
                {
                    MessageBox.Show("Please specify at least one file type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var excludeFolders = txtExcludeFolders.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var excludeFiles = txtExcludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var includeFiles = txtIncludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                settingsManager.Settings.FileTypes = types.ToList();
                settingsManager.Settings.IncludeFiles = includeFiles;
                SaveSettings();

                var contents = GetDirectoryContents(settingsManager.Settings.DirectoryPath, settingsManager.Settings.FileTypes, excludeFolders, excludeFiles, includeFiles);
                if (!contents.Any())
                {
                    MessageBox.Show("No matching files found in the directory.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string outputPath = txtOutputPathTxt.Text;
                if (string.IsNullOrEmpty(Path.GetDirectoryName(outputPath)))
                {
                    outputPath = Path.Combine("output", Path.GetFileName(outputPath));
                }

                outputDirectory = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                CreateTxtWithContents(contents, outputPath);
                MessageBox.Show($"TXT file created: {outputPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnShowFolder.Visible = true;

                // Calculate and display total lines and tokens
                var (totalLines, totalTokens) = TokensAndLines.Calculate(contents);
                lblTotalLines.Text = $"Lines: {totalLines}";
                lblTotalTokens.Text = $"Tokens: {totalTokens}";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating TXT file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTokenDisplay(int totalLines, int totalTokens, TokenAnalysis analysis)
        {
            lblTotalLines.Text = $"Total Lines: {totalLines:N0}";
            lblTotalTokens.Text = $"Total Tokens: {totalTokens:N0}";
            lblMaxTokenLength.Text = $"Max Token Length: {analysis.MaxTokenLength:N0} characters";

            // Make the details button visible if there are large tokens
            btnShowTokenDetails.Visible = analysis.LargeTokens.Any();
        }

        private void BtnShowTokenDetails_Click(object sender, EventArgs e)
        {
            if (lastTokenAnalysis?.LargeTokens == null || !lastTokenAnalysis.LargeTokens.Any())
            {
                MessageBox.Show("No token details to display.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var detailsForm = new Form
            {
                Text = "Token Analysis Details",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent
            };

            var textBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                ReadOnly = true
            };

            var details = new StringBuilder();
            details.AppendLine("Large Tokens Found (>10 characters):");
            details.AppendLine();

            foreach (var token in lastTokenAnalysis.LargeTokens)
            {
                details.AppendLine($"Length: {token.Length,4} | Token: {token}");
            }

            textBox.Text = details.ToString();
            detailsForm.Controls.Add(textBox);
            detailsForm.ShowDialog();
        }

        private void InitializeTokenControls()
        {
            
            if (lblTotalLines == null)
            {
                lblTotalLines = new Label
                {
                    Location = new Point(12, 50),
                    AutoSize = true
                };
                this.Controls.Add(lblTotalLines);
            }

            if (lblTotalTokens == null)
            {
                lblTotalTokens = new Label
                {
                    Location = new Point(12, 70),
                    AutoSize = true
                };
                this.Controls.Add(lblTotalTokens);
            }

            if (lblMaxTokenLength == null)
            {
                lblMaxTokenLength = new Label
                {
                    Location = new Point(12, 80),
                    AutoSize = true
                };
                this.Controls.Add(lblMaxTokenLength);
            }

            if (btnShowTokenDetails == null)
            {
                btnShowTokenDetails = new Button
                {
                    Text = "Show Token Details",
                    Location = new Point(),
                    Visible = false
                };
                btnShowTokenDetails.Click += BtnShowTokenDetails_Click;
                this.Controls.Add(btnShowTokenDetails);
            }
        }

        private List<(string FilePath, string FileName, string Content)> GetDirectoryContents(
    string dirPath,
    List<string> types,
    List<string> excludeFolders = null,
    List<string> excludeFiles = null,
    List<string> includeFiles = null)
        {
            LogToFile($"Starting GetDirectoryContents for path: {dirPath}");
            var contents = new List<(string, string, string)>();

            try
            {
                var rootFiles = Directory.GetFiles(dirPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => types.Any(t => f.EndsWith($".{t}", StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                LogToFile($"Root Files Before Exclusion: {string.Join(", ", rootFiles)}");

                if (excludeFiles != null && excludeFiles.Any())
                {
                    rootFiles = rootFiles.Where(f => !excludeFiles.Any(mask =>
                    {
                        string fileName = Path.GetFileName(f);
                        return mask.Contains("*")
                            ? fileName.StartsWith(mask.TrimEnd('*'))
                            : fileName.Equals(mask, StringComparison.OrdinalIgnoreCase);
                    })).ToList();
                }

                if (includeFiles != null && includeFiles.Any())
                {
                    rootFiles = rootFiles.Where(f => includeFiles.Any(mask =>
                    {
                        string fileName = Path.GetFileName(f);
                        return mask.Contains("*")
                            ? fileName.StartsWith(mask.TrimEnd('*'))
                            : fileName.Equals(mask, StringComparison.OrdinalIgnoreCase);
                    })).ToList();
                }

                LogToFile($"Root Files After Exclusion: {string.Join(", ", rootFiles)}");

                foreach (var file in rootFiles)
                {
                    try
                    {
                        var content = File.ReadAllText(file, Encoding.UTF8);
                        content = FileCleaner.CleanContent(
                            content,
                            chkRemoveComments.Checked,
                            chkReplaceSensitiveInfo.Checked
                        );
                        contents.Add((file, Path.GetFileName(file), content));
                    }
                    catch (Exception ex)
                    {
                        LogToFile($"Error reading file {file}: {ex.Message}");
                    }
                }

                var allDirectories = Directory.GetDirectories(dirPath, "*.*", SearchOption.AllDirectories);
                LogToFile($"All Directories Before Exclusion: {string.Join(", ", allDirectories)}");

                var filteredDirectories = allDirectories.Where(d =>
                    excludeFolders == null || !excludeFolders.Any(ex => d.Contains(ex, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                LogToFile($"Filtered Directories: {string.Join(", ", filteredDirectories)}");

                foreach (var directory in filteredDirectories)
                {
                    var files = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => types.Any(t => f.EndsWith($".{t}", StringComparison.OrdinalIgnoreCase)))
                        .ToList();

                    LogToFile($"Files in Directory '{directory}' Before Exclusion: {string.Join(", ", files)}");

                    if (excludeFiles != null && excludeFiles.Any())
                    {
                        files = files.Where(f => !excludeFiles.Any(mask =>
                        {
                            string fileName = Path.GetFileName(f);
                            return mask.Contains("*")
                                ? fileName.StartsWith(mask.TrimEnd('*'))
                                : fileName.Equals(mask, StringComparison.OrdinalIgnoreCase);
                        })).ToList();
                    }

                    if (includeFiles != null && includeFiles.Any())
                    {
                        files = files.Where(f => includeFiles.Any(mask =>
                        {
                            string fileName = Path.GetFileName(f);
                            return mask.Contains("*")
                                ? fileName.StartsWith(mask.TrimEnd('*'))
                                : fileName.Equals(mask, StringComparison.OrdinalIgnoreCase);
                        })).ToList();
                    }

                    LogToFile($"Files in Directory '{directory}' After Exclusion: {string.Join(", ", files)}");

                    foreach (var file in files)
                    {
                        try
                        {
                            var content = File.ReadAllText(file, Encoding.UTF8);
                            content = FileCleaner.CleanContent(
                                content,
                                chkRemoveComments.Checked,
                                chkReplaceSensitiveInfo.Checked
                            );
                            contents.Add((file, Path.GetFileName(file), content));
                        }
                        catch (Exception ex)
                        {
                            LogToFile($"Error reading file {file}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogToFile($"Error accessing directory: {ex.Message}");
            }

            return contents;
        }


        private void CreatePDFWithContents(List<(string FilePath, string FileName, string Content)> contents, string outputPath)
        {
            using (var fs = new FileStream(outputPath, FileMode.Create))
            using (var document = new Document())
            {
                PdfWriter.GetInstance(document, fs);
                document.Open();

                var titleFont = FontFactory.GetFont(settingsManager.Settings.TitleFont, settingsManager.Settings.TitleFontSize);
                var contentFont = FontFactory.GetFont(settingsManager.Settings.ContentFont, settingsManager.Settings.ContentFontSize);

                foreach (var (filePath, fileName, content) in contents)
                {
                    document.Add(new Paragraph($"File Path: {filePath}", titleFont));
                    document.Add(Chunk.NEWLINE);

                    var truncatedContent = content.Length > settingsManager.Settings.TruncatedContentLength
                        ? content.Substring(0, settingsManager.Settings.TruncatedContentLength)
                        : content;

                    document.Add(new Paragraph(truncatedContent, contentFont));
                    document.Add(Chunk.NEWLINE);
                }

                document.Close();
            }
        }

        private void CreateTxtWithContents(List<(string FilePath, string FileName, string Content)> contents, string outputPath)
        {
            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                foreach (var (filePath, fileName, content) in contents)
                {
                    writer.WriteLine($"File Path: {filePath}");
                    writer.WriteLine();

                    var truncatedContent = content.Length > settingsManager.Settings.TruncatedContentLength
                        ? content.Substring(0, settingsManager.Settings.TruncatedContentLength) + "\n{Content truncated...}"
                        : content;

                    writer.WriteLine(truncatedContent);
                    writer.WriteLine();
                    writer.WriteLine();
                }
            }
        }

        private void txtDirectory_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void txtDirectory_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
        }

        private void HandleDragDrop(DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                if (Directory.Exists(files[0]))
                {
                    settingsManager.Settings.DirectoryPath = files[0];
                    txtDirectory.Text = settingsManager.Settings.DirectoryPath;
                    SetDefaultOutputFilename();
                }
            }
        }

        private void SetDefaultOutputFilename()
        {
            string folderName = new DirectoryInfo(settingsManager.Settings.DirectoryPath).Name;
            txtOutputPath.Text = $"{folderName}_contents.pdf";
            txtOutputPathTxt.Text = $"{folderName}_contents.txt";
        }

        private void BtnShowFolder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(outputDirectory))
            {
                MessageBox.Show("Output directory path is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(outputDirectory))
            {
                MessageBox.Show("Output directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Process.Start("explorer.exe", outputDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening output directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(this, settingsManager.Settings.DarkMode);
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(settingsManager))
            {
                settingsForm.StartPosition = FormStartPosition.CenterParent;
                settingsForm.ShowDialog(this);
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDirectory.Text = folderBrowserDialog.SelectedPath;
                    settingsManager.Settings.DirectoryPath = folderBrowserDialog.SelectedPath;
                    SetDefaultOutputFilename();
                }
            }
        }

        private void BtnDarkMode_Click(object sender, EventArgs e)
        {
            settingsManager.Settings.DarkMode = !settingsManager.Settings.DarkMode;
            ApplyTheme();
            settingsManager.SaveSettings();
        }

        private void txtOutputPath_TextChanged(object sender, EventArgs e)
        {
            outputDirectory = Path.GetDirectoryName(txtOutputPath.Text);
        }

        private void txtOutputPathTxt_TextChanged(object sender, EventArgs e)
        {
            outputDirectory = Path.GetDirectoryName(txtOutputPathTxt.Text);
        }

        private void BtnProfiles_Click(object sender, EventArgs e)
        {
            using (var profilesForm = new ProfilesForm(profileManager, this))
            {
                profilesForm.StartPosition = FormStartPosition.CenterParent;
                profilesForm.ShowDialog(this);
            }
        }

        public void LoadProfile(Profile profile)
        {
            txtDirectory.Text = profile.DirectoryPath;
            txtFileTypes.Text = string.Join(", ", profile.FileTypes);
            txtOutputPath.Text = profile.OutputPathPdf;
            txtOutputPathTxt.Text = profile.OutputPathTxt;
            txtExcludeFolders.Text = string.Join(", ", profile.ExcludeFolders);
            txtExcludeFiles.Text = string.Join(", ", profile.ExcludeFiles);
            txtIncludeFiles.Text = string.Join(", ", profile.IncludeFiles);
        }

        public Profile GetCurrentProfile()
        {
            return new Profile
            {
                DirectoryPath = txtDirectory.Text,
                FileTypes = txtFileTypes.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                OutputPathPdf = txtOutputPath.Text,
                OutputPathTxt = txtOutputPathTxt.Text,
                ExcludeFolders = txtExcludeFolders.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                ExcludeFiles = txtExcludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                IncludeFiles = txtIncludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            };
        }

        private void lblOutputPathTxt_Click(object sender, EventArgs e)
        {

        }
    }
}
