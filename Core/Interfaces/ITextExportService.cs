// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Interfaces;

/// <summary>
/// Generates standardized plain-text archives combining scanned codebase files with clear section delimiters,
/// file header banners, and directory trees suitable for direct pasting into LLM context windows.
/// </summary>
public interface ITextExportService
{
    /// <summary>
    /// Compiles all selected scanned files into a single plain-text aggregate document and saves it asynchronously.
    /// </summary>
    /// <param name="outputPath">The file destination path where the plain text document will be written.</param>
    /// <param name="rootDirectory">The root directory of the scanned codebase.</param>
    /// <param name="files">The collection of scanned and cleaned files to include.</param>
    /// <param name="directoryTree">Optional directory tree text representation to include in the header.</param>
    /// <param name="settings">Application formatting and truncation preferences.</param>
    /// <param name="progress">Optional progress reporter for notifying UI of write operations.</param>
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
    /// Formats the scanned files and directory structure into a single plain-text string in memory.
    /// </summary>
    /// <param name="rootDirectory">The root directory of the scanned codebase.</param>
    /// <param name="files">The collection of scanned and cleaned files to include.</param>
    /// <param name="directoryTree">Optional directory tree text representation to include in the header.</param>
    /// <param name="settings">Application formatting and truncation preferences.</param>
    /// <returns>The complete aggregate plain text document as a string.</returns>
    string FormatAsText(
        string rootDirectory,
        IEnumerable<ScannedFile> files,
        string? directoryTree,
        AppSettings settings);
}
