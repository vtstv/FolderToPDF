// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

namespace FolderToPDF.Core.Models;

/// <summary>
/// Represents a single file discovered during scanning, along with its metadata, raw content, and cleaned/redacted content.
/// </summary>
public class ScannedFile
{
    /// <summary>
    /// Gets or sets the absolute filesystem path to the file.
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relative path of the file relative to the root scan folder.
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file name including extension.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the lowercased file extension with leading period (e.g. ".cs").
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the raw file in bytes.
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// Gets or sets the number of lines of text in the cleaned content.
    /// </summary>
    public int LineCount { get; set; }

    /// <summary>
    /// Gets or sets the calculated token count using the active tokenizer model.
    /// </summary>
    public int TokenCount { get; set; }

    /// <summary>
    /// Gets or sets the original unmodified text content read from disk.
    /// </summary>
    public string RawContent { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the processed text content after comment removal, redaction, and truncation.
    /// </summary>
    public string CleanedContent { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this file is selected for inclusion in the export.
    /// </summary>
    public bool IsIncluded { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this file contains unusually long single tokens (e.g., base64 strings or minified chunks).
    /// </summary>
    public bool HasLargeTokens { get; set; }

    /// <summary>
    /// Gets or sets any error or warning message encountered while reading or processing this file.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
