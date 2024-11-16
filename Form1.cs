//Form1.cs
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Newtonsoft.Json;
using System.Text;

namespace FolderToPDF
{
    public partial class MainForm : Form
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

        private const string SETTINGS_FILE = "settings.json";
        private string directoryPath = string.Empty;
        private List<string> fileTypes = new List<string> { "py", "css", "html" };

        public MainForm()
        {
            InitializeComponent();
            SetupForm();
            LoadSettings();

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
            }
            base.Dispose(disposing);
        }

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
            SuspendLayout();
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
            lblOutputPath.Size = new Size(120, 20);
            lblOutputPath.TabIndex = 9;
            lblOutputPath.Text = "Output PDF Path:";
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
            lblOutputPathTxt.Size = new Size(119, 20);
            lblOutputPathTxt.TabIndex = 0;
            lblOutputPathTxt.Text = "Output TXT Path:";
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
            // MainForm
            // 
            ClientSize = new Size(582, 242);
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
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetupForm()
        {
            AllowDrop = true;
            DragEnter += new DragEventHandler(MainForm_DragEnter);
            DragDrop += new DragEventHandler(MainForm_DragDrop);
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SETTINGS_FILE))
                {
                    var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_FILE));
                    if (settings != null)
                    {
                        txtDirectory.Text = settings.DirectoryPath;
                        txtFileTypes.Text = settings.FileTypes != null ? string.Join(", ", settings.FileTypes) : string.Empty;
                        txtExcludeFolders.Text = settings.ExcludeFolders != null ? string.Join(", ", settings.ExcludeFolders) : string.Empty;
                        txtExcludeFiles.Text = settings.ExcludeFiles != null ? string.Join(", ", settings.ExcludeFiles) : string.Empty;
                        txtOutputPathTxt.Text = settings.OutputPathTxt;
                        directoryPath = settings.DirectoryPath;
                        fileTypes = settings.FileTypes ?? new List<string> { "py", "css", "html" };
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void SaveSettings()
        {
            try
            {
                var settings = new Settings
                {
                    DirectoryPath = txtDirectory.Text,
                    FileTypes = txtFileTypes.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    ExcludeFolders = txtExcludeFolders.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    ExcludeFiles = txtExcludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    OutputPathTxt = txtOutputPathTxt.Text
                };
                File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    directoryPath = dialog.SelectedPath;
                    txtDirectory.Text = directoryPath;
                    SetDefaultOutputFilename();
                }
            }
        }

        private void SetDefaultOutputFilename()
        {
            if (!string.IsNullOrEmpty(directoryPath))
            {
                string folderName = new DirectoryInfo(directoryPath).Name;
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



                fileTypes = types.ToList();
                SaveSettings();

                var contents = GetDirectoryContents(directoryPath, fileTypes, excludeFolders, excludeFiles);
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

                fileTypes = types.ToList();
                SaveSettings();

                var contents = GetDirectoryContents(directoryPath, fileTypes, excludeFolders, excludeFiles);
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

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
                var contentFont = FontFactory.GetFont(FontFactory.COURIER, 6);

                foreach (var (filePath, fileName, content) in contents)
                {
                    document.Add(new Paragraph($"File Path: {filePath}", titleFont));

                    var line = new LineSeparator();
                    document.Add(line);
                    document.Add(Chunk.NEWLINE);

                    // Limit content length to prevent memory issues
                    var truncatedContent = content.Length > 100000 ? content.Substring(0, 100000) : content;
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
                    writer.WriteLine(new string('-', 80)); // Separator line
                    writer.WriteLine();

                    // Limit content length to prevent memory issues
                    var truncatedContent = content.Length > 100000 ? content.Substring(0, 100000) + "\n[Content truncated...]" : content;
                    writer.WriteLine(truncatedContent);
                    writer.WriteLine();
                    writer.WriteLine(new string('=', 80)); // File separator
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
                    directoryPath = files[0];
                    txtDirectory.Text = directoryPath;
                    SetDefaultOutputFilename();
                }
            }
        }

        private Label lblExcludeFolders;
        private Label lblExcludeFiles;

        private void lblOutputPathTxt_Click(object sender, EventArgs e)
        {

        }
    }

    public class Settings
    {
        public string DirectoryPath { get; set; }
        public List<string> FileTypes { get; set; }
        public List<string> ExcludeFolders { get; set; }
        public List<string> ExcludeFiles { get; set; }
        public string OutputPathTxt { get; set; }
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