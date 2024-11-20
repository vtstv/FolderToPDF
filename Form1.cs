using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;

namespace FolderToPDF
{
    public partial class MainForm : Form
    {
        private SettingsManager settingsManager;

        public MainForm()
        {
            InitializeComponent();
            settingsManager = new SettingsManager();
            SetupForm();
            ApplySettings();
            ApplyTheme();
        }

        private void LogToFile(string message)
        {
            try
            {
                File.AppendAllText("debug_log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");
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
                if (lblOutputPathTxt != null) lblOutputPathTxt.Dispose();
                if (aboutButton != null) aboutButton.Dispose();
                if (chkRemoveComments != null) chkRemoveComments.Dispose();
                if (chkReplaceSensitiveInfo != null) chkReplaceSensitiveInfo.Dispose();
                if (btnDarkMode != null) btnDarkMode.Dispose();
                if (btnSettings != null) btnSettings.Dispose();
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
            txtExcludeFolders.Text = string.Join(", ", settings.ExcludeFolders);
            txtExcludeFiles.Text = string.Join(", ", settings.ExcludeFiles);
            txtOutputPathTxt.Text = settings.OutputPathTxt;
            txtOutputPath.Text = settings.OutputPathPdf;
            chkRemoveComments.Checked = settings.RemoveComments;
            chkReplaceSensitiveInfo.Checked = settings.ReplaceSensitiveInfo;
        }

        private void SaveSettings()
        {
            var settings = settingsManager.Settings;
            settings.DirectoryPath = txtDirectory.Text;
            settings.FileTypes = txtFileTypes.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            settings.ExcludeFolders = txtExcludeFolders.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            settings.ExcludeFiles = txtExcludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            settings.OutputPathTxt = txtOutputPathTxt.Text;
            settings.OutputPathPdf = txtOutputPath.Text;
            settings.RemoveComments = chkRemoveComments.Checked;
            settings.ReplaceSensitiveInfo = chkReplaceSensitiveInfo.Checked;
            settingsManager.SaveSettings();
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    settingsManager.Settings.DirectoryPath = dialog.SelectedPath;
                    txtDirectory.Text = settingsManager.Settings.DirectoryPath;
                    SetDefaultOutputFilename();
                }
            }
        }

        private void SetDefaultOutputFilename()
        {
            if (!string.IsNullOrEmpty(settingsManager.Settings.DirectoryPath))
            {
                string folderName = new DirectoryInfo(settingsManager.Settings.DirectoryPath).Name;
                txtOutputPath.Text = $"{folderName}_contents.pdf";
                txtOutputPathTxt.Text = $"{folderName}_contents.txt";
            }
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

                var excludeFolders = txtExcludeFolders.Text
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var excludeFiles = txtExcludeFiles.Text
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                settingsManager.Settings.FileTypes = types.ToList();
                SaveSettings();

                var contents = GetDirectoryContents(settingsManager.Settings.DirectoryPath, settingsManager.Settings.FileTypes, excludeFolders, excludeFiles);
                if (!contents.Any())
                {
                    MessageBox.Show("No matching files found in the directory.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CreatePDFWithContents(contents, txtOutputPath.Text);
                MessageBox.Show($"PDF created: {txtOutputPath.Text}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                var excludeFolders = txtExcludeFolders.Text
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                var excludeFiles = txtExcludeFiles.Text
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                settingsManager.Settings.FileTypes = types.ToList();
                SaveSettings();

                var contents = GetDirectoryContents(settingsManager.Settings.DirectoryPath, settingsManager.Settings.FileTypes, excludeFolders, excludeFiles);
                if (!contents.Any())
                {
                    MessageBox.Show("No matching files found in the directory.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CreateTxtWithContents(contents, txtOutputPathTxt.Text);
                MessageBox.Show($"TXT file created: {txtOutputPathTxt.Text}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating TXT file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<(string FilePath, string FileName, string Content)> GetDirectoryContents(
            string dirPath,
            List<string> types,
            List<string> excludeFolders = null,
            List<string> excludeFiles = null)
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

                var allDirectories = Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories);

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

                    files = files.Where(f => !excludeFiles.Any(mask =>
                    {
                        string fileName = Path.GetFileName(f);
                        return mask.Contains("*")
                            ? fileName.StartsWith(mask.TrimEnd('*'))
                            : fileName.Equals(mask, StringComparison.OrdinalIgnoreCase);
                    })).ToList();

                    LogToFile($"Files in Directory '{directory}' After Exclusion: {string.Join(", ", files)}");

                    foreach (var file in files)
                    {
                        try
                        {
                            var content = File.ReadAllText(file, Encoding.UTF8);
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

                    var line = new LineSeparator();
                    document.Add(line);
                    document.Add(Chunk.NEWLINE);

                    // Limit content length to prevent memory issues
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

                    // Limit content length to prevent memory issues
                    var truncatedContent = content.Length > settingsManager.Settings.TruncatedContentLength
                        ? content.Substring(0, settingsManager.Settings.TruncatedContentLength) + "\n[Content truncated...]"
                        : content;
                    writer.WriteLine(truncatedContent);
                    writer.WriteLine();
                    writer.WriteLine();
                }
            }
        }

        private void TxtDirectory_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void TxtDirectory_DragDrop(object sender, DragEventArgs e)
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

        private void lblOutputPathTxt_Click(object sender, EventArgs e)
        {

        }

        private void BtnDarkMode_Click(object sender, EventArgs e)
        {
            settingsManager.Settings.DarkMode = !settingsManager.Settings.DarkMode;
            ApplyTheme();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog(this);
            }
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyTheme(this, settingsManager.Settings.DarkMode);
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
