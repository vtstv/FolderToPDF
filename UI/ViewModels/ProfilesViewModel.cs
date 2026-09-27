// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Models;

namespace FolderToPDF.UI.ViewModels;

/// <summary>
/// ViewModel managing the Profiles tab: creating, editing, applying, and deleting scanning preset configurations.
/// </summary>
public partial class ProfilesViewModel : ObservableObject
{
    private readonly IProfileService _profileService;
    private readonly ISettingsService _settingsService;
    private readonly ScannerViewModel _scannerVm;

    [ObservableProperty]
    private ObservableCollection<Profile> _profiles = new();

    [ObservableProperty]
    private Profile? _selectedProfile;

    [ObservableProperty]
    private string _newProfileName = string.Empty;

    [ObservableProperty]
    private string _newProfileDescription = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Manage and switch presets.";

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfilesViewModel"/> class.
    /// </summary>
    public ProfilesViewModel(
        IProfileService profileService,
        ISettingsService settingsService,
        ScannerViewModel scannerVm)
    {
        _profileService = profileService;
        _settingsService = settingsService;
        _scannerVm = scannerVm;

        RefreshProfiles();
    }

    /// <summary>Reloads profiles from the service into the observable collection.</summary>
    public void RefreshProfiles()
    {
        Profiles.Clear();
        foreach (var p in _profileService.GetProfiles())
        {
            Profiles.Add(p);
        }

        SelectedProfile = Profiles.FirstOrDefault(p => p.Id == _settingsService.CurrentSettings.ActiveProfileId) 
                          ?? Profiles.FirstOrDefault();
    }

    /// <summary>Applies the selected profile's settings directly to the scanner ViewModel.</summary>
    [RelayCommand]
    private void ApplyProfile(Profile? profile)
    {
        var target = profile ?? SelectedProfile;
        if (target == null) return;

        _scannerVm.SelectedProfile = target;
        StatusMessage = $"Profile '{target.Name}' applied to packager!";
    }

    /// <summary>Saves current scanner filter state as a new user profile.</summary>
    [RelayCommand]
    private void SaveCurrentAsNewProfile()
    {
        if (string.IsNullOrWhiteSpace(NewProfileName))
        {
            StatusMessage = "Please enter a profile name.";
            return;
        }

        var newProfile = new Profile
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = NewProfileName.Trim(),
            Description = string.IsNullOrWhiteSpace(NewProfileDescription) ? "User custom preset" : NewProfileDescription.Trim(),
            IsBuiltIn = false,
            DirectoryPath = _scannerVm.RootDirectory,
            FileTypes = ParseTokens(_scannerVm.FileTypesText),
            ExcludeFolders = ParseTokens(_scannerVm.ExcludeFoldersText),
            ExcludeFiles = ParseTokens(_scannerVm.ExcludeFilesText),
            IncludeFiles = ParseTokens(_scannerVm.IncludeFilesText),
            RemoveComments = _scannerVm.RemoveComments,
            RedactSecrets = _scannerVm.RedactSecrets,
            IncludeFileTreeHeader = _scannerVm.IncludeFileTreeHeader
        };

        _profileService.SaveProfile(newProfile);
        RefreshProfiles();
        SelectedProfile = Profiles.FirstOrDefault(p => p.Id == newProfile.Id);
        _scannerVm.LoadProfiles();

        NewProfileName = string.Empty;
        NewProfileDescription = string.Empty;
        StatusMessage = $"Profile '{newProfile.Name}' saved successfully!";
    }

    /// <summary>Deletes a custom user profile.</summary>
    [RelayCommand]
    private void DeleteProfile(Profile? profile)
    {
        var target = profile ?? SelectedProfile;
        if (target == null) return;

        if (target.IsBuiltIn)
        {
            StatusMessage = "Built-in profiles cannot be deleted.";
            return;
        }

        if (_profileService.DeleteProfile(target.Id))
        {
            RefreshProfiles();
            _scannerVm.LoadProfiles();
            StatusMessage = $"Profile '{target.Name}' removed.";
        }
    }

    /// <summary>Restores default built-in profile presets.</summary>
    [RelayCommand]
    private void ResetToDefaults()
    {
        _profileService.ResetToDefaults();
        RefreshProfiles();
        _scannerVm.LoadProfiles();
        StatusMessage = "Profiles reset to default built-ins.";
    }

    private static List<string> ParseTokens(string input)
    {
        return input.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
    }
}
