// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Interfaces;

/// <summary>
/// Generates vector-quality, printable PDF documents using QuestPDF from scanned source code archives.
/// Includes cover page, optional table of contents, directory tree, code syntax styling, and page numbering.
/// </summary>
public interface IPdfExportService
{
    /// <summary>
    /// Generates a structured PDF document and writes it to the designated disk path asynchronously.
    /// </summary>
    /// <param name="outputPath">The file destination path for the exported PDF file.</param>
    /// <param name="rootDirectory">The source root directory path of the scanned codebase.</param>
    /// <param name="files">The collection of scanned and cleaned files to render into pages.</param>
    /// <param name="directoryTree">Optional directory hierarchy text to render into the summary section.</param>
    /// <param name="settings">Application formatting options including font names, sizes, and TOC flags.</param>
    /// <param name="progress">Optional progress reporter to notify UI during page generation and writing.</param>
    /// <param name="cancellationToken">Cancellation token to gracefully cancel document rendering.</param>
    /// <returns>A task representing the asynchronous PDF generation operation.</returns>
    Task ExportAsync(
        string outputPath,
        string rootDirectory,
        IEnumerable<ScannedFile> files,
        string? directoryTree,
        AppSettings settings,
        IProgress<ScanProgressReport>? progress = null,
        CancellationToken cancellationToken = default);
}
