// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.IO;
using System.Text.Json;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Services;

/// <summary>
/// Provides persistent local JSON storage, retrieval, and management of exported codebase project archives.
/// </summary>
public class ExportHistoryService : IExportHistoryService
{
    private readonly string _historyFilePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private List<ExportHistoryItem> _items = new();

    /// <summary>Occurs when export history items are added, removed, or cleared.</summary>
    public event EventHandler? HistoryChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportHistoryService"/> class.
    /// </summary>
    /// <param name="historyFilePath">Optional custom path to the history file. Defaults to "exports_history.json".</param>
    public ExportHistoryService(string? historyFilePath = null)
    {
        _historyFilePath = historyFilePath ?? "exports_history.json";
        LoadHistorySync();
    }

    /// <summary>
    /// Retrieves all recorded export history entries in reverse chronological order.
    /// </summary>
    /// <returns>A read-only collection of <see cref="ExportHistoryItem"/> records.</returns>
    public IReadOnlyList<ExportHistoryItem> GetHistory()
    {
        lock (_items)
        {
            return _items.OrderByDescending(i => i.CreatedAt).ToList();
        }
    }

    /// <summary>
    /// Appends a new export artifact record to persistent history storage.
    /// </summary>
    /// <param name="item">The export metadata item to store.</param>
    public async Task AddItemAsync(ExportHistoryItem item)
    {
        await _semaphore.WaitAsync();
        try
        {
            lock (_items)
            {
                // Remove existing entry if path matches, then insert at top
                _items.RemoveAll(i => string.Equals(i.OutputFilePath, item.OutputFilePath, StringComparison.OrdinalIgnoreCase));
                _items.Insert(0, item);
            }

            await SaveHistoryInternalAsync();
        }
        finally
        {
            _semaphore.Release();
        }

        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Removes a specific export history item by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the record to delete.</param>
    public async Task RemoveItemAsync(string id)
    {
        await _semaphore.WaitAsync();
        try
        {
            lock (_items)
            {
                _items.RemoveAll(i => i.Id == id);
            }

            await SaveHistoryInternalAsync();
        }
        finally
        {
            _semaphore.Release();
        }

        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Deletes all recorded history entries.
    /// </summary>
    public async Task ClearHistoryAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            lock (_items)
            {
                _items.Clear();
            }

            await SaveHistoryInternalAsync();
        }
        finally
        {
            _semaphore.Release();
        }

        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Reloads history entries from disk and refreshes local file existence states.
    /// </summary>
    public async Task RefreshAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            LoadHistorySync();
        }
        finally
        {
            _semaphore.Release();
        }

        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    private void LoadHistorySync()
    {
        try
        {
            if (File.Exists(_historyFilePath))
            {
                var json = File.ReadAllText(_historyFilePath);
                var loaded = JsonSerializer.Deserialize<List<ExportHistoryItem>>(json);
                if (loaded != null)
                {
                    lock (_items)
                    {
                        _items = loaded;
                    }
                    return;
                }
            }
        }
        catch
        {
            // Ignore parse errors on corrupt file
        }

        lock (_items)
        {
            _items = new List<ExportHistoryItem>();
        }
    }

    private async Task SaveHistoryInternalAsync()
    {
        try
        {
            string json;
            lock (_items)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                json = JsonSerializer.Serialize(_items, options);
            }

            var dir = Path.GetDirectoryName(_historyFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.WriteAllTextAsync(_historyFilePath, json);
        }
        catch
        {
            // Silently ignore disk write failures
        }
    }
}
