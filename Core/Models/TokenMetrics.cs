// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

namespace FolderToPDF.Core.Models;

/// <summary>
/// Provides a detailed statistical breakdown of token metrics, characters, lines, and LLM context window utilization percentages.
/// </summary>
public class TokenMetrics
{
    /// <summary>
    /// Gets or sets the total number of BPE or heuristic tokens in the analyzed text.
    /// </summary>
    public int TotalTokens { get; set; }

    /// <summary>
    /// Gets or sets the total number of newline-separated lines in the analyzed text.
    /// </summary>
    public int TotalLines { get; set; }

    /// <summary>
    /// Gets or sets the total number of characters in the analyzed text.
    /// </summary>
    public int TotalCharacters { get; set; }

    /// <summary>
    /// Gets or sets a collection of token strings that exceed standard length thresholds (e.g. minified code or embedded payloads).
    /// </summary>
    public List<string> LargeTokens { get; set; } = new();

    /// <summary>
    /// Gets the percentage of a 32,768 token context window (e.g. GPT-4 32k) consumed by these tokens, clamped to 100%.
    /// </summary>
    public double PercentOf32k => Math.Min(100.0, (double)TotalTokens / 32_768 * 100.0);

    /// <summary>
    /// Gets the percentage of a 128,000 token context window (e.g. GPT-4o 128k) consumed by these tokens, clamped to 100%.
    /// </summary>
    public double PercentOf128k => Math.Min(100.0, (double)TotalTokens / 128_000 * 100.0);

    /// <summary>
    /// Gets the percentage of a 200,000 token context window (e.g. Claude 3.5 Sonnet 200k) consumed by these tokens, clamped to 100%.
    /// </summary>
    public double PercentOf200k => Math.Min(100.0, (double)TotalTokens / 200_000 * 100.0);

    /// <summary>
    /// Gets the percentage of a 1,000,000 token context window (e.g. Gemini 1.5 Pro 1M) consumed by these tokens, clamped to 100%.
    /// </summary>
    public double PercentOf1M => Math.Min(100.0, (double)TotalTokens / 1_000_000 * 100.0);

    /// <summary>
    /// Gets the percentage of a 2,000,000 token context window (e.g. Gemini 2.0 / 1.5 Pro 2M) consumed by these tokens, clamped to 100%.
    /// </summary>
    public double PercentOf2M => Math.Min(100.0, (double)TotalTokens / 2_000_000 * 100.0);
}
