// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Interfaces;

/// <summary>
/// Service contract for persisting, tracking, and retrieving generated codebase export history records.
/// </summary>
public interface IExportHistoryService
{
    /// <summary>Occurs when export history items are added, removed, or cleared.</summary>
    event EventHandler? HistoryChanged;

    /// <summary>
    /// Retrieves all recorded export history entries in reverse chronological order.
    /// </summary>
    /// <returns>A read-only collection of <see cref="ExportHistoryItem"/> records.</returns>
    IReadOnlyList<ExportHistoryItem> GetHistory();

    /// <summary>
    /// Appends a new export artifact record to persistent history storage.
    /// </summary>
    /// <param name="item">The export metadata item to store.</param>
    Task AddItemAsync(ExportHistoryItem item);

    /// <summary>
    /// Removes a specific export history item by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the record to delete.</param>
    Task RemoveItemAsync(string id);

    /// <summary>
    /// Deletes all recorded history entries.
    /// </summary>
    Task ClearHistoryAsync();

    /// <summary>
    /// Reloads history entries from disk and refreshes local file existence states.
    /// </summary>
    Task RefreshAsync();
}
