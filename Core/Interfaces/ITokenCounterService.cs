// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Enums;
using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Interfaces;

/// <summary>
/// Calculates accurate BPE (Byte Pair Encoding) token counts, line counts, character metrics,
/// and AI context window utilization estimates across selected tokenizer models.
/// </summary>
public interface ITokenCounterService
{
    /// <summary>
    /// Calculates the token count of a given string using the specified tokenizer model.
    /// </summary>
    /// <param name="text">The text content to tokenize.</param>
    /// <param name="model">The tokenizer model or estimation algorithm to apply.</param>
    /// <returns>The calculated number of tokens.</returns>
    int CountTokens(string text, TokenizerModel model);

    /// <summary>
    /// Counts the number of newline-separated lines within a text string.
    /// </summary>
    /// <param name="text">The text content to analyze.</param>
    /// <returns>The number of lines in the text (0 for empty strings).</returns>
    int CountLines(string text);

    /// <summary>
    /// Computes full token and line metrics, detecting oversized tokens and context window percentages for a text block.
    /// </summary>
    /// <param name="text">The text content to analyze.</param>
    /// <param name="model">The tokenizer model or estimation algorithm to use.</param>
    /// <returns>A populated <see cref="TokenMetrics"/> instance.</returns>
    TokenMetrics AnalyzeText(string text, TokenizerModel model);

    /// <summary>
    /// Computes aggregate token and line metrics across a collection of scanned files.
    /// </summary>
    /// <param name="files">The scanned files to aggregate.</param>
    /// <param name="model">The tokenizer model to use for metrics.</param>
    /// <returns>A populated <see cref="TokenMetrics"/> instance containing aggregate stats.</returns>
    TokenMetrics AnalyzeFiles(IEnumerable<ScannedFile> files, TokenizerModel model);
}
