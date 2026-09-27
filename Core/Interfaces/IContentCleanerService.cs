// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

namespace FolderToPDF.Core.Interfaces;

/// <summary>
/// Provides deterministic content sanitization, including programming language comment removal
/// and zero-trust redacting of sensitive secrets (API keys, passwords, JWT tokens, IP addresses).
/// </summary>
public interface IContentCleanerService
{
    /// <summary>
    /// Executes all configured sanitization steps on source code content based on options.
    /// </summary>
    /// <param name="content">The raw text content to clean.</param>
    /// <param name="fileExtension">The file extension (e.g. ".cs", ".py") used to pick language-specific comment rules.</param>
    /// <param name="removeComments">Whether programming comments should be stripped.</param>
    /// <param name="redactSecrets">Whether sensitive secrets, tokens, and keys should be redacted.</param>
    /// <returns>The sanitized text content.</returns>
    string Clean(string content, string fileExtension, bool removeComments, bool redactSecrets);

    /// <summary>
    /// Strips line and block comments from source code content based on the language grammar of the file extension.
    /// </summary>
    /// <param name="content">The source code content containing comments.</param>
    /// <param name="fileExtension">The target language extension (e.g. ".cs", ".js", ".py", ".sql").</param>
    /// <returns>The source code with comments stripped while preserving string literals and line structures.</returns>
    string RemoveComments(string content, string fileExtension);

    /// <summary>
    /// Redacts known sensitive patterns such as API keys, Bearer tokens, private keys, passwords, and IP addresses.
    /// </summary>
    /// <param name="content">The text content to scan and redact.</param>
    /// <returns>Text content with matching secrets replaced by descriptive redaction tokens (e.g. [REDACTED_API_KEY]).</returns>
    string RedactSensitiveData(string content);
}
