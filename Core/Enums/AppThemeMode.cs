// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

namespace FolderToPDF.Core.Enums;

/// <summary>
/// Specifies the visual theme appearance preferences for the application UI.
/// </summary>
public enum AppThemeMode
{
    /// <summary>
    /// Synchronizes the theme with the current Windows OS light or dark setting.
    /// </summary>
    System,

    /// <summary>
    /// Forces Fluent light theme appearance.
    /// </summary>
    Light,

    /// <summary>
    /// Forces Fluent dark theme appearance with Mica backdrop.
    /// </summary>
    Dark
}
