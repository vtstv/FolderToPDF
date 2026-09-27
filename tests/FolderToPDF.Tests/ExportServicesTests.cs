// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;
using FolderToPDF.Core.Services;
using Xunit;

namespace FolderToPDF.Tests;

public class ExportServicesTests : IDisposable
{
    private readonly string _tempOutputDir;
    private readonly List<ScannedFile> _sampleFiles;

    public ExportServicesTests()
    {
        _tempOutputDir = Path.Combine(Path.GetTempPath(), "FolderToPDF_ExportTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempOutputDir);

        _sampleFiles = new List<ScannedFile>
        {
            new()
            {
                FileName = "Program.cs",
                RelativePath = Path.Combine("src", "Program.cs"),
                Extension = ".cs",
                SizeInBytes = 250,
                LineCount = 10,
                TokenCount = 45,
                RawContent = "Console.WriteLine(\"Hello World!\");",
                CleanedContent = "Console.WriteLine(\"Hello World!\");",
                IsIncluded = true
            },
            new()
            {
                FileName = "config.json",
                RelativePath = "config.json",
                Extension = ".json",
                SizeInBytes = 120,
                LineCount = 5,
                TokenCount = 20,
                RawContent = "{ \"version\": \"2.0\" }",
                CleanedContent = "{ \"version\": \"2.0\" }",
                IsIncluded = true
            }
        };
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempOutputDir))
            {
                Directory.Delete(_tempOutputDir, true);
            }
        }
        catch { }
    }

    [Fact]
    public async Task TextExportService_ExportsValidTxtFile()
    {
        var service = new TextExportService();
        var outputPath = Path.Combine(_tempOutputDir, "output.txt");
        var settings = new AppSettings();

        await service.ExportAsync(outputPath, _tempOutputDir, _sampleFiles, "sample/tree", settings);

        Assert.True(File.Exists(outputPath));
        var content = await File.ReadAllTextAsync(outputPath);
        Assert.Contains("PROJECT ARCHIVE", content);
        Assert.Contains("FILE: src", content);
        Assert.Contains("Console.WriteLine", content);
    }

    [Fact]
    public async Task MarkdownExportService_ExportsFormattedMarkdown()
    {
        var service = new MarkdownExportService();
        var outputPath = Path.Combine(_tempOutputDir, "output.md");
        var settings = new AppSettings();

        await service.ExportAsync(outputPath, _tempOutputDir, _sampleFiles, "sample/tree", settings);

        Assert.True(File.Exists(outputPath));
        var content = await File.ReadAllTextAsync(outputPath);
        Assert.Contains("# Codebase Context", content);
        Assert.Contains("```csharp", content);
        Assert.Contains("```json", content);
    }

    [Fact]
    public async Task PdfExportService_ExportsValidPdfDocument()
    {
        var service = new PdfExportService();
        var outputPath = Path.Combine(_tempOutputDir, "output.pdf");
        var settings = new AppSettings();

        await service.ExportAsync(outputPath, _tempOutputDir, _sampleFiles, "sample/tree", settings);

        Assert.True(File.Exists(outputPath));
        var fileInfo = new FileInfo(outputPath);
        Assert.True(fileInfo.Length > 1000); // Valid PDF with content
    }

    [Fact]
    public async Task ExportServices_EmptyFiles_ThrowsInvalidOperationException()
    {
        var textService = new TextExportService();
        var mdService = new MarkdownExportService();
        var pdfService = new PdfExportService();
        var settings = new AppSettings();

        var emptyList = new List<ScannedFile>();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            textService.ExportAsync(Path.Combine(_tempOutputDir, "1.txt"), _tempOutputDir, emptyList, null, settings));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            mdService.ExportAsync(Path.Combine(_tempOutputDir, "1.md"), _tempOutputDir, emptyList, null, settings));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pdfService.ExportAsync(Path.Combine(_tempOutputDir, "1.pdf"), _tempOutputDir, emptyList, null, settings));
    }

    [Fact]
    public async Task PdfExportService_ExportsWithLineNumbers()
    {
        var service = new PdfExportService();
        var outputPath = Path.Combine(_tempOutputDir, "output_linenumbers.pdf");
        var settings = new AppSettings { PdfIncludeLineNumbers = true };

        await service.ExportAsync(outputPath, _tempOutputDir, _sampleFiles, "sample/tree", settings);

        Assert.True(File.Exists(outputPath));
        var fileInfo = new FileInfo(outputPath);
        Assert.True(fileInfo.Length > 1000);
    }

    [Fact]
    public void ExportServices_FormatSafelyWithEmptyOrRootDirectory()
    {
        var textService = new TextExportService();
        var mdService = new MarkdownExportService();
        var settings = new AppSettings();

        var textResult = textService.FormatAsText("", _sampleFiles, null, settings);
        Assert.Contains("PROJECT ARCHIVE: Project", textResult);

        var mdResult = mdService.FormatAsMarkdown("", _sampleFiles, null, settings);
        Assert.Contains("# Codebase Context: Project", mdResult);
    }
}
