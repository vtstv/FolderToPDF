// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Services;

/// <summary>
/// Provides high-speed asynchronous directory scanning, file pattern filtering,
/// binary content detection, content sanitization, and ASCII directory tree generation.
/// </summary>
public class FileScannerService : IFileScannerService
{
    private readonly IContentCleanerService _cleanerService;
    private readonly ITokenCounterService _tokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileScannerService"/> class.
    /// </summary>
    /// <param name="cleanerService">Service used to strip comments and scrub sensitive credentials.</param>
    /// <param name="tokenService">Service used to calculate token counts and line counts.</param>
    public FileScannerService(IContentCleanerService cleanerService, ITokenCounterService tokenService)
    {
        _cleanerService = cleanerService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Traverses the specified root directory asynchronously, loading, sanitizing, and tokenizing matching files.
    /// </summary>
    /// <param name="options">Scan options including directory path, patterns, and exclusion lists.</param>
    /// <param name="progress">Optional progress reporter for progress notifications.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the scan operation.</param>
    /// <returns>A <see cref="ScanResult"/> containing the collection of scanned files, metrics, and errors.</returns>
    public async Task<ScanResult> ScanDirectoryAsync(
        ScanOptions options,
        IProgress<ScanProgressReport>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var result = new ScanResult
        {
            RootDirectory = options.RootDirectory
        };

        if (string.IsNullOrWhiteSpace(options.RootDirectory) || !Directory.Exists(options.RootDirectory))
        {
            result.Errors.Add("Selected root directory does not exist or path is empty.");
            return result;
        }

        await Task.Run(() =>
        {
            // 1. Gather files recursively while respecting folder exclusions
            var discoveredFilePaths = new List<string>();
            try
            {
                CollectFilesRecursively(options.RootDirectory, options, discoveredFilePaths, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                result.Errors.Add("Scan was cancelled by the user.");
                return;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Directory traversal error: {ex.Message}");
                return;
            }

            int totalFiles = discoveredFilePaths.Count;
            int processed = 0;

            // 2. Read and process files sequentially
            foreach (var filePath in discoveredFilePaths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                processed++;
                var fileName = Path.GetFileName(filePath);
                var relativePath = Path.GetRelativePath(options.RootDirectory, filePath);

                progress?.Report(new ScanProgressReport
                {
                    ProcessedFiles = processed,
                    TotalEstimatedFiles = totalFiles,
                    CurrentFile = relativePath,
                    StatusMessage = $"Scanning {processed}/{totalFiles}: {fileName}"
                });

                try
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > options.MaxFileSizeInBytes)
                    {
                        result.SkippedFiles.Add($"{relativePath} (Exceeded max size: {fileInfo.Length / 1024} KB)");
                        continue;
                    }

                    // Check for null bytes to skip compiled binaries, images, or archive formats
                    if (IsBinaryFile(filePath))
                    {
                        result.SkippedFiles.Add($"{relativePath} (Binary file skipped)");
                        continue;
                    }

                    var rawContent = File.ReadAllText(filePath, Encoding.UTF8);

                    // Truncate if individual file is excessively long to prevent memory exhaustion
                    if (rawContent.Length > options.MaxContentLengthPerFile)
                    {
                        rawContent = rawContent.Substring(0, options.MaxContentLengthPerFile) +
                                     $"\n\n... [Content truncated at {options.MaxContentLengthPerFile:N0} characters by FolderToPDF] ...";
                    }

                    var cleanedContent = _cleanerService.Clean(
                        rawContent,
                        fileInfo.Extension,
                        options.RemoveComments,
                        options.RedactSecrets);

                    int lines = _tokenService.CountLines(cleanedContent);
                    int tokens = _tokenService.CountTokens(cleanedContent, options.Tokenizer);

                    var scannedFile = new ScannedFile
                    {
                        FullPath = filePath,
                        RelativePath = relativePath,
                        FileName = fileName,
                        Extension = fileInfo.Extension,
                        SizeInBytes = fileInfo.Length,
                        RawContent = rawContent,
                        CleanedContent = cleanedContent,
                        LineCount = lines,
                        TokenCount = tokens,
                        IsIncluded = true
                    };

                    result.Files.Add(scannedFile);
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Failed to read '{relativePath}': {ex.Message}");
                }
            }
        }, cancellationToken);

        result.Duration = DateTime.UtcNow - startTime;
        return result;
    }

    /// <summary>
    /// Recursively enumerates files from the directory tree, pruning excluded folders and checking inclusion patterns.
    /// </summary>
    private void CollectFilesRecursively(
        string currentDir,
        ScanOptions options,
        List<string> collectedFiles,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dirName = Path.GetFileName(currentDir);
        if (!string.IsNullOrEmpty(dirName) && options.ExcludeFolders.Any(ex => 
            string.Equals(ex.Trim(), dirName, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        try
        {
            var files = Directory.GetFiles(currentDir);
            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fileName = Path.GetFileName(file);

                // Check file exclusion patterns first
                if (options.ExcludeFiles.Any(mask => MatchesPattern(fileName, mask)))
                {
                    continue;
                }

                // Check extension wildcard inclusion or explicit inclusion
                bool matchedType = options.FileTypes.Count == 0 ||
                                   options.FileTypes.Any(mask => MatchesPattern(fileName, mask));

                bool matchedInclude = options.IncludeFiles.Count > 0 &&
                                      options.IncludeFiles.Any(mask => MatchesPattern(fileName, mask));

                if (matchedType || matchedInclude)
                {
                    collectedFiles.Add(file);
                }
            }

            var subDirs = Directory.GetDirectories(currentDir);
            foreach (var subDir in subDirs)
            {
                CollectFilesRecursively(subDir, options, collectedFiles, cancellationToken);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Silently skip inaccessible system or permission-restricted folders
        }
        catch (PathTooLongException)
        {
            // Silently skip paths exceeding Windows MAX_PATH limits
        }
    }

    /// <summary>
    /// Generates an ASCII/Unicode directory tree view representing the structure of all included files.
    /// </summary>
    /// <param name="rootPath">The root directory path.</param>
    /// <param name="includedRelativePaths">List of relative paths to represent in the tree.</param>
    /// <returns>A formatted multi-line directory tree string.</returns>
    public string GenerateDirectoryTree(string rootPath, IEnumerable<string> includedRelativePaths)
    {
        var sb = new StringBuilder();
        var rootName = GetSafeDirectoryName(rootPath);
        sb.AppendLine(rootName + "/");

        var sortedPaths = includedRelativePaths.OrderBy(p => p).ToList();
        var visitedDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in sortedPaths)
        {
            var parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var currentPath = "";

            for (int i = 0; i < parts.Length; i++)
            {
                currentPath = i == 0 ? parts[0] : currentPath + "/" + parts[i];
                var indent = new string(' ', i * 2);

                if (i == parts.Length - 1)
                {
                    // Leaf file node
                    sb.AppendLine($"{indent}├── {parts[i]}");
                }
                else if (visitedDirs.Add(currentPath))
                {
                    // Intermediate directory node
                    sb.AppendLine($"{indent}├── {parts[i]}/");
                }
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Evaluates whether a filename satisfies a wildcard pattern (e.g. *.cs, *test*, *.min.js).
    /// </summary>
    private static bool MatchesPattern(string filename, string pattern)
    {
        pattern = pattern.Trim();
        if (string.IsNullOrEmpty(pattern) || pattern == "*.*" || pattern == "*")
            return true;

        if (pattern.Contains('*') || pattern.Contains('?'))
        {
            var regex = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return Regex.IsMatch(filename, regex, RegexOptions.IgnoreCase);
        }

        if (pattern.StartsWith("."))
        {
            return filename.EndsWith(pattern, StringComparison.OrdinalIgnoreCase);
        }

        return filename.Equals(pattern, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Inspects the initial bytes of a file to check for null bytes, indicating binary contents (images, executables, dlls).
    /// </summary>
    private static bool IsBinaryFile(string filePath)
    {
        try
        {
            using var stream = File.OpenRead(filePath);
            byte[] buffer = new byte[Math.Min(512, (int)stream.Length)];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            for (int i = 0; i < bytesRead; i++)
            {
                if (buffer[i] == 0) // Null byte indicator for binary files
                    return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Safely resolves the directory name for display, handling root drives and trailing separators.
    /// </summary>
    private static string GetSafeDirectoryName(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "Root";

        try
        {
            var trimmed = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var name = Path.GetFileName(trimmed);
            return string.IsNullOrWhiteSpace(name) ? (string.IsNullOrWhiteSpace(trimmed) ? "Root" : trimmed) : name;
        }
        catch
        {
            return "Root";
        }
    }
}
