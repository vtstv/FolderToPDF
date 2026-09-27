// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

namespace FolderToPDF.Core.Models;

/// <summary>
/// Status notification object emitted asynchronously to notify listeners (such as UI progress bars) of scan progress.
/// </summary>
public class ScanProgressReport
{
    /// <summary>
    /// Gets or sets the count of files successfully processed or examined so far.
    /// </summary>
    public int ProcessedFiles { get; set; }

    /// <summary>
    /// Gets or sets the total estimated or discovered files to process.
    /// </summary>
    public int TotalEstimatedFiles { get; set; }

    /// <summary>
    /// Gets or sets the path or name of the file currently being processed.
    /// </summary>
    public string CurrentFile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable description of current activity (e.g. "Counting tokens...", "Exporting PDF...").
    /// </summary>
    public string StatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets the current operation progress as a percentage between 0 and 100.
    /// </summary>
    public double ProgressPercentage => TotalEstimatedFiles > 0 
        ? Math.Min(100.0, Math.Round((double)ProcessedFiles / TotalEstimatedFiles * 100.0, 1))
        : 0;
}
