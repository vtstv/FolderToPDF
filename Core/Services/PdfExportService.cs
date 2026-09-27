// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.IO;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FolderToPDF.Core.Services;

/// <summary>
/// Generates vector-quality, high-fidelity PDF documents using the QuestPDF layout engine.
/// Renders headers, summary telemetry, directory trees, file banners, line numbers, and footers.
/// </summary>
public class PdfExportService : IPdfExportService
{
    static PdfExportService()
    {
        // Free Community License for open-source and small projects
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Generates and renders a comprehensive PDF codebase document to the specified destination path asynchronously.
    /// </summary>
    /// <param name="outputPath">The file destination path for the exported PDF.</param>
    /// <param name="rootDirectory">The root directory path of the codebase.</param>
    /// <param name="files">The scanned files to render.</param>
    /// <param name="directoryTree">Optional directory tree text hierarchy.</param>
    /// <param name="settings">User formatting preferences including font families, sizes, and line numbering.</param>
    /// <param name="progress">Optional progress reporter for notifying UI.</param>
    /// <param name="cancellationToken">Cancellation token to gracefully cancel rendering.</param>
    /// <returns>A task representing the PDF generation operation.</returns>
    public async Task ExportAsync(
        string outputPath,
        string rootDirectory,
        IEnumerable<ScannedFile> files,
        string? directoryTree,
        AppSettings settings,
        IProgress<ScanProgressReport>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var targetFiles = files.Where(f => f.IsIncluded).ToList();
        if (targetFiles.Count == 0)
        {
            throw new InvalidOperationException("No files selected for PDF export.");
        }

        var dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        progress?.Report(new ScanProgressReport
        {
            ProcessedFiles = 0,
            TotalEstimatedFiles = targetFiles.Count,
            StatusMessage = "Composing PDF document..."
        });

        await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var folderName = GetSafeFolderName(rootDirectory);
            int totalLines = targetFiles.Sum(f => f.LineCount);
            int totalTokens = targetFiles.Sum(f => f.TokenCount);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(28);
                    page.DefaultTextStyle(x => x.FontFamily(settings.ContentFont).FontSize(settings.ContentFontSize));

                    // Document Header: Title, directory path, aggregate metrics
                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Project: {folderName}")
                                    .FontFamily(settings.TitleFont)
                                    .FontSize(16)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);

                                col.Item().Text($"Directory: {rootDirectory}")
                                    .FontSize(8)
                                    .FontColor(Colors.Grey.Medium);
                            });

                            row.ConstantItem(180).AlignRight().Column(stats =>
                            {
                                stats.Item().Text($"{targetFiles.Count:N0} Files  |  {totalLines:N0} Lines  |  {totalTokens:N0} Tokens")
                                    .FontSize(8)
                                    .Bold()
                                    .FontColor(Colors.Grey.Darken3);

                                stats.Item().Text($"Exported: {DateTime.Now:yyyy-MM-dd HH:mm}")
                                    .FontSize(7)
                                    .FontColor(Colors.Grey.Medium);
                            });
                        });

                        header.Item().PaddingVertical(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    // Document Content: Directory structure followed by individual source files
                    page.Content().PaddingVertical(8).Column(contentCol =>
                    {
                        // 1. Directory Tree Summary Section (if enabled)
                        if (!string.IsNullOrWhiteSpace(directoryTree))
                        {
                            contentCol.Item().Background(Colors.Grey.Lighten4).Padding(8).Column(treeBox =>
                            {
                                treeBox.Item().Text("PROJECT STRUCTURE")
                                    .FontFamily(settings.TitleFont)
                                    .FontSize(9)
                                    .Bold()
                                    .FontColor(Colors.Grey.Darken2);

                                treeBox.Item().Text(directoryTree)
                                    .FontFamily("Consolas")
                                    .FontSize(7)
                                    .FontColor(Colors.Grey.Darken3);
                            });

                            contentCol.Item().PageBreak();
                        }

                        // 2. Sequential file listings
                        for (int i = 0; i < targetFiles.Count; i++)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var file = targetFiles[i];

                            contentCol.Item().PaddingTop(10).Column(fileBox =>
                            {
                                // File Header Banner with file metadata
                                fileBox.Item().Background(Colors.Grey.Lighten3).Padding(4).Row(banner =>
                                {
                                    banner.RelativeItem().Text(file.RelativePath)
                                        .FontFamily(settings.TitleFont)
                                        .FontSize(settings.TitleFontSize)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken3);

                                    banner.AutoItem().Text($"{file.LineCount:N0} lines  •  {file.TokenCount:N0} tokens  •  {file.SizeInBytes / 1024.0:F1} KB")
                                        .FontSize(8)
                                        .FontColor(Colors.Grey.Darken2);
                                });

                                // Code content block with optional line numbering
                                var textContent = string.IsNullOrEmpty(file.CleanedContent) ? file.RawContent : file.CleanedContent;
                                if (settings.PdfIncludeLineNumbers)
                                {
                                    var rawLines = textContent.Split('\n');
                                    int pad = Math.Max(3, rawLines.Length.ToString().Length);
                                    var sb = new System.Text.StringBuilder();
                                    for (int l = 0; l < rawLines.Length; l++)
                                    {
                                        sb.Append((l + 1).ToString().PadLeft(pad));
                                        sb.Append(" | ");
                                        sb.AppendLine(rawLines[l].TrimEnd('\r'));
                                    }
                                    textContent = sb.ToString();
                                }

                                fileBox.Item().Padding(4).Text(textContent)
                                    .FontFamily(settings.ContentFont)
                                    .FontSize(settings.ContentFontSize)
                                    .FontColor(Colors.Grey.Darken4);
                            });

                            // Clean page break between files except for the trailing file
                            if (i < targetFiles.Count - 1)
                            {
                                contentCol.Item().PageBreak();
                            }
                        }
                    });

                    // Document Footer: Application branding and dynamic page numbers
                    page.Footer().Row(footer =>
                    {
                        footer.RelativeItem().Text("Generated with FolderToPDF")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium);

                        footer.RelativeItem().AlignRight().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                    });
                });
            });

            document.GeneratePdf(outputPath);
        }, cancellationToken);

        progress?.Report(new ScanProgressReport
        {
            ProcessedFiles = targetFiles.Count,
            TotalEstimatedFiles = targetFiles.Count,
            StatusMessage = $"PDF successfully generated at: {outputPath}"
        });
    }

    /// <summary>
    /// Safely resolves the directory name from the root path for heading display.
    /// </summary>
    private static string GetSafeFolderName(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "Project";

        try
        {
            var trimmed = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var name = Path.GetFileName(trimmed);
            return string.IsNullOrWhiteSpace(name) ? trimmed : name;
        }
        catch
        {
            return "Project";
        }
    }
}
