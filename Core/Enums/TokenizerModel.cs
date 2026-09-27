// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

namespace FolderToPDF.Core.Enums;

/// <summary>
/// Specifies the AI tokenizer model or heuristic algorithm used to count prompt tokens and estimate context window consumption.
/// </summary>
public enum TokenizerModel
{
    /// <summary>
    /// High-efficiency byte-pair encoding tokenizer for OpenAI GPT-4o, GPT-4o-mini, o1, o3, and flagship models (SharpToken o200k_base).
    /// </summary>
    O200kBase,

    /// <summary>
    /// Calibrated code-aware tokenizer algorithm for Anthropic Claude models (Claude 3.5 Sonnet, 3.7 Sonnet, Opus, Haiku).
    /// </summary>
    Claude,

    /// <summary>
    /// SentencePiece 256k vocabulary byte-pair encoding algorithm for Google Gemini models (Gemini 2.0 Flash/Pro, Gemini 1.5 Pro/Flash).
    /// </summary>
    Gemini,

    /// <summary>
    /// Byte-pair encoding tokenizer for Meta Llama open-weight models (Llama 3, 3.1, 3.2, 3.3 128k vocabulary).
    /// </summary>
    Llama3,

    /// <summary>
    /// Byte-level multi-lingual code BPE tokenizer for DeepSeek models (DeepSeek-V3, DeepSeek-R1 128k vocabulary).
    /// </summary>
    DeepSeek,

    /// <summary>
    /// Byte-pair encoding tokenizer for OpenAI GPT-4, GPT-3.5-Turbo, and text-embedding-ada-002 models (SharpToken cl100k_base).
    /// </summary>
    Cl100kBase
}
