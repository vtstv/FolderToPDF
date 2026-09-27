// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Enums;

namespace FolderToPDF.Core.Models;

/// <summary>
/// Represents persistent application-wide settings and user preferences.
/// Saved to and loaded from local JSON configuration across user sessions.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Gets or sets the path of the last directory scanned or selected by the user.
    /// </summary>
    public string LastDirectoryPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default relative or absolute directory where export artifacts are stored.
    /// </summary>
    public string DefaultOutputDirectory { get; set; } = "output";

    /// <summary>
    /// Gets or sets a value indicating whether the output directory in Windows File Explorer should open automatically after export completes.
    /// </summary>
    public bool AutoOpenOutputFolder { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the generated document should open immediately in the default system viewer after export.
    /// </summary>
    public bool AutoOpenExportedFile { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether consecutive empty lines in code files should be collapsed into single blank lines to save tokens.
    /// </summary>
    public bool NormalizeEmptyLines { get; set; } = false;

    /// <summary>
    /// Gets or sets the identifier of the active scan/exclusion profile.
    /// </summary>
    public string ActiveProfileId { get; set; } = "builtin-codebase";

    /// <summary>
    /// Gets or sets the font family used for document headings and metadata in exported PDFs.
    /// </summary>
    public string TitleFont { get; set; } = "Segoe UI";

    /// <summary>
    /// Gets or sets the font size (in points) for document titles and section headers.
    /// </summary>
    public int TitleFontSize { get; set; } = 11;

    /// <summary>
    /// Gets or sets the monospace font family used for code blocks and file listings.
    /// </summary>
    public string ContentFont { get; set; } = "Consolas";

    /// <summary>
    /// Gets or sets the font size (in points) for source code listings.
    /// </summary>
    public int ContentFontSize { get; set; } = 8;

    /// <summary>
    /// Gets or sets a value indicating whether line numbers should be rendered in PDF code blocks.
    /// </summary>
    public bool PdfIncludeLineNumbers { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a Table of Contents is generated at the start of PDF documents.
    /// </summary>
    public bool PdfIncludeTableOfContents { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum character limit per file before content is truncated with a warning notice.
    /// </summary>
    public int TruncatedContentLength { get; set; } = 250_000;

    /// <summary>
    /// Gets or sets the user's preferred tokenizer model for token counting and context estimation.
    /// Defaults to OpenAI GPT-4o o200k_base.
    /// </summary>
    public TokenizerModel PreferredTokenizer { get; set; } = TokenizerModel.O200kBase;

    /// <summary>
    /// Gets or sets the application appearance theme (System, Light, or Dark).
    /// </summary>
    public AppThemeMode ThemeMode { get; set; } = AppThemeMode.System;

    /// <summary>
    /// Gets or sets a value indicating whether API keys and auth tokens should be redacted.
    /// </summary>
    public bool RedactApiKeys { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether password assignments and credentials should be redacted.
    /// </summary>
    public bool RedactPasswords { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether email addresses should be redacted.
    /// </summary>
    public bool RedactEmails { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether IPv4 and IPv6 addresses should be redacted.
    /// </summary>
    public bool RedactIps { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether RSA/SSH/EC private key blocks should be redacted.
    /// </summary>
    public bool RedactPrivateKeys { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether JSON Web Tokens (JWT) should be redacted.
    /// </summary>
    public bool RedactJwtTokens { get; set; } = true;
}
