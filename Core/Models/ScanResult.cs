// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

namespace FolderToPDF.Core.Models;

/// <summary>
/// Encapsulates the complete result of a directory scan operation, including all processed files, exclusions, and aggregate metrics.
/// </summary>
public class ScanResult
{
    /// <summary>
    /// Gets or sets the root folder directory path that was scanned.
    /// </summary>
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of files discovered and evaluated during the scan.
    /// </summary>
    public List<ScannedFile> Files { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of relative file paths that were skipped due to size limits or binary detection.
    /// </summary>
    public List<string> SkippedFiles { get; set; } = new();

    /// <summary>
    /// Gets or sets any error or warning messages encountered during the scan process.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Gets or sets the total duration taken to scan, sanitize, and tokenize files.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets the count of files marked as included for export.
    /// </summary>
    public int TotalIncludedFiles => Files.Count(f => f.IsIncluded);

    /// <summary>
    /// Gets the total size in bytes of all included files.
    /// </summary>
    public long TotalIncludedBytes => Files.Where(f => f.IsIncluded).Sum(f => f.SizeInBytes);

    /// <summary>
    /// Gets the total line count across all included files.
    /// </summary>
    public int TotalIncludedLines => Files.Where(f => f.IsIncluded).Sum(f => f.LineCount);

    /// <summary>
    /// Gets the total token count across all included files.
    /// </summary>
    public int TotalIncludedTokens => Files.Where(f => f.IsIncluded).Sum(f => f.TokenCount);
}
