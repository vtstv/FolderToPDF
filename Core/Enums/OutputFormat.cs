// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

namespace FolderToPDF.Core.Enums;

/// <summary>
/// Specifies the supported file formats when exporting scanned and cleaned project content.
/// </summary>
public enum OutputFormat
{
    /// <summary>
    /// Formatted PDF document generated via QuestPDF with syntax-highlighted code blocks, metadata summary, and file tree.
    /// </summary>
    Pdf,

    /// <summary>
    /// Plain-text aggregate file with structured delimiters and file headers suitable for basic LLM prompts.
    /// </summary>
    Txt,

    /// <summary>
    /// GitHub-flavored Markdown document with fenced code blocks, language identifiers, and directory hierarchy.
    /// </summary>
    Markdown
}
