// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Enums;
using FolderToPDF.Core.Models;
using FolderToPDF.Core.Services;
using Xunit;

namespace FolderToPDF.Tests;

public class TokenCounterServiceTests
{
    private readonly TokenCounterService _tokenService = new();

    [Fact]
    public void CountLines_EmptyString_ReturnsZero()
    {
        Assert.Equal(0, _tokenService.CountLines(""));
        Assert.Equal(0, _tokenService.CountLines(null!));
    }

    [Fact]
    public void CountLines_MultipleLines_ReturnsCorrectCount()
    {
        var text = "Line 1\nLine 2\r\nLine 3\nLine 4";
        Assert.Equal(4, _tokenService.CountLines(text));
    }

    [Theory]
    [InlineData(TokenizerModel.O200kBase)]
    [InlineData(TokenizerModel.Claude)]
    [InlineData(TokenizerModel.Gemini)]
    [InlineData(TokenizerModel.Llama3)]
    [InlineData(TokenizerModel.DeepSeek)]
    [InlineData(TokenizerModel.Cl100kBase)]
    public void CountTokens_NonEmptyText_ReturnsPositiveCount(TokenizerModel model)
    {
        var sample = "public class Example { public string Name { get; set; } }";
        var count = _tokenService.CountTokens(sample, model);
        Assert.True(count > 0);
    }

    [Fact]
    public void AnalyzeText_ReturnsValidMetrics()
    {
        var sample = "Line 1\nLine 2\nLine 3\nLine 4\nLine 5";
        var metrics = _tokenService.AnalyzeText(sample, TokenizerModel.Cl100kBase);

        Assert.Equal(5, metrics.TotalLines);
        Assert.Equal(sample.Length, metrics.TotalCharacters);
        Assert.True(metrics.TotalTokens > 0);
    }

    [Fact]
    public void AnalyzeFiles_CalculatesAggregateStatistics()
    {
        var files = new List<ScannedFile>
        {
            new()
            {
                IsIncluded = true,
                LineCount = 10,
                TokenCount = 50,
                RawContent = "1234567890",
                CleanedContent = "1234567890"
            },
            new()
            {
                IsIncluded = false, // Excluded
                LineCount = 100,
                TokenCount = 500,
                RawContent = "excluded",
                CleanedContent = "excluded"
            },
            new()
            {
                IsIncluded = true,
                LineCount = 20,
                TokenCount = 100,
                RawContent = "12345678901234567890",
                CleanedContent = "12345678901234567890"
            }
        };

        var metrics = _tokenService.AnalyzeFiles(files, TokenizerModel.Cl100kBase);

        Assert.Equal(30, metrics.TotalLines);
        Assert.Equal(150, metrics.TotalTokens);
        Assert.Equal(30, metrics.TotalCharacters);
    }
}
