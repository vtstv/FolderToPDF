// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.IO;

namespace FolderToPDF.Core.Models;

/// <summary>
/// Represents a record of a previously generated codebase export artifact (PDF, TXT, or Markdown).
/// </summary>
public class ExportHistoryItem
{
    /// <summary>Gets or sets the unique record identifier.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>Gets or sets the user-assigned project or archive title.</summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>Gets or sets the source directory path scanned for this export.</summary>
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>Gets or sets the absolute path where the generated document was written.</summary>
    public string OutputFilePath { get; set; } = string.Empty;

    /// <summary>Gets or sets the file format designation (e.g. "PDF", "TXT", "MARKDOWN").</summary>
    public string Format { get; set; } = "PDF";

    /// <summary>Gets or sets the UTC creation timestamp of the export.</summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    /// <summary>Gets or sets the total number of files included in the generated document.</summary>
    public int FileCount { get; set; }

    /// <summary>Gets or sets the total token count calculated for the exported context.</summary>
    public int TokenCount { get; set; }

    /// <summary>Gets or sets the output file size in bytes at the time of export.</summary>
    public long FileSizeBytes { get; set; }

    /// <summary>Gets a value indicating whether the target file currently exists on the local filesystem.</summary>
    public bool FileExists => !string.IsNullOrEmpty(OutputFilePath) && File.Exists(OutputFilePath);

    /// <summary>Gets a human-readable representation of the export file size.</summary>
    public string FormattedSize
    {
        get
        {
            if (FileSizeBytes < 1024)
                return $"{FileSizeBytes} B";
            if (FileSizeBytes < 1024 * 1024)
                return $"{(double)FileSizeBytes / 1024:0.0} KB";
            return $"{(double)FileSizeBytes / (1024 * 1024):0.00} MB";
        }
    }
}
