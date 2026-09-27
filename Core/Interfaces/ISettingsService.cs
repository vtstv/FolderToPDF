// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Interfaces;

/// <summary>
/// Provides persistent storage, loading, and real-time access to application settings across user sessions.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Gets the current in-memory application settings instance.
    /// </summary>
    AppSettings CurrentSettings { get; }

    /// <summary>
    /// Serializes and commits the current application settings to local disk storage (JSON file).
    /// </summary>
    void SaveSettings();

    /// <summary>
    /// Reloads settings from disk storage, resetting unsaved modifications or restoring missing defaults.
    /// </summary>
    void ReloadSettings();
}
