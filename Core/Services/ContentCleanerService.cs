// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Text.RegularExpressions;
using FolderToPDF.Core.Interfaces;

namespace FolderToPDF.Core.Services;

/// <summary>
/// Provides deterministic content sanitization, stripping programming language comments
/// without mutilating string literals, and zero-trust redaction of sensitive credentials.
/// </summary>
public class ContentCleanerService : IContentCleanerService
{
    // ========================================================================
    // Regex Tokenizers for Comment Stripping
    //
    // Note: To avoid stripping comment markers that appear INSIDE string literals
    // (such as "http://example.com" or "/* not a comment */"), our regexes match
    // string literals and comments in a single pass from left to right.
    // The match evaluator checks if the token is a comment, replacing it with an
    // empty string, or preserves the token if it is a string literal.
    // ========================================================================

    /// <summary>
    /// Matches C-family tokens: verbatim strings (@"..."), regular strings ("..."),
    /// char literals ('...'), block comments (/*...*/), and line comments (//...).
    /// </summary>
    private static readonly Regex CFamilyTokensRegex = new(
        @"@""(?:""""|[^""])*""|""(?:\\.|[^""\\])*""|'(?:\\.|[^'\\])*'|/\*[\s\S]*?\*/|//.*$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    /// <summary>
    /// Matches scripting/hash-comment tokens (Python, Bash, Ruby, YAML):
    /// triple-quoted strings ("""...""" or '''...'''), quoted strings, and # comments.
    /// </summary>
    private static readonly Regex HashTokensRegex = new(
        @"\""{3}[\s\S]*?\""{3}|'{3}[\s\S]*?'{3}|""(?:\\.|[^""\\])*""|'(?:\\.|[^'\\])*'|#.*$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    /// <summary>
    /// Matches HTML, XML, SVG, and XAML comments (&lt;!-- ... --&gt;).
    /// </summary>
    private static readonly Regex HtmlXmlCommentRegex = new(
        @"<!--[\s\S]*?-->",
        RegexOptions.Compiled);

    /// <summary>
    /// Matches SQL strings (including doubled single quotes ''), double-quoted identifiers,
    /// block comments (/*...*/), and single-line comments (--...).
    /// </summary>
    private static readonly Regex SqlTokensRegex = new(
        @"'(?:''|[^'])*'|""(?:\\.|[^""\\])*""|/\*[\s\S]*?\*/|--.*$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    /// <summary>
    /// Matches INI and configuration file comments (; or # at start of line or following whitespace).
    /// </summary>
    private static readonly Regex IniTokensRegex = new(
        @"""(?:\\.|[^""\\])*""|'(?:\\.|[^'\\])*'|(?<=^|\s);.*$|#.*$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    /// <summary>
    /// Matches 3 or more consecutive blank lines to compress vertical whitespace after comment removal.
    /// </summary>
    private static readonly Regex MultipleBlankLinesRegex = new(
        @"(\r?\n\s*){3,}",
        RegexOptions.Compiled);

    // ========================================================================
    // Secret Scrubbing Regular Expressions
    // ========================================================================

    /// <summary>Matches Anthropic API secret keys (sk-ant-...).</summary>
    private static readonly Regex AnthropicKeyRegex = new(@"\bsk-ant-[A-Za-z0-9_-]{20,}\b", RegexOptions.Compiled);

    /// <summary>Matches OpenAI API secret and project keys (sk-... or sk-proj-...).</summary>
    private static readonly Regex OpenAiKeyRegex = new(@"\bsk-(?!ant-)(?:proj-)?[A-Za-z0-9_-]{20,}\b", RegexOptions.Compiled);

    /// <summary>Matches GitHub Personal Access Tokens and OAuth tokens (ghp_, gho_, ghu_, ghs_, ghr_, github_pat_).</summary>
    private static readonly Regex GitHubTokenRegex = new(@"\b(?:ghp|gho|ghu|ghs|ghr)_[A-Za-z0-9_]{36,}\b|\bgithub_pat_[A-Za-z0-9_]{82}\b", RegexOptions.Compiled);

    /// <summary>Matches AWS Access Key Identifiers (AKIA...).</summary>
    private static readonly Regex AwsKeyRegex = new(@"\bAKIA[0-9A-Z]{16}\b", RegexOptions.Compiled);

    /// <summary>Matches JSON Web Tokens composed of three Base64URL-encoded segments separated by periods.</summary>
    private static readonly Regex JwtRegex = new(@"\beyJ[A-Za-z0-9_-]{10,}\.eyJ[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{10,}\b", RegexOptions.Compiled);

    /// <summary>Matches PEM-encoded RSA, EC, DSA, or OPENSSH private key blocks.</summary>
    private static readonly Regex PrivateKeyRegex = new(@"-----BEGIN (?:RSA |EC |OPENSSH |DSA )?PRIVATE KEY-----[\s\S]*?-----END (?:RSA |EC |OPENSSH |DSA )?PRIVATE KEY-----", RegexOptions.Compiled);

    /// <summary>Matches explicit assignment statements of secrets, passwords, or tokens in source code and config files.</summary>
    private static readonly Regex KeyAssignmentRegex = new(@"(?i)\b(password|passwd|pwd|secret|api_key|apikey|auth_token|client_secret)(\s*[:=]\s*)([""'])(?:(?!\3).)+?\3", RegexOptions.Compiled);

    /// <summary>Matches HTTP Authorization Bearer token headers.</summary>
    private static readonly Regex BearerTokenRegex = new(@"(?i)Bearer\s+[A-Za-z0-9_\-\.]{20,}", RegexOptions.Compiled);

    /// <summary>Matches standard email address formats.</summary>
    private static readonly Regex EmailRegex = new(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b", RegexOptions.Compiled);

    /// <summary>Matches IPv4 dotted-quad addresses.</summary>
    private static readonly Regex IpRegex = new(@"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b", RegexOptions.Compiled);

    /// <summary>
    /// Executes sanitization on the given content according to the requested comment-removal and redaction flags.
    /// </summary>
    /// <param name="content">The original file content string.</param>
    /// <param name="fileExtension">The file extension used to apply language-specific syntax rules.</param>
    /// <param name="removeComments">If true, strips language-specific comments.</param>
    /// <param name="redactSecrets">If true, replaces detected sensitive credentials with redaction markers.</param>
    /// <returns>The sanitized file content.</returns>
    public string Clean(string content, string fileExtension, bool removeComments, bool redactSecrets)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var result = content;

        if (removeComments)
        {
            result = RemoveComments(result, fileExtension);
        }

        if (redactSecrets)
        {
            result = RedactSensitiveData(result);
        }

        return result;
    }

    /// <summary>
    /// Strips line and block comments from source code content based on the language grammar of the file extension,
    /// while preserving string literals and minimizing whitespace gaps.
    /// </summary>
    /// <param name="content">The source code content containing comments.</param>
    /// <param name="fileExtension">The target language extension (e.g. ".cs", ".js", ".py", ".sql").</param>
    /// <returns>The source code with comments stripped.</returns>
    public string RemoveComments(string content, string fileExtension)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var ext = fileExtension.ToLowerInvariant().TrimStart('.');
        var result = content;

        switch (ext)
        {
            case "cs":
            case "js":
            case "ts":
            case "tsx":
            case "jsx":
            case "java":
            case "cpp":
            case "c":
            case "h":
            case "hpp":
            case "go":
            case "rs":
            case "kt":
            case "swift":
            case "php":
            case "css":
            case "scss":
            case "less":
                // C-style comments: strip if starts with // or /*, keep if it's a string literal
                result = CFamilyTokensRegex.Replace(result, m =>
                {
                    var val = m.Value;
                    return val.StartsWith("//") || val.StartsWith("/*") ? string.Empty : val;
                });
                break;

            case "py":
            case "rb":
            case "sh":
            case "bash":
            case "yaml":
            case "yml":
            case "r":
                // Hash-style comments: strip if starts with #, keep if it's a quoted string
                result = HashTokensRegex.Replace(result, m =>
                {
                    var val = m.Value;
                    return val.StartsWith("#") ? string.Empty : val;
                });
                break;

            case "html":
            case "xml":
            case "svg":
            case "xaml":
            case "vue":
                // Markup comments: remove <!-- ... -->
                result = HtmlXmlCommentRegex.Replace(result, string.Empty);
                break;

            case "sql":
                // SQL comments: strip if starts with -- or /*, keep if it's a string literal
                result = SqlTokensRegex.Replace(result, m =>
                {
                    var val = m.Value;
                    return val.StartsWith("--") || val.StartsWith("/*") ? string.Empty : val;
                });
                break;

            case "ini":
            case "conf":
                // Configuration comments: strip if line begins with ; or #
                result = IniTokensRegex.Replace(result, m =>
                {
                    var val = m.Value;
                    return val.StartsWith(";") || val.StartsWith("#") ? string.Empty : val;
                });
                break;
        }

        // Compress multiple blank lines down to a clean double-newline
        result = MultipleBlankLinesRegex.Replace(result, "\n\n");
        return result.Trim();
    }

    /// <summary>
    /// Scans text for high-risk sensitive patterns (API tokens, private keys, passwords, IP addresses)
    /// and replaces them with safe redaction placeholder tokens.
    /// </summary>
    /// <param name="content">The text content to inspect.</param>
    /// <returns>The sanitized text with sensitive secrets masked.</returns>
    public string RedactSensitiveData(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var result = content;

        // 1. Scrub PEM private keys first (multi-line block)
        result = PrivateKeyRegex.Replace(result, "-----BEGIN PRIVATE KEY-----\n[REDACTED_PRIVATE_KEY]\n-----END PRIVATE KEY-----");

        // 2. Scrub specific known provider secret formats
        result = OpenAiKeyRegex.Replace(result, "sk-[REDACTED_OPENAI_KEY]");
        result = AnthropicKeyRegex.Replace(result, "sk-ant-[REDACTED_ANTHROPIC_KEY]");
        result = GitHubTokenRegex.Replace(result, "ghp_[REDACTED_GITHUB_TOKEN]");
        result = AwsKeyRegex.Replace(result, "AKIA[REDACTED_AWS_KEY]");
        result = BearerTokenRegex.Replace(result, "Bearer [REDACTED_BEARER_TOKEN]");
        result = JwtRegex.Replace(result, "eyJ[REDACTED_JWT_TOKEN]");

        // 3. Scrub variable/key-value assignments: password = "..." preserving delimiters
        result = KeyAssignmentRegex.Replace(result, "$1$2$3<REDACTED_SECRET>$3");

        // 4. Scrub email addresses
        result = EmailRegex.Replace(result, "<REDACTED_EMAIL>");

        // 5. Scrub IP addresses (skipping local loopback and broadcast defaults)
        result = IpRegex.Replace(result, match =>
        {
            var val = match.Value;
            if (val == "127.0.0.1" || val == "0.0.0.0" || val == "255.255.255.255")
                return val;
            return "<REDACTED_IP>";
        });

        return result;
    }
}
