//Form1.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Newtonsoft.Json;

namespace FolderToPDF
{
    public partial class MainForm : Form
    {
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
                if (btnBrowse != null) btnBrowse.Dispose();
                if (btnGenerate != null) btnGenerate.Dispose();
                if (lblDirectory != null) lblDirectory.Dispose();
                if (lblFileTypes != null) lblFileTypes.Dispose();
                if (lblOutputPath != null) lblOutputPath.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Initialize controls
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

            // Labels and TextBoxes for folders to exclude
            Label lblExcludeFolders = new Label();
            lblExcludeFolders.Text = "Exclude Folders:";
            lblExcludeFolders.Location = new Point(10, 105);
            lblExcludeFolders.AutoSize = true;

            txtExcludeFolders.Location = new Point(120, 102);
            txtExcludeFolders.Size = new Size(350, 20);

            // Labels and TextBoxes for files to exclude
            Label lblExcludeFiles = new Label();
            lblExcludeFiles.Text = "Exclude Files:";
            lblExcludeFiles.Location = new Point(10, 135); // Позиция ниже "Exclude Folders"
            lblExcludeFiles.AutoSize = true;

            txtExcludeFiles = new TextBox();
            txtExcludeFiles.Location = new Point(120, 132);
            txtExcludeFiles.Size = new Size(350, 20);

            // Add new controls to form
            Controls.Add(lblExcludeFolders);
            Controls.Add(txtExcludeFolders);
            Controls.Add(lblExcludeFiles);
            Controls.Add(txtExcludeFiles);

            // Form settings
            SuspendLayout();
            Text = "Directory to PDF Converter";
            Size = new Size(600, 250);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            // Directory Label
            lblDirectory.Text = "Select Directory:";
            lblDirectory.Location = new Point(10, 15);
            lblDirectory.AutoSize = true;

            // Directory TextBox
            txtDirectory.Location = new Point(120, 12);
            txtDirectory.Size = new Size(350, 20);
            txtDirectory.AllowDrop = true;
            txtDirectory.DragDrop += new DragEventHandler(TxtDirectory_DragDrop);
            txtDirectory.DragEnter += new DragEventHandler(TxtDirectory_DragEnter);

            // Browse Button
            btnBrowse.Text = "Browse";
            btnBrowse.Location = new Point(480, 10);
            btnBrowse.Click += new EventHandler(BtnBrowse_Click);

            // File Types Label
            lblFileTypes.Text = "File Types:";
            lblFileTypes.Location = new Point(10, 45);
            lblFileTypes.AutoSize = true;

            // File Types TextBox
            txtFileTypes.Location = new Point(120, 42);
            txtFileTypes.Size = new Size(350, 20);

            // Output Path Label
            lblOutputPath.Text = "Output PDF Path:";
            lblOutputPath.Location = new Point(10, 75);
            lblOutputPath.AutoSize = true;

            // Output Path TextBox
            txtOutputPath.Location = new Point(120, 72);
            txtOutputPath.Size = new Size(350, 20);

            // Generate Button
            btnGenerate.Text = "Generate PDF";
            btnGenerate.Location = new Point(120, 180); 
            btnGenerate.Size = new Size(350, 30);
            btnGenerate.Click += new EventHandler(BtnGenerate_Click);

            // Add controls to form
            Controls.AddRange(new Control[] {
        lblDirectory, txtDirectory, btnBrowse,
        lblFileTypes, txtFileTypes,
        lblOutputPath, txtOutputPath,
        btnGenerate
    });

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
                    ExcludeFiles = txtExcludeFiles.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList()
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
                // Получение всех файлов в корневой директории
                var rootFiles = Directory.GetFiles(dirPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => types.Any(t => f.EndsWith($".{t}", StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                LogToFile($"Root Files Before Exclusion: {string.Join(", ", rootFiles)}");

                // Исключаем файлы по маске
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

                // Обрабатываем поддиректории
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

                    // Исключаем файлы по маске
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
    }

    public class Settings
    {
        public string DirectoryPath { get; set; }
        public List<string> FileTypes { get; set; }
        public List<string> ExcludeFolders { get; set; }
        public List<string> ExcludeFiles { get; set; }
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