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
/// Generates GitHub-flavored Markdown documents structured specifically for LLM context injection,
/// code reviews, and offline documentation archives.
/// </summary>
public class MarkdownExportService : IMarkdownExportService
{
    /// <summary>
    /// Formats selected files and writes the complete Markdown document to the specified output path asynchronously.
    /// </summary>
    /// <param name="outputPath">The target file path on disk where the Markdown file will be saved.</param>
    /// <param name="rootDirectory">The root directory of the scanned project.</param>
    /// <param name="files">The scanned files to include in the output.</param>
    /// <param name="directoryTree">Optional directory hierarchy text.</param>
    /// <param name="settings">Application formatting settings.</param>
    /// <param name="progress">Optional progress reporter for status updates.</param>
    /// <param name="cancellationToken">Cancellation token to abort the operation.</param>
    /// <returns>A task representing the export operation.</returns>
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
            throw new InvalidOperationException("No files selected for Markdown export.");
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
            StatusMessage = "Composing Markdown prompt document..."
        });

        var content = await Task.Run(() => FormatAsMarkdown(rootDirectory, targetFiles, directoryTree, settings), cancellationToken);
        await File.WriteAllTextAsync(outputPath, content, Encoding.UTF8, cancellationToken);

        progress?.Report(new ScanProgressReport
        {
            ProcessedFiles = targetFiles.Count,
            TotalEstimatedFiles = targetFiles.Count,
            StatusMessage = $"Markdown file successfully generated at: {outputPath}"
        });
    }

    /// <summary>
    /// Compiles scanned files into a single GitHub-flavored Markdown string with fenced code blocks.
    /// </summary>
    /// <param name="rootDirectory">The root directory path of the scanned codebase.</param>
    /// <param name="files">The scanned files to include.</param>
    /// <param name="directoryTree">Optional directory tree text representation.</param>
    /// <param name="settings">Application formatting settings.</param>
    /// <returns>The complete formatted Markdown string.</returns>
    public string FormatAsMarkdown(
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

        sb.AppendLine($"# Codebase Context: {folderName}");
        sb.AppendLine();
        sb.AppendLine($"- **Root Path:** `{rootDirectory}`");
        sb.AppendLine($"- **Generated:** `{DateTime.Now:yyyy-MM-dd HH:mm:ss}`");
        sb.AppendLine($"- **Files Included:** {targetFiles.Count:N0}");
        sb.AppendLine($"- **Total Lines of Code:** {totalLines:N0}");
        sb.AppendLine($"- **Estimated AI Tokens:** ~{totalTokens:N0}");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(directoryTree))
        {
            sb.AppendLine("## Directory Structure");
            sb.AppendLine();
            sb.AppendLine("```text");
            sb.AppendLine(directoryTree.TrimEnd());
            sb.AppendLine("```");
            sb.AppendLine();
        }

        sb.AppendLine("## Source Files");
        sb.AppendLine();

        foreach (var file in targetFiles)
        {
            var lang = GetMarkdownLanguageId(file.Extension);
            sb.AppendLine($"### `{file.RelativePath}`");
            sb.AppendLine();
            sb.AppendLine($"> Lines: **{file.LineCount:N0}** | Tokens: **~{file.TokenCount:N0}** | Size: **{file.SizeInBytes / 1024.0:F1} KB**");
            sb.AppendLine();
            sb.AppendLine($"```{lang}");
            var text = string.IsNullOrEmpty(file.CleanedContent) ? file.RawContent : file.CleanedContent;
            sb.AppendLine(text);
            sb.AppendLine("```");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Maps a file extension to its corresponding Markdown code fence language identifier.
    /// </summary>
    /// <param name="extension">The file extension with or without leading dot.</param>
    /// <returns>The canonical language identifier string (e.g. "csharp", "python").</returns>
    private static string GetMarkdownLanguageId(string extension)
    {
        var ext = extension.TrimStart('.').ToLowerInvariant();
        return ext switch
        {
            "cs" => "csharp",
            "js" => "javascript",
            "ts" => "typescript",
            "tsx" => "tsx",
            "jsx" => "jsx",
            "py" => "python",
            "cpp" or "cc" or "cxx" => "cpp",
            "c" or "h" => "c",
            "hpp" => "cpp",
            "rs" => "rust",
            "go" => "go",
            "rb" => "ruby",
            "java" => "java",
            "kt" or "kts" => "kotlin",
            "swift" => "swift",
            "php" => "php",
            "sh" or "bash" => "bash",
            "ps1" => "powershell",
            "sql" => "sql",
            "html" => "html",
            "css" => "css",
            "scss" => "scss",
            "json" => "json",
            "yaml" or "yml" => "yaml",
            "xml" or "xaml" => "xml",
            "md" => "markdown",
            _ => "text"
        };
    }

    /// <summary>
    /// Extracts a user-friendly folder name from a root directory path.
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
