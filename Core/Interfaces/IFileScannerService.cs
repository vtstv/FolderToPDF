// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Interfaces;

/// <summary>
/// Scans directory structures recursively, applies inclusion/exclusion filters,
/// detects binary files, performs sanitization, and produces hierarchical project trees.
/// </summary>
public interface IFileScannerService
{
    /// <summary>
    /// Traverses the specified root directory asynchronously, loading and sanitizing matching text files.
    /// </summary>
    /// <param name="options">Options specifying directory path, patterns, exclusion lists, and processing flags.</param>
    /// <param name="progress">Optional progress reporter for notifying UI of scanning and processing status.</param>
    /// <param name="cancellationToken">Cancellation token to gracefully abort background scanning.</param>
    /// <returns>A <see cref="ScanResult"/> containing the collection of scanned files, metrics, and errors.</returns>
    Task<ScanResult> ScanDirectoryAsync(
        ScanOptions options,
        IProgress<ScanProgressReport>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Constructs a visual ASCII/Unicode directory tree representation for a list of relative file paths.
    /// </summary>
    /// <param name="rootPath">The root directory path being visualized.</param>
    /// <param name="includedRelativePaths">The relative file paths to include in the tree hierarchy.</param>
    /// <returns>A formatted multi-line string showing directory branches and files.</returns>
    string GenerateDirectoryTree(string rootPath, IEnumerable<string> includedRelativePaths);
}
