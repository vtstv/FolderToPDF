// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.IO;
using System.Text.Json;
using FolderToPDF.Core.Enums;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Models;
using Microsoft.Win32;

namespace FolderToPDF.Core.Services;

/// <summary>
/// Provides persistent storage and retrieval of application-wide user configuration using local JSON storage.
/// Automatically detects Windows system light/dark theme preference upon initial launch.
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly string _settingsFilePath;
    private AppSettings _settings;

    /// <summary>
    /// Gets the current in-memory application settings instance.
    /// </summary>
    public AppSettings CurrentSettings => _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    /// <param name="settingsFilePath">Optional custom path to settings JSON file. Defaults to "settings.json".</param>
    public SettingsService(string? settingsFilePath = null)
    {
        _settingsFilePath = settingsFilePath ?? "settings.json";
        _settings = LoadSettings();
    }

    /// <summary>
    /// Serializes and writes the current application settings to disk as formatted JSON.
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_settings, options);
            var dir = Path.GetDirectoryName(_settingsFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(_settingsFilePath, json);
        }
        catch
        {
            // Silently ignore settings file write errors (e.g. read-only disk environments)
        }
    }

    /// <summary>
    /// Reloads the application settings from disk storage.
    /// </summary>
    public void ReloadSettings()
    {
        _settings = LoadSettings();
    }

    /// <summary>
    /// Loads settings from the JSON configuration file, falling back to default values with auto-detected OS theme.
    /// </summary>
    private AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                if (loaded != null)
                {
                    return loaded;
                }
            }
        }
        catch
        {
            // Fall back to default on corrupt or unreadable settings file
        }

        return new AppSettings
        {
            ThemeMode = DetectSystemDarkMode() ? AppThemeMode.Dark : AppThemeMode.Light
        };
    }

    /// <summary>
    /// Queries the Windows registry to inspect whether dark mode is active for applications.
    /// </summary>
    /// <returns>True if Windows is configured for dark app theme; otherwise false.</returns>
    private static bool DetectSystemDarkMode()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key?.GetValue("AppsUseLightTheme") is int lightThemeValue)
            {
                return lightThemeValue == 0;
            }
        }
        catch
        {
            // Fallback default for developer convenience
        }
        return true;
    }
}
