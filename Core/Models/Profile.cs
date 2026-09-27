// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

namespace FolderToPDF.Core.Models;

/// <summary>
/// Represents a named scanning and export preset configuration that users can save, customize, and switch between.
/// </summary>
public class Profile
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Gets or sets the user-friendly display name of the profile.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional descriptive summary explaining what this profile targets.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this profile is a built-in immutable default.
    /// </summary>
    public bool IsBuiltIn { get; set; }

    /// <summary>
    /// Gets or sets the default target directory associated with this profile, if any.
    /// </summary>
    public string DirectoryPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of file wildcard patterns to include during scanning (e.g. *.cs, *.ts).
    /// </summary>
    public List<string> FileTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets directory names or wildcard patterns to exclude from scanning (e.g. node_modules, bin).
    /// </summary>
    public List<string> ExcludeFolders { get; set; } = new();

    /// <summary>
    /// Gets or sets file wildcard patterns to explicitly exclude from scanning (e.g. *.lock, *.min.js).
    /// </summary>
    public List<string> ExcludeFiles { get; set; } = new();

    /// <summary>
    /// Gets or sets specific relative or absolute file paths to force-include regardless of exclusion rules.
    /// </summary>
    public List<string> IncludeFiles { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether comments should be stripped from supported code files.
    /// </summary>
    public bool RemoveComments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether sensitive data (keys, passwords, tokens) should be redacted.
    /// </summary>
    public bool RedactSecrets { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a directory tree representation is generated at the top of exports.
    /// </summary>
    public bool IncludeFileTreeHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets the default output PDF file name when exporting.
    /// </summary>
    public string OutputPdfName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default output TXT file name when exporting.
    /// </summary>
    public string OutputTxtName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default output Markdown file name when exporting.
    /// </summary>
    public string OutputMarkdownName { get; set; } = string.Empty;
}
