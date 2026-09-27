// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Enums;

namespace FolderToPDF.Core.Models;

/// <summary>
/// Encapsulates the complete set of parameters driving a directory traversal and file sanitization execution.
/// </summary>
public class ScanOptions
{
    /// <summary>
    /// Gets or sets the absolute path to the root folder to be scanned.
    /// </summary>
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file pattern filters to include (e.g. *.cs, *.py, *.ts).
    /// </summary>
    public List<string> FileTypes { get; set; } = new() { "*.cs", "*.ts", "*.tsx", "*.js", "*.py", "*.json", "*.md" };

    /// <summary>
    /// Gets or sets directory names or patterns to ignore during recursive search.
    /// </summary>
    public List<string> ExcludeFolders { get; set; } = new() { "node_modules", "bin", "obj", ".git", ".vs", ".idea", "dist", "build", "__pycache__", ".venv" };

    /// <summary>
    /// Gets or sets file patterns to ignore during scanning.
    /// </summary>
    public List<string> ExcludeFiles { get; set; } = new() { "*.lock", "package-lock.json", "*.min.js", "*.min.css", "*.map", "*.svg", "*.png", "*.jpg", "*.ico" };

    /// <summary>
    /// Gets or sets explicitly included file paths that bypass normal exclusion filters.
    /// </summary>
    public List<string> IncludeFiles { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether comments should be stripped from files.
    /// </summary>
    public bool RemoveComments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether regex-based secret scrubbing is enabled.
    /// </summary>
    public bool RedactSecrets { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether an ASCII/Unicode directory tree is prepended to output.
    /// </summary>
    public bool IncludeFileTreeHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets the character count threshold after which single-file content is truncated.
    /// </summary>
    public int MaxContentLengthPerFile { get; set; } = 250_000;

    /// <summary>
    /// Gets or sets the maximum allowed raw file size in bytes before skipping (default 5 MB).
    /// </summary>
    public long MaxFileSizeInBytes { get; set; } = 5 * 1024 * 1024;

    /// <summary>
    /// Gets or sets the tokenizer model algorithm used for metric computations during scan.
    /// </summary>
    public TokenizerModel Tokenizer { get; set; } = TokenizerModel.Cl100kBase;
}
