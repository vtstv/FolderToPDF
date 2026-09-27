// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using FolderToPDF.Core.Models;
using Microsoft.Win32;

namespace FolderToPDF.UI.ViewModels;

/// <summary>
/// Partial class for <see cref="ScannerViewModel"/> containing export orchestration, file dialogs, and OS process launching.
/// </summary>
public partial class ScannerViewModel
{
    /// <summary>Prompts save dialog and exports scanned codebase into a formatted PDF document.</summary>
    [RelayCommand]
    private async Task ExportPdfAsync()
    {
        await ExecuteExportAsync("pdf", async (path, files, progress, token) =>
        {
            await _pdfExportService.ExportAsync(path, RootDirectory, files, 
                IncludeFileTreeHeader ? _directoryTree : null, _settingsService.CurrentSettings, progress, token);
        });
    }

    /// <summary>Prompts save dialog and exports scanned codebase into a plain text archive.</summary>
    [RelayCommand]
    private async Task ExportTxtAsync()
    {
        await ExecuteExportAsync("txt", async (path, files, progress, token) =>
        {
            await _textExportService.ExportAsync(path, RootDirectory, files, 
                IncludeFileTreeHeader ? _directoryTree : null, _settingsService.CurrentSettings, progress, token);
        });
    }

    /// <summary>Prompts save dialog and exports scanned codebase into a Markdown prompt document.</summary>
    [RelayCommand]
    private async Task ExportMarkdownAsync()
    {
        await ExecuteExportAsync("md", async (path, files, progress, token) =>
        {
            await _markdownExportService.ExportAsync(path, RootDirectory, files, 
                IncludeFileTreeHeader ? _directoryTree : null, _settingsService.CurrentSettings, progress, token);
        });
    }

    /// <summary>Formats the entire scanned codebase as Markdown and copies it straight to the clipboard.</summary>
    [RelayCommand]
    private void CopyToClipboard()
    {
        if (_scannedFiles.Count == 0)
        {
            StatusMessage = "No scanned files available. Run scan first.";
            return;
        }

        try
        {
            var content = _markdownExportService.FormatAsMarkdown(
                RootDirectory, _scannedFiles, IncludeFileTreeHeader ? _directoryTree : null, _settingsService.CurrentSettings);

            Clipboard.SetText(content);
            StatusMessage = "Full prompt context copied to clipboard!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to copy: {ex.Message}";
        }
    }

    /// <summary>Opens the last exported document using the system default associated application.</summary>
    [RelayCommand]
    private void OpenOutputFile()
    {
        if (File.Exists(LastExportedFilePath))
        {
            Process.Start(new ProcessStartInfo { FileName = LastExportedFilePath, UseShellExecute = true });
        }
        else
        {
            StatusMessage = "Exported file not found.";
        }
    }

    /// <summary>Opens the last exported document in the integrated in-app document viewer.</summary>
    [RelayCommand]
    private void PreviewLastExport()
    {
        if (!string.IsNullOrEmpty(LastExportedFilePath) && File.Exists(LastExportedFilePath))
        {
            OpenDocumentRequested?.Invoke(this, LastExportedFilePath);
        }
        else
        {
            StatusMessage = "No exported file available to preview. Export a document first.";
        }
    }

    /// <summary>Opens Windows File Explorer highlighting or navigated to the output directory.</summary>
    [RelayCommand]
    private void OpenOutputFolder()
    {
        var configuredDir = _settingsService.CurrentSettings.DefaultOutputDirectory;
        var defaultDir = Path.IsPathRooted(configuredDir)
            ? configuredDir
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuredDir);

        var targetDir = !string.IsNullOrEmpty(LastExportedFilePath) && File.Exists(LastExportedFilePath)
            ? Path.GetDirectoryName(LastExportedFilePath)
            : defaultDir;

        if (string.IsNullOrEmpty(targetDir)) return;

        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        Process.Start(new ProcessStartInfo { FileName = targetDir, UseShellExecute = true });
    }

    /// <summary>
    /// Displays a SaveFileDialog and runs the designated export delegate with cancellation and progress reporting.
    /// </summary>
    private async Task ExecuteExportAsync(string extension, Func<string, List<ScannedFile>, IProgress<ScanProgressReport>, CancellationToken, Task> exportAction)
    {
        if (_scannedFiles.Count == 0)
        {
            StatusMessage = "No files to export. Please run a scan first.";
            return;
        }

        var configuredDir = _settingsService.CurrentSettings.DefaultOutputDirectory;
        var initialDir = Path.IsPathRooted(configuredDir)
            ? configuredDir
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuredDir);

        if (!Directory.Exists(initialDir))
        {
            try { Directory.CreateDirectory(initialDir); } catch { }
        }

        var saveDialog = new SaveFileDialog
        {
            Title = $"Save {extension.ToUpperInvariant()} Archive",
            FileName = $"{OutputFileName}.{extension}",
            Filter = $"{extension.ToUpperInvariant()} files (*.{extension})|*.{extension}|All files (*.*)|*.*",
            InitialDirectory = initialDir
        };

        if (saveDialog.ShowDialog() != true)
            return;

        var targetPath = saveDialog.FileName;
        IsBusy = true;
        ProgressPercentage = 0;
        StatusMessage = $"Generating {extension.ToUpperInvariant()}...";
        _cancellationTokenSource = new CancellationTokenSource();

        var progress = new Progress<ScanProgressReport>(r =>
        {
            ProgressPercentage = r.ProgressPercentage;
            StatusMessage = r.StatusMessage;
        });

        try
        {
            await exportAction(targetPath, _scannedFiles, progress, _cancellationTokenSource.Token);
            LastExportedFilePath = targetPath;
            StatusMessage = $"Export complete: {Path.GetFileName(targetPath)}";

            // Record into Export History
            var fileInfo = new FileInfo(targetPath);
            await _exportHistoryService.AddItemAsync(new ExportHistoryItem
            {
                Id = Guid.NewGuid().ToString("N"),
                ProjectName = OutputFileName,
                RootDirectory = RootDirectory,
                OutputFilePath = targetPath,
                Format = extension.ToUpperInvariant(),
                CreatedAt = DateTimeOffset.Now,
                FileCount = _scannedFiles.Count(f => f.IsIncluded),
                TokenCount = TotalTokens,
                FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0
            });

            // Auto-open handling based on user settings
            if (_settingsService.CurrentSettings.AutoOpenOutputFolder)
            {
                OpenOutputFolder();
            }
            else if (_settingsService.CurrentSettings.AutoOpenExportedFile)
            {
                OpenOutputFile();
            }
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Export cancelled.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
