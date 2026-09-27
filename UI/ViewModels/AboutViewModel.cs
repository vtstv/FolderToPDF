// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Diagnostics;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FolderToPDF.UI.ViewModels;

/// <summary>
/// ViewModel managing application metadata, author attribution, and remote update checking.
/// </summary>
public partial class AboutViewModel : ObservableObject
{
    [ObservableProperty]
    private string _version = "2.0.0 (Modernized Edition)";

    [ObservableProperty]
    private string _author = "Murr";

    [ObservableProperty]
    private string _repositoryUrl = "https://github.com/vtstv/FolderToPDF";

    [ObservableProperty]
    private string _updateStatus = "Click below to check for updates.";

    [ObservableProperty]
    private bool _isCheckingUpdates = false;

    /// <summary>
    /// Launches the default web browser navigating to the GitHub repository.
    /// </summary>
    [RelayCommand]
    private void OpenGitHub()
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = RepositoryUrl, UseShellExecute = true });
        }
        catch
        {
            // Silently ignore browser launch failures
        }
    }

    /// <summary>
    /// Queries the update endpoint asynchronously to compare installed version against the latest release.
    /// </summary>
    [RelayCommand]
    private async Task CheckForUpdatesAsync()
    {
        IsCheckingUpdates = true;
        UpdateStatus = "Checking server for latest release...";

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var response = await client.GetStringAsync("https://pdf.murr.li/version.txt");
            var latestStr = response.Trim();

            if (System.Version.TryParse(latestStr, out var latest) &&
                System.Version.TryParse("2.0.0", out var current))
            {
                if (latest > current)
                {
                    UpdateStatus = $"New version {latest} available at {RepositoryUrl}";
                }
                else
                {
                    UpdateStatus = "You are on the latest version! (2.0.0)";
                }
            }
            else
            {
                UpdateStatus = "Version check completed: Current version is up to date.";
            }
        }
        catch
        {
            UpdateStatus = "Unable to connect to update server. Check your network connection.";
        }
        finally
        {
            IsCheckingUpdates = false;
        }
    }
}
