// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;
using FolderToPDF.Core.Services;
using Xunit;

namespace FolderToPDF.Tests;

public class FileScannerServiceTests : IDisposable
{
    private readonly string _tempTestDir;
    private readonly FileScannerService _scannerService;

    public FileScannerServiceTests()
    {
        _tempTestDir = Path.Combine(Path.GetTempPath(), "FolderToPDF_Test_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempTestDir);

        var cleaner = new ContentCleanerService();
        var tokenCounter = new TokenCounterService();
        _scannerService = new FileScannerService(cleaner, tokenCounter);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempTestDir))
            {
                Directory.Delete(_tempTestDir, true);
            }
        }
        catch { }
    }

    [Fact]
    public async Task ScanDirectoryAsync_FindsFilesAndAppliesExclusions()
    {
        // Setup folder structure
        var srcDir = Path.Combine(_tempTestDir, "src");
        var nodeModulesDir = Path.Combine(_tempTestDir, "node_modules");
        var binDir = Path.Combine(_tempTestDir, "bin");
        Directory.CreateDirectory(srcDir);
        Directory.CreateDirectory(nodeModulesDir);
        Directory.CreateDirectory(binDir);

        File.WriteAllText(Path.Combine(srcDir, "App.cs"), "// Test C# file\npublic class App {}");
        File.WriteAllText(Path.Combine(srcDir, "config.json"), "{ \"env\": \"test\" }");
        File.WriteAllText(Path.Combine(srcDir, "bundle.min.js"), "console.log('minified');");
        File.WriteAllText(Path.Combine(nodeModulesDir, "ignored.js"), "console.log('ignored');");
        File.WriteAllText(Path.Combine(binDir, "app.dll"), "binary");

        var options = new ScanOptions
        {
            RootDirectory = _tempTestDir,
            FileTypes = new() { "*.cs", "*.json", "*.js" },
            ExcludeFolders = new() { "node_modules", "bin" },
            ExcludeFiles = new() { "*.min.js" },
            RemoveComments = true,
            RedactSecrets = false
        };

        var result = await _scannerService.ScanDirectoryAsync(options);

        Assert.Empty(result.Errors);
        Assert.Equal(2, result.Files.Count);

        var foundFileNames = result.Files.Select(f => f.FileName).ToList();
        Assert.Contains("App.cs", foundFileNames);
        Assert.Contains("config.json", foundFileNames);
        Assert.DoesNotContain("bundle.min.js", foundFileNames);
        Assert.DoesNotContain("ignored.js", foundFileNames);

        var csFile = result.Files.First(f => f.FileName == "App.cs");
        Assert.DoesNotContain("// Test C# file", csFile.CleanedContent);
        Assert.Contains("public class App {}", csFile.CleanedContent);
    }

    [Fact]
    public void GenerateDirectoryTree_ProducesReadableTree()
    {
        var relativePaths = new List<string>
        {
            Path.Combine("src", "Core", "Engine.cs"),
            Path.Combine("src", "Utils", "Logger.cs"),
            "README.md"
        };

        var tree = _scannerService.GenerateDirectoryTree(_tempTestDir, relativePaths);

        Assert.Contains("src/", tree);
        Assert.Contains("Core/", tree);
        Assert.Contains("Engine.cs", tree);
        Assert.Contains("README.md", tree);
    }

    [Fact]
    public async Task ScanDirectoryAsync_NonExistentDirectory_ReturnsError()
    {
        var options = new ScanOptions
        {
            RootDirectory = Path.Combine(Path.GetTempPath(), "NonExistent_" + Guid.NewGuid().ToString("N"))
        };

        var result = await _scannerService.ScanDirectoryAsync(options);

        Assert.NotEmpty(result.Errors);
        Assert.Empty(result.Files);
    }

    [Fact]
    public async Task ScanDirectoryAsync_SkipsBinaryFilesWithNullBytes()
    {
        var binPath = Path.Combine(_tempTestDir, "test.dat");
        File.WriteAllBytes(binPath, new byte[] { 0x48, 0x65, 0x6C, 0x00, 0x6F }); // contains null byte

        var options = new ScanOptions
        {
            RootDirectory = _tempTestDir,
            FileTypes = new() { "*.dat" }
        };

        var result = await _scannerService.ScanDirectoryAsync(options);

        Assert.Empty(result.Files);
        Assert.Single(result.SkippedFiles);
        Assert.Contains("Binary file skipped", result.SkippedFiles[0]);
    }

    [Fact]
    public async Task ScanDirectoryAsync_SkipsFilesExceedingMaxFileSize()
    {
        var largeFilePath = Path.Combine(_tempTestDir, "large.txt");
        File.WriteAllText(largeFilePath, new string('A', 5000));

        var options = new ScanOptions
        {
            RootDirectory = _tempTestDir,
            FileTypes = new() { "*.txt" },
            MaxFileSizeInBytes = 1000 // Limit to 1 KB
        };

        var result = await _scannerService.ScanDirectoryAsync(options);

        Assert.Empty(result.Files);
        Assert.Single(result.SkippedFiles);
        Assert.Contains("Exceeded max size", result.SkippedFiles[0]);
    }

    [Fact]
    public async Task ScanDirectoryAsync_TruncatesExcessivelyLongContent()
    {
        var longFilePath = Path.Combine(_tempTestDir, "long.txt");
        File.WriteAllText(longFilePath, new string('X', 500));

        var options = new ScanOptions
        {
            RootDirectory = _tempTestDir,
            FileTypes = new() { "*.txt" },
            MaxFileSizeInBytes = 100_000,
            MaxContentLengthPerFile = 100 // Truncate at 100 chars
        };

        var result = await _scannerService.ScanDirectoryAsync(options);

        Assert.Single(result.Files);
        Assert.Contains("Content truncated at 100 characters", result.Files[0].RawContent);
    }

    [Fact]
    public void GenerateDirectoryTree_HandlesEmptyOrWhitespaceRoot()
    {
        var relativePaths = new List<string> { "file1.cs", "dir/file2.cs" };
        var tree = _scannerService.GenerateDirectoryTree("", relativePaths);

        Assert.Contains("Root/", tree);
        Assert.Contains("file1.cs", tree);
        Assert.Contains("dir/", tree);
    }
}
