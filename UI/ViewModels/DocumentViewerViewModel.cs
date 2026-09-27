// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FolderToPDF.UI.ViewModels;

/// <summary>
/// ViewModel managing the embedded in-app document previewer for generated PDF, Markdown, and TXT archives.
/// </summary>
public partial class DocumentViewerViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _currentFilePath;

    [ObservableProperty]
    private string _documentTitle = "No Document Loaded";

    [ObservableProperty]
    private string _documentFormat = "None";

    [ObservableProperty]
    private string _textContent = string.Empty;

    [ObservableProperty]
    private bool _isPdf;

    [ObservableProperty]
    private bool _isText;

    [ObservableProperty]
    private bool _hasDocument;

    [ObservableProperty]
    private string _statusMessage = "Select an export from History or Scanner to preview.";

    /// <summary>Fires when a new document path is loaded to trigger WebView2 navigation.</summary>
    public event EventHandler<string>? DocumentLoaded;

    /// <summary>
    /// Loads a document from the local file system into the embedded previewer.
    /// </summary>
    /// <param name="filePath">Target file path to display.</param>
    public void LoadDocument(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            StatusMessage = "Document not found on disk.";
            HasDocument = false;
            return;
        }

        CurrentFilePath = filePath;
        DocumentTitle = Path.GetFileName(filePath);
        var ext = Path.GetExtension(filePath).ToLowerInvariant();

        HasDocument = true;

        if (ext == ".pdf")
        {
            IsPdf = true;
            IsText = false;
            DocumentFormat = "PDF Document";
            TextContent = string.Empty;
            StatusMessage = $"Viewing PDF: {DocumentTitle}";
            DocumentLoaded?.Invoke(this, filePath);
        }
        else
        {
            IsPdf = false;
            IsText = true;
            DocumentFormat = ext == ".md" ? "Markdown Document" : "Plain Text Archive";
            try
            {
                TextContent = File.ReadAllText(filePath);
                StatusMessage = $"Viewing {DocumentFormat}: {DocumentTitle}";
            }
            catch (Exception ex)
            {
                TextContent = $"Error loading file: {ex.Message}";
                StatusMessage = "Error reading file content.";
            }
            DocumentLoaded?.Invoke(this, filePath);
        }
    }

    /// <summary>Closes the current document preview.</summary>
    [RelayCommand]
    private void CloseDocument()
    {
        CurrentFilePath = null;
        DocumentTitle = "No Document Loaded";
        DocumentFormat = "None";
        TextContent = string.Empty;
        IsPdf = false;
        IsText = false;
        HasDocument = false;
        StatusMessage = "Document closed.";
    }

    /// <summary>Launches the loaded document in the operating system's default viewer.</summary>
    [RelayCommand]
    private void OpenInExternalApp()
    {
        if (!string.IsNullOrEmpty(CurrentFilePath) && File.Exists(CurrentFilePath))
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = CurrentFilePath, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to open in external viewer: {ex.Message}";
            }
        }
    }

    /// <summary>Opens Windows File Explorer with the current document highlighted.</summary>
    [RelayCommand]
    private void OpenContainingFolder()
    {
        if (!string.IsNullOrEmpty(CurrentFilePath) && File.Exists(CurrentFilePath))
        {
            Process.Start("explorer.exe", $"/select,\"{CurrentFilePath}\"");
        }
    }

    /// <summary>Copies the document's absolute file path to the system clipboard.</summary>
    [RelayCommand]
    private void CopyFilePath()
    {
        if (!string.IsNullOrEmpty(CurrentFilePath))
        {
            try
            {
                Clipboard.SetText(CurrentFilePath);
                StatusMessage = "File path copied to clipboard!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to copy path: {ex.Message}";
            }
        }
    }
}
