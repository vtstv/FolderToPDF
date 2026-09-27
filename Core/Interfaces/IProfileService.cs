// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;

namespace FolderToPDF.Core.Interfaces;

/// <summary>
/// Manages user presets and built-in profiles for scanning exclusions, file types, and redaction flags.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Retrieves all available profiles, including default system presets and custom user profiles.
    /// </summary>
    /// <returns>A read-only list of available profiles.</returns>
    IReadOnlyList<Profile> GetProfiles();

    /// <summary>
    /// Finds a specific profile by its unique identifier.
    /// </summary>
    /// <param name="id">The unique profile identifier.</param>
    /// <returns>The profile if found; otherwise, null.</returns>
    Profile? GetProfileById(string id);

    /// <summary>
    /// Returns the system default scan profile configured for standard multi-language software projects.
    /// </summary>
    /// <returns>The default fallback profile instance.</returns>
    Profile GetDefaultProfile();

    /// <summary>
    /// Saves or updates a profile in the persistent store. Built-in profiles cannot be overwritten.
    /// </summary>
    /// <param name="profile">The profile instance to persist.</param>
    void SaveProfile(Profile profile);

    /// <summary>
    /// Deletes a user profile with the given ID. Built-in profiles cannot be removed.
    /// </summary>
    /// <param name="id">The unique identifier of the profile to remove.</param>
    /// <returns>True if the profile was deleted; false if not found or protected.</returns>
    bool DeleteProfile(string id);

    /// <summary>
    /// Resets all profiles back to factory built-in presets, removing all user custom profiles.
    /// </summary>
    void ResetToDefaults();
}
