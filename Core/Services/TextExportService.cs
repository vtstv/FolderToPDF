// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.IO;
using System.Text;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Services;

/// <summary>
/// Compiles scanned files into a single unified plain-text archive with clear visual ASCII delimiters,
/// summary headers, and file demarcations suitable for command-line tools or LLMs.
/// </summary>
public class TextExportService : ITextExportService
{
    /// <summary>
    /// Exports the selected files as an aggregate plain text document and writes it to disk asynchronously.
    /// </summary>
    /// <param name="outputPath">The file destination path where the plain text file will be saved.</param>
    /// <param name="rootDirectory">The root directory of the scanned codebase.</param>
    /// <param name="files">The collection of scanned and cleaned files to include.</param>
    /// <param name="directoryTree">Optional directory tree text hierarchy.</param>
    /// <param name="settings">Application formatting and truncation preferences.</param>
    /// <param name="progress">Optional progress reporter for notifying UI.</param>
    /// <param name="cancellationToken">Cancellation token to gracefully cancel writing.</param>
    /// <returns>A task representing the plain text export operation.</returns>
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
            throw new InvalidOperationException("No files selected for TXT export.");
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
            StatusMessage = "Composing TXT document..."
        });

        var content = await Task.Run(() => FormatAsText(rootDirectory, targetFiles, directoryTree, settings), cancellationToken);
        await File.WriteAllTextAsync(outputPath, content, Encoding.UTF8, cancellationToken);

        progress?.Report(new ScanProgressReport
        {
            ProcessedFiles = targetFiles.Count,
            TotalEstimatedFiles = targetFiles.Count,
            StatusMessage = $"TXT file successfully generated at: {outputPath}"
        });
    }

    /// <summary>
    /// Formats the scanned files and metadata directly into a plain-text string with structured section separators.
    /// </summary>
    /// <param name="rootDirectory">The root directory path of the scanned codebase.</param>
    /// <param name="files">The collection of scanned files to include.</param>
    /// <param name="directoryTree">Optional directory tree text hierarchy.</param>
    /// <param name="settings">Application formatting and truncation preferences.</param>
    /// <returns>The complete aggregate plain text document string.</returns>
    public string FormatAsText(
        string rootDirectory,
        IEnumerable<ScannedFile> files,
        string? directoryTree,
        AppSettings settings)
    {
        var sb = new StringBuilder();
        var targetFiles = files.Where(f => f.IsIncluded).ToList();
        var folderName = GetSafeFolderName(rootDirectory);
        int totalLines = targetFiles.Sum(f => f.LineCount);
        int totalTokens = targetFiles.Sum(f => f.TokenCount);

        sb.AppendLine("================================================================================");
        sb.AppendLine($"PROJECT ARCHIVE: {folderName}");
        sb.AppendLine($"ROOT PATH:       {rootDirectory}");
        sb.AppendLine($"GENERATED AT:    {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"STATISTICS:      {targetFiles.Count:N0} Files | {totalLines:N0} Lines | ~{totalTokens:N0} Tokens");
        sb.AppendLine("================================================================================");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(directoryTree))
        {
            sb.AppendLine("PROJECT DIRECTORY TREE:");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine(directoryTree);
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine();
        }

        foreach (var file in targetFiles)
        {
            sb.AppendLine("================================================================================");
            sb.AppendLine($"FILE: {file.RelativePath}");
            sb.AppendLine($"LINES: {file.LineCount:N0} | TOKENS: ~{file.TokenCount:N0} | SIZE: {file.SizeInBytes / 1024.0:F1} KB");
            sb.AppendLine("================================================================================");
            sb.AppendLine();

            var text = string.IsNullOrEmpty(file.CleanedContent) ? file.RawContent : file.CleanedContent;
            sb.AppendLine(text);
            sb.AppendLine();
            sb.AppendLine();
        }

        return sb.ToString();
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
