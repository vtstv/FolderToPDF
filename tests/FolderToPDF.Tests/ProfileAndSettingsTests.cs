// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Models;
using FolderToPDF.Core.Services;
using Xunit;

namespace FolderToPDF.Tests;

public class ProfileAndSettingsTests : IDisposable
{
    private readonly string _profilesFile;
    private readonly string _settingsFile;

    public ProfileAndSettingsTests()
    {
        _profilesFile = Path.Combine(Path.GetTempPath(), "test_prof_" + Guid.NewGuid().ToString("N") + ".json");
        _settingsFile = Path.Combine(Path.GetTempPath(), "test_sett_" + Guid.NewGuid().ToString("N") + ".json");
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_profilesFile)) File.Delete(_profilesFile);
            if (File.Exists(_settingsFile)) File.Delete(_settingsFile);
        }
        catch { }
    }

    [Fact]
    public void ProfileService_ProvidesBuiltInPresets()
    {
        var service = new ProfileService(_profilesFile);
        var profiles = service.GetProfiles();

        Assert.NotEmpty(profiles);
        Assert.Contains(profiles, p => p.Id == "builtin-codebase");
        Assert.Contains(profiles, p => p.Id == "builtin-frontend");
        Assert.Contains(profiles, p => p.Id == "builtin-python");
        Assert.Contains(profiles, p => p.Id == "builtin-dotnet");
    }

    [Fact]
    public void ProfileService_CanSaveAndRetrieveCustomProfile()
    {
        var service = new ProfileService(_profilesFile);
        var custom = new Profile
        {
            Id = "custom-test-profile",
            Name = "Custom Test Profile",
            Description = "For testing purposes",
            IsBuiltIn = false,
            FileTypes = new() { "*.rs", "*.toml" }
        };

        service.SaveProfile(custom);

        var retrieved = service.GetProfileById("custom-test-profile");
        Assert.NotNull(retrieved);
        Assert.Equal("Custom Test Profile", retrieved.Name);
        Assert.Contains("*.rs", retrieved.FileTypes);

        // Delete custom profile
        var deleted = service.DeleteProfile("custom-test-profile");
        Assert.True(deleted);
        Assert.Null(service.GetProfileById("custom-test-profile"));
    }

    [Fact]
    public void ProfileService_CannotDeleteBuiltInProfiles()
    {
        var service = new ProfileService(_profilesFile);
        var deleted = service.DeleteProfile("builtin-codebase");
        Assert.False(deleted);
        Assert.NotNull(service.GetProfileById("builtin-codebase"));
    }

    [Fact]
    public void SettingsService_CanSaveAndReload()
    {
        var service = new SettingsService(_settingsFile);
        service.CurrentSettings.TitleFont = "Arial Custom";
        service.CurrentSettings.TitleFontSize = 14;
        service.SaveSettings();

        var reloadedService = new SettingsService(_settingsFile);
        Assert.Equal("Arial Custom", reloadedService.CurrentSettings.TitleFont);
        Assert.Equal(14, reloadedService.CurrentSettings.TitleFontSize);
    }

    [Fact]
    public void SettingsService_DefaultOutputFolderAndFlags_Persist()
    {
        var service = new SettingsService(_settingsFile);
        service.CurrentSettings.DefaultOutputDirectory = @"C:\CustomExports";
        service.CurrentSettings.AutoOpenOutputFolder = true;
        service.CurrentSettings.AutoOpenExportedFile = true;
        service.CurrentSettings.NormalizeEmptyLines = true;
        service.SaveSettings();

        var reloaded = new SettingsService(_settingsFile);
        Assert.Equal(@"C:\CustomExports", reloaded.CurrentSettings.DefaultOutputDirectory);
        Assert.True(reloaded.CurrentSettings.AutoOpenOutputFolder);
        Assert.True(reloaded.CurrentSettings.AutoOpenExportedFile);
        Assert.True(reloaded.CurrentSettings.NormalizeEmptyLines);
    }

    [Fact]
    public async Task ExportHistoryService_AddRetrieveAndRemove_WorksCorrectly()
    {
        var historyFile = Path.Combine(Path.GetTempPath(), "test_hist_" + Guid.NewGuid().ToString("N") + ".json");
        try
        {
            var historyService = new ExportHistoryService(historyFile);
            var item = new ExportHistoryItem
            {
                Id = "rec-1",
                ProjectName = "SampleProject",
                OutputFilePath = @"C:\Fake\test.pdf",
                Format = "PDF",
                TokenCount = 1500,
                FileCount = 10,
                FileSizeBytes = 45000
            };

            await historyService.AddItemAsync(item);
            var list = historyService.GetHistory();
            Assert.Single(list);
            Assert.Equal("SampleProject", list[0].ProjectName);
            Assert.Equal(1500, list[0].TokenCount);

            await historyService.RemoveItemAsync("rec-1");
            Assert.Empty(historyService.GetHistory());
        }
        finally
        {
            if (File.Exists(historyFile)) File.Delete(historyFile);
        }
    }
}
