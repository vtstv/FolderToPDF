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
/// Manages persistent storage and retrieval of user preset configurations and built-in profiles.
/// Built-in profiles are always guaranteed to exist and cannot be deleted.
/// </summary>
public class ProfileService : IProfileService
{
    private readonly string _profilesFilePath;
    private readonly List<Profile> _profiles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileService"/> class.
    /// </summary>
    /// <param name="profilesFilePath">Optional custom file path for storing profiles JSON. Defaults to "profiles.json".</param>
    public ProfileService(string? profilesFilePath = null)
    {
        _profilesFilePath = profilesFilePath ?? "profiles.json";
        LoadProfiles();
    }

    /// <summary>
    /// Gets all registered profiles as a read-only list.
    /// </summary>
    public IReadOnlyList<Profile> GetProfiles() => _profiles.AsReadOnly();

    /// <summary>
    /// Retrieves a profile by its unique string identifier.
    /// </summary>
    /// <param name="id">The profile identifier.</param>
    /// <returns>The matching profile, or null if not found.</returns>
    public Profile? GetProfileById(string id) => _profiles.FirstOrDefault(p => p.Id == id);

    /// <summary>
    /// Retrieves the default profile, defaulting to the first built-in profile.
    /// </summary>
    /// <returns>The default <see cref="Profile"/> instance.</returns>
    public Profile GetDefaultProfile() => _profiles.FirstOrDefault(p => p.IsBuiltIn) ?? CreateBuiltInProfiles().First();

    /// <summary>
    /// Saves or updates a profile in the collection and persists changes to disk.
    /// </summary>
    /// <param name="profile">The profile to save.</param>
    public void SaveProfile(Profile profile)
    {
        var existingIndex = _profiles.FindIndex(p => p.Id == profile.Id);
        if (existingIndex >= 0)
        {
            _profiles[existingIndex] = profile;
        }
        else
        {
            _profiles.Add(profile);
        }

        PersistToFile();
    }

    /// <summary>
    /// Deletes a custom profile by ID. Built-in system profiles are protected from deletion.
    /// </summary>
    /// <param name="id">The identifier of the profile to remove.</param>
    /// <returns>True if the profile was successfully removed; otherwise false.</returns>
    public bool DeleteProfile(string id)
    {
        var profile = _profiles.FirstOrDefault(p => p.Id == id);
        if (profile != null && !profile.IsBuiltIn)
        {
            _profiles.Remove(profile);
            PersistToFile();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Clears all custom profiles and restores the built-in default presets.
    /// </summary>
    public void ResetToDefaults()
    {
        _profiles.Clear();
        _profiles.AddRange(CreateBuiltInProfiles());
        PersistToFile();
    }

    /// <summary>
    /// Reads persisted profiles from the JSON configuration file, falling back to built-ins if the file is missing or invalid.
    /// </summary>
    private void LoadProfiles()
    {
        try
        {
            if (File.Exists(_profilesFilePath))
            {
                var json = File.ReadAllText(_profilesFilePath);
                var loaded = JsonSerializer.Deserialize<List<Profile>>(json);
                if (loaded != null && loaded.Count > 0)
                {
                    _profiles.Clear();
                    _profiles.AddRange(loaded);

                    // Ensure built-ins are always present even if omitted in external JSON
                    foreach (var builtin in CreateBuiltInProfiles())
                    {
                        if (!_profiles.Any(p => p.Id == builtin.Id))
                        {
                            _profiles.Insert(0, builtin);
                        }
                    }
                    return;
                }
            }
        }
        catch
        {
            // Silently fall back to built-ins on corrupt configuration
        }

        _profiles.Clear();
        _profiles.AddRange(CreateBuiltInProfiles());
        PersistToFile();
    }

    /// <summary>
    /// Writes the in-memory profiles list to disk as formatted JSON.
    /// </summary>
    private void PersistToFile()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_profiles, options);
            var dir = Path.GetDirectoryName(_profilesFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(_profilesFilePath, json);
        }
        catch
        {
            // Ignore background file system write errors
        }
    }

    /// <summary>
    /// Factory creating default built-in profile presets for diverse technology stacks.
    /// </summary>
    private static List<Profile> CreateBuiltInProfiles()
    {
        return new List<Profile>
        {
            new()
            {
                Id = "builtin-codebase",
                Name = "Full Codebase (Standard)",
                Description = "General developer profile scanning all common language source files with standard exclusions.",
                IsBuiltIn = true,
                FileTypes = new() { "*.cs", "*.ts", "*.tsx", "*.js", "*.jsx", "*.py", "*.go", "*.rs", "*.java", "*.cpp", "*.h", "*.sql", "*.json", "*.yaml", "*.md" },
                ExcludeFolders = new() { "node_modules", "bin", "obj", ".git", ".vs", ".idea", "dist", "build", ".next", "__pycache__", ".venv" },
                ExcludeFiles = new() { "*.lock", "package-lock.json", "*.min.js", "*.min.css", "*.map", "*.svg", "*.png", "*.jpg", "*.ico" },
                IncludeFiles = new(),
                RemoveComments = false,
                RedactSecrets = true,
                IncludeFileTreeHeader = true
            },
            new()
            {
                Id = "builtin-frontend",
                Name = "Web & Frontend (React/Vue/Next)",
                Description = "Optimized for modern TypeScript, React, Vue, CSS, and web components.",
                IsBuiltIn = true,
                FileTypes = new() { "*.ts", "*.tsx", "*.js", "*.jsx", "*.vue", "*.css", "*.scss", "*.html", "*.json" },
                ExcludeFolders = new() { "node_modules", ".next", ".nuxt", "dist", "build", "coverage", ".git", ".turbo" },
                ExcludeFiles = new() { "*.lock", "package-lock.json", "pnpm-lock.yaml", "*.map", "*.min.js" },
                IncludeFiles = new(),
                RemoveComments = false,
                RedactSecrets = true,
                IncludeFileTreeHeader = true
            },
            new()
            {
                Id = "builtin-python",
                Name = "Python & AI Backend",
                Description = "Python services, FastAPI, Django, and machine learning pipelines.",
                IsBuiltIn = true,
                FileTypes = new() { "*.py", "*.sql", "*.yaml", "*.yml", "*.toml", "*.json", "*.md" },
                ExcludeFolders = new() { "__pycache__", ".venv", "venv", "env", ".pytest_cache", ".mypy_cache", ".git", ".tox" },
                ExcludeFiles = new() { "poetry.lock", "Pipfile.lock", "*.pyc" },
                IncludeFiles = new(),
                RemoveComments = false,
                RedactSecrets = true,
                IncludeFileTreeHeader = true
            },
            new()
            {
                Id = "builtin-dotnet",
                Name = ".NET & C# Solution",
                Description = "Optimized for C# solutions, ASP.NET Core, and WPF/MAUI architectures.",
                IsBuiltIn = true,
                FileTypes = new() { "*.cs", "*.xaml", "*.json", "*.xml", "*.csproj", "*.props", "*.targets" },
                ExcludeFolders = new() { "bin", "obj", ".vs", "TestResults", ".git", "packages" },
                ExcludeFiles = new() { "*.suo", "*.user", "*.cache" },
                IncludeFiles = new(),
                RemoveComments = false,
                RedactSecrets = true,
                IncludeFileTreeHeader = true
            }
        };
    }
}
