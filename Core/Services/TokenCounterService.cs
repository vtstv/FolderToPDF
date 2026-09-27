// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Enums;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Models;
using SharpToken;

namespace FolderToPDF.Core.Services;

/// <summary>
/// Provides accurate Byte-Pair Encoding (BPE) token counting using SharpToken,
/// heuristic estimation models for Claude and Gemini, line count metrics, and context window analysis.
/// </summary>
public class TokenCounterService : ITokenCounterService
{
    private readonly GptEncoding? _cl100kEncoder;
    private readonly GptEncoding? _o200kEncoder;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenCounterService"/> class,
    /// loading standard OpenAI BPE vocabularies (cl100k_base and o200k_base).
    /// </summary>
    public TokenCounterService()
    {
        try
        {
            _cl100kEncoder = GptEncoding.GetEncoding("cl100k_base");
        }
        catch
        {
            _cl100kEncoder = null;
        }

        try
        {
            // Load o200k_base if supported by the installed SharpToken version
            _o200kEncoder = GptEncoding.GetEncoding("o200k_base");
        }
        catch
        {
            // Fallback gracefully to cl100k encoder if o200k is unavailable
            _o200kEncoder = _cl100kEncoder;
        }
    }

    /// <summary>
    /// Counts tokens for the given text string using the specified tokenizer model or algorithm.
    /// </summary>
    /// <param name="text">The text to tokenize.</param>
    /// <param name="model">The target tokenizer model.</param>
    /// <returns>The calculated number of tokens.</returns>
    public int CountTokens(string text, TokenizerModel model)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        try
        {
            switch (model)
            {
                case TokenizerModel.O200kBase:
                    if (_o200kEncoder != null)
                        return _o200kEncoder.Encode(text).Count;
                    return (int)Math.Ceiling(text.Length / 3.7);

                case TokenizerModel.Claude:
                    // Anthropic Claude 3.5 / 3.7 Sonnet: conservative punctuation and number tokenization,
                    // averaging ~3.35 characters per token in source code, or ~1.05x of cl100k.
                    if (_cl100kEncoder != null)
                        return (int)Math.Ceiling(_cl100kEncoder.Encode(text).Count * 1.05);
                    return (int)Math.Ceiling(text.Length / 3.35);

                case TokenizerModel.Gemini:
                    // Google Gemini 2.0 / 1.5 (256k SentencePiece):
                    // High code keyword compression with byte fallback, averaging ~3.7 chars per token.
                    if (_o200kEncoder != null)
                        return (int)Math.Ceiling(_o200kEncoder.Encode(text).Count * 0.98);
                    return (int)Math.Ceiling(text.Length / 3.7);

                case TokenizerModel.Llama3:
                    // Meta Llama 3 / 3.1 / 3.2 / 3.3 (128k BPE vocabulary):
                    // Tiktoken-like BPE regex, averaging ~0.99x of cl100k / 1.02x of o200k.
                    if (_o200kEncoder != null)
                        return (int)Math.Ceiling(_o200kEncoder.Encode(text).Count * 1.02);
                    return (int)Math.Ceiling(text.Length / 3.6);

                case TokenizerModel.DeepSeek:
                    // DeepSeek-V3 / DeepSeek-R1 (128k byte-level BPE):
                    // Exceptionally dense compression for programming languages, averaging ~3.8 chars per token.
                    if (_o200kEncoder != null)
                        return (int)Math.Ceiling(_o200kEncoder.Encode(text).Count * 0.95);
                    return (int)Math.Ceiling(text.Length / 3.8);

                case TokenizerModel.Cl100kBase:
                default:
                    if (_cl100kEncoder != null)
                        return _cl100kEncoder.Encode(text).Count;
                    return (int)Math.Ceiling(text.Length / 4.0);
            }
        }
        catch
        {
            // Heuristic fallback if tokenizer encounters rare invalid UTF-8 sequences
            return (int)Math.Ceiling(text.Length / 4.0);
        }
    }

    /// <summary>
    /// Counts the total number of lines in a text string without allocating an array of strings.
    /// </summary>
    /// <param name="text">The text to inspect.</param>
    /// <returns>The count of lines (0 for null or empty strings).</returns>
    public int CountLines(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        int lines = 1;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
                lines++;
        }
        return lines;
    }

    /// <summary>
    /// Generates detailed token metrics, character metrics, and detects unusually large tokens in a single string.
    /// </summary>
    /// <param name="text">The text content to analyze.</param>
    /// <param name="model">The tokenizer model to apply.</param>
    /// <returns>A populated <see cref="TokenMetrics"/> instance.</returns>
    public TokenMetrics AnalyzeText(string text, TokenizerModel model)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new TokenMetrics();
        }

        int totalTokens = CountTokens(text, model);
        int totalLines = CountLines(text);
        int totalChars = text.Length;

        var largeTokens = new List<string>();

        // Detect large individual token fragments (e.g. minified code or base64 data)
        if (_cl100kEncoder != null && totalTokens > 0)
        {
            try
            {
                var encoded = _cl100kEncoder.Encode(text);
                int limit = Math.Min(encoded.Count, 500);
                for (int i = 0; i < limit; i++)
                {
                    var piece = _cl100kEncoder.Decode(new List<int> { encoded[i] });
                    if (piece.Length > 20 && !largeTokens.Contains(piece))
                    {
                        largeTokens.Add(piece);
                        if (largeTokens.Count >= 15)
                            break;
                    }
                }
            }
            catch
            {
                // Silently ignore piece decoding exceptions
            }
        }

        return new TokenMetrics
        {
            TotalTokens = totalTokens,
            TotalLines = totalLines,
            TotalCharacters = totalChars,
            LargeTokens = largeTokens
        };
    }

    /// <summary>
    /// Aggregates token and line metrics across all included files in a scanned file collection.
    /// </summary>
    /// <param name="files">The scanned files to aggregate.</param>
    /// <param name="model">The tokenizer model to apply.</param>
    /// <returns>A populated <see cref="TokenMetrics"/> instance.</returns>
    public TokenMetrics AnalyzeFiles(IEnumerable<ScannedFile> files, TokenizerModel model)
    {
        int totalTokens = 0;
        int totalLines = 0;
        int totalChars = 0;
        var largeTokens = new List<string>();

        foreach (var file in files.Where(f => f.IsIncluded))
        {
            var content = string.IsNullOrEmpty(file.CleanedContent) ? file.RawContent : file.CleanedContent;
            totalTokens += file.TokenCount > 0 ? file.TokenCount : CountTokens(content, model);
            totalLines += file.LineCount > 0 ? file.LineCount : CountLines(content);
            totalChars += content.Length;
        }

        return new TokenMetrics
        {
            TotalTokens = totalTokens,
            TotalLines = totalLines,
            TotalCharacters = totalChars,
            LargeTokens = largeTokens
        };
    }
}
