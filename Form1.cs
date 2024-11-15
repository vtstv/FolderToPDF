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

namespace DirectoryToPDFConverter
{
    public partial class MainForm : Form
    {
        // Control declarations
        private TextBox txtDirectory;
        private TextBox txtFileTypes;
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
            this.txtDirectory = new TextBox();
            this.txtFileTypes = new TextBox();
            this.txtOutputPath = new TextBox();
            this.btnBrowse = new Button();
            this.btnGenerate = new Button();
            this.lblDirectory = new Label();
            this.lblFileTypes = new Label();
            this.lblOutputPath = new Label();

            // Form settings
            this.SuspendLayout();
            this.Text = "Directory to PDF Converter";
            this.Size = new System.Drawing.Size(600, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Directory Label
            this.lblDirectory.Text = "Select Directory:";
            this.lblDirectory.Location = new System.Drawing.Point(10, 15);
            this.lblDirectory.AutoSize = true;

            // Directory TextBox
            this.txtDirectory.Location = new System.Drawing.Point(120, 12);
            this.txtDirectory.Size = new System.Drawing.Size(350, 20);
            this.txtDirectory.AllowDrop = true;
            this.txtDirectory.DragDrop += new DragEventHandler(TxtDirectory_DragDrop);
            this.txtDirectory.DragEnter += new DragEventHandler(TxtDirectory_DragEnter);

            // Browse Button
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.Location = new System.Drawing.Point(480, 10);
            this.btnBrowse.Click += new EventHandler(BtnBrowse_Click);

            // File Types Label
            this.lblFileTypes.Text = "File Types:";
            this.lblFileTypes.Location = new System.Drawing.Point(10, 45);
            this.lblFileTypes.AutoSize = true;

            // File Types TextBox
            this.txtFileTypes.Location = new System.Drawing.Point(120, 42);
            this.txtFileTypes.Size = new System.Drawing.Size(350, 20);

            // Output Path Label
            this.lblOutputPath.Text = "Output PDF Path:";
            this.lblOutputPath.Location = new System.Drawing.Point(10, 75);
            this.lblOutputPath.AutoSize = true;

            // Output Path TextBox
            this.txtOutputPath.Location = new System.Drawing.Point(120, 72);
            this.txtOutputPath.Size = new System.Drawing.Size(350, 20);

            // Generate Button
            this.btnGenerate.Text = "Generate PDF";
            this.btnGenerate.Location = new System.Drawing.Point(120, 110);
            this.btnGenerate.Size = new System.Drawing.Size(350, 30);
            this.btnGenerate.Click += new EventHandler(BtnGenerate_Click);

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                this.lblDirectory, this.txtDirectory, this.btnBrowse,
                this.lblFileTypes, this.txtFileTypes,
                this.lblOutputPath, this.txtOutputPath,
                this.btnGenerate
            });

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetupForm()
        {
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(MainForm_DragEnter);
            this.DragDrop += new DragEventHandler(MainForm_DragDrop);
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
                        directoryPath = settings.DirectoryPath;
                        fileTypes = settings.FileTypes ?? new List<string> { "py", "css", "html" };

                        txtDirectory.Text = directoryPath;
                        txtFileTypes.Text = string.Join(", ", fileTypes);
                        SetDefaultOutputFilename();
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
                    DirectoryPath = directoryPath,
                    FileTypes = fileTypes
                };

                File.WriteAllText(SETTINGS_FILE, JsonConvert.SerializeObject(settings));
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

                if (!txtOutputPath.Text.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Output file must be a PDF.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                fileTypes = types.ToList();
                SaveSettings();

                var contents = GetDirectoryContents(directoryPath, fileTypes);
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

        private List<(string FilePath, string FileName, string Content)> GetDirectoryContents(string dirPath, List<string> types)
        {
            var contents = new List<(string, string, string)>();
            try
            {
                var files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => types.Any(t => f.EndsWith($".{t}", StringComparison.OrdinalIgnoreCase)));

                foreach (var file in files)
                {
                    try
                    {
                        var content = File.ReadAllText(file, Encoding.UTF8);
                        contents.Add((file, Path.GetFileName(file), content));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading file {file}: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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