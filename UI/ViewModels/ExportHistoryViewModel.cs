// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Models;

namespace FolderToPDF.UI.ViewModels;

/// <summary>
/// ViewModel managing the export history screen, search filtering, document launching, and history cleanup.
/// </summary>
public partial class ExportHistoryViewModel : ObservableObject
{
    private readonly IExportHistoryService _historyService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private ExportHistoryItem? _selectedItem;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private int _totalExports;

    [ObservableProperty]
    private int _totalExportedTokens;

    [ObservableProperty]
    private string _formattedTotalSize = "0 B";

    /// <summary>Fires when an export item is requested to be opened in the embedded document viewer.</summary>
    public event EventHandler<string>? OpenDocumentRequested;

    /// <summary>Gets the observable collection of history records displayed to the user.</summary>
    public ObservableCollection<ExportHistoryItem> FilteredItems { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportHistoryViewModel"/> class.
    /// </summary>
    /// <param name="historyService">Service handling export history persistence.</param>
    public ExportHistoryViewModel(IExportHistoryService historyService)
    {
        _historyService = historyService;
        _historyService.HistoryChanged += (s, e) => LoadItems();
        LoadItems();
    }

    partial void OnSearchQueryChanged(string value)
    {
        ApplyFilter();
    }

    /// <summary>Reloads history items from persistent storage and updates aggregate statistics.</summary>
    [RelayCommand]
    public void LoadItems()
    {
        var all = _historyService.GetHistory();
        TotalExports = all.Count;
        TotalExportedTokens = all.Sum(i => i.TokenCount);

        long totalBytes = all.Sum(i => i.FileSizeBytes);
        FormattedTotalSize = FormatBytes(totalBytes);

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var all = _historyService.GetHistory();
        FilteredItems.Clear();

        var query = SearchQuery.Trim();
        var matching = string.IsNullOrEmpty(query)
            ? all
            : all.Where(i => i.ProjectName.Contains(query, StringComparison.OrdinalIgnoreCase)
                          || i.Format.Contains(query, StringComparison.OrdinalIgnoreCase)
                          || i.OutputFilePath.Contains(query, StringComparison.OrdinalIgnoreCase)
                          || i.RootDirectory.Contains(query, StringComparison.OrdinalIgnoreCase));

        foreach (var item in matching)
        {
            FilteredItems.Add(item);
        }

        StatusMessage = $"{FilteredItems.Count} export(s) found.";
    }

    /// <summary>Opens the selected export document inside the application's embedded document viewer.</summary>
    [RelayCommand]
    private void PreviewItem(ExportHistoryItem? item)
    {
        item ??= SelectedItem;
        if (item == null) return;

        if (File.Exists(item.OutputFilePath))
        {
            OpenDocumentRequested?.Invoke(this, item.OutputFilePath);
        }
        else
        {
            StatusMessage = "File no longer exists at target path.";
        }
    }

    /// <summary>Opens the selected export file in the operating system's default viewer.</summary>
    [RelayCommand]
    private void OpenFile(ExportHistoryItem? item)
    {
        item ??= SelectedItem;
        if (item == null) return;

        if (File.Exists(item.OutputFilePath))
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = item.OutputFilePath, UseShellExecute = true });
                StatusMessage = $"Opened: {Path.GetFileName(item.OutputFilePath)}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to open file: {ex.Message}";
            }
        }
        else
        {
            StatusMessage = "File no longer exists at target path.";
        }
    }

    /// <summary>Opens Windows File Explorer with the selected export file highlighted.</summary>
    [RelayCommand]
    private void OpenFolder(ExportHistoryItem? item)
    {
        item ??= SelectedItem;
        if (item == null) return;

        if (File.Exists(item.OutputFilePath))
        {
            Process.Start("explorer.exe", $"/select,\"{item.OutputFilePath}\"");
            StatusMessage = "Explorer opened.";
        }
        else
        {
            var folder = Path.GetDirectoryName(item.OutputFilePath);
            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            {
                Process.Start(new ProcessStartInfo { FileName = folder, UseShellExecute = true });
            }
            else
            {
                StatusMessage = "Directory no longer exists.";
            }
        }
    }

    /// <summary>Copies the full absolute path of the export document to the system clipboard.</summary>
    [RelayCommand]
    private void CopyPath(ExportHistoryItem? item)
    {
        item ??= SelectedItem;
        if (item == null) return;

        try
        {
            Clipboard.SetText(item.OutputFilePath);
            StatusMessage = "File path copied to clipboard!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to copy path: {ex.Message}";
        }
    }

    /// <summary>Deletes the selected history record from storage.</summary>
    [RelayCommand]
    private async Task DeleteItemAsync(ExportHistoryItem? item)
    {
        item ??= SelectedItem;
        if (item == null) return;

        await _historyService.RemoveItemAsync(item.Id);
        StatusMessage = "Record removed from history.";
    }

    /// <summary>Purges all export history records.</summary>
    [RelayCommand]
    private async Task ClearAllAsync()
    {
        if (FilteredItems.Count == 0) return;

        var result = MessageBox.Show(
            "Are you sure you want to clear all export history records? (Generated files on disk will not be deleted)",
            "Clear Export History",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            await _historyService.ClearHistoryAsync();
            StatusMessage = "Export history cleared.";
        }
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{(double)bytes / 1024:0.0} KB";
        return $"{(double)bytes / (1024 * 1024):0.00} MB";
    }
}
