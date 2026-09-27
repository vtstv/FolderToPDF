// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderToPDF.Core.Models;

namespace FolderToPDF.UI.ViewModels;

/// <summary>
/// ViewModel managing the file preview browser, search filtering, and inclusion selection.
/// </summary>
public partial class FilePreviewViewModel : ObservableObject
{
    private List<ScannedFile> _allFiles = new();

    [ObservableProperty]
    private string _rootDirectory = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ScannedFile> _filteredFiles = new();

    [ObservableProperty]
    private ScannedFile? _selectedFile;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _showCleanedContent = true;

    [ObservableProperty]
    private string _previewContent = "Select a file to inspect its content.";

    [ObservableProperty]
    private string _selectedFileInfo = "No file selected.";

    /// <summary>
    /// Loads a new collection of scanned files and sets the scan root directory.
    /// </summary>
    /// <param name="files">The scanned files to display.</param>
    /// <param name="rootDir">The root directory path.</param>
    public void SetFiles(IEnumerable<ScannedFile> files, string rootDir)
    {
        _allFiles = files.ToList();
        RootDirectory = rootDir;
        ApplyFilter();
    }

    partial void OnSearchQueryChanged(string value) => ApplyFilter();

    partial void OnShowCleanedContentChanged(bool value) => UpdatePreview();

    partial void OnSelectedFileChanged(ScannedFile? value) => UpdatePreview();

    /// <summary>
    /// Filters the file list by filename or relative path substring matching.
    /// </summary>
    private void ApplyFilter()
    {
        FilteredFiles.Clear();
        var query = SearchQuery.Trim();

        foreach (var file in _allFiles)
        {
            if (string.IsNullOrEmpty(query) ||
                file.RelativePath.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                file.Extension.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                FilteredFiles.Add(file);
            }
        }

        if (SelectedFile == null || !FilteredFiles.Contains(SelectedFile))
        {
            SelectedFile = FilteredFiles.FirstOrDefault();
        }
    }

    /// <summary>
    /// Refreshes the preview viewer pane based on currently selected file and clean/raw toggle.
    /// </summary>
    private void UpdatePreview()
    {
        if (SelectedFile == null)
        {
            PreviewContent = "Select a file from the list to preview.";
            SelectedFileInfo = "No file selected.";
            return;
        }

        SelectedFileInfo = $"{SelectedFile.RelativePath}  •  {SelectedFile.LineCount:N0} lines  •  {SelectedFile.TokenCount:N0} tokens  •  {SelectedFile.SizeInBytes / 1024.0:F1} KB";

        PreviewContent = ShowCleanedContent
            ? (string.IsNullOrEmpty(SelectedFile.CleanedContent) ? SelectedFile.RawContent : SelectedFile.CleanedContent)
            : SelectedFile.RawContent;
    }

    /// <summary>Marks all currently filtered files as included in export.</summary>
    [RelayCommand]
    private void SelectAll() => SetAllSelection(true);

    /// <summary>Unchecks all currently filtered files from export inclusion.</summary>
    [RelayCommand]
    private void DeselectAll() => SetAllSelection(false);

    private void SetAllSelection(bool select)
    {
        foreach (var file in FilteredFiles)
        {
            file.IsIncluded = select;
        }
        var current = SelectedFile;
        SelectedFile = null;
        SelectedFile = current;
    }

    /// <summary>Copies the active preview text content to the clipboard.</summary>
    [RelayCommand]
    private void CopyPreviewContent()
    {
        if (!string.IsNullOrEmpty(PreviewContent))
        {
            Clipboard.SetText(PreviewContent);
        }
    }
}
