// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Interfaces;

/// <summary>
/// Generates GitHub-flavored Markdown archives optimized for LLM context prompts, documentation, and code reviews.
/// </summary>
public interface IMarkdownExportService
{
    /// <summary>
    /// Compiles scanned files into a single structured Markdown file and writes it to disk asynchronously.
    /// </summary>
    /// <param name="outputPath">The target file path on disk where the Markdown document will be saved.</param>
    /// <param name="rootDirectory">The root directory path of the scanned project.</param>
    /// <param name="files">The collection of scanned and cleaned files to include.</param>
    /// <param name="directoryTree">Optional directory tree text representation to include in the header.</param>
    /// <param name="settings">Application formatting and truncation preferences.</param>
    /// <param name="progress">Optional progress reporter for UI notification.</param>
    /// <param name="cancellationToken">Cancellation token to cancel file writing.</param>
    /// <returns>A task representing the asynchronous export operation.</returns>
    Task ExportAsync(
        string outputPath,
        string rootDirectory,
        IEnumerable<ScannedFile> files,
        string? directoryTree,
        AppSettings settings,
        IProgress<ScanProgressReport>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Formats the scanned files and metadata directly into a Markdown-formatted string in memory.
    /// </summary>
    /// <param name="rootDirectory">The root directory path of the scanned project.</param>
    /// <param name="files">The collection of scanned and cleaned files to format.</param>
    /// <param name="directoryTree">Optional directory tree text representation to include.</param>
    /// <param name="settings">Application formatting and truncation preferences.</param>
    /// <returns>The complete formatted Markdown string.</returns>
    string FormatAsMarkdown(
        string rootDirectory,
        IEnumerable<ScannedFile> files,
        string? directoryTree,
        AppSettings settings);
}
