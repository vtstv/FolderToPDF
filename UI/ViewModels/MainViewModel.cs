// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderToPDF.Core.Enums;
using FolderToPDF.Core.Interfaces;
using Wpf.Ui.Appearance;

namespace FolderToPDF.UI.ViewModels;

/// <summary>
/// Root ViewModel orchestrating main window navigation, theme toggling, and inter-ViewModel communication.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _title = "FolderToPDF - Codebase Context & Document Packager";

    [ObservableProperty]
    private int _currentNavigationIndex = 0;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private bool _isDarkMode;

    /// <summary>Gets the scanner workflow ViewModel.</summary>
    public ScannerViewModel ScannerVm { get; }

    /// <summary>Gets the file preview and selection ViewModel.</summary>
    public FilePreviewViewModel FilePreviewVm { get; }

    /// <summary>Gets the embedded PDF and document viewer ViewModel.</summary>
    public DocumentViewerViewModel DocumentViewerVm { get; }

    /// <summary>Gets the generated projects and export history ViewModel.</summary>
    public ExportHistoryViewModel ExportHistoryVm { get; }

    /// <summary>Gets the preset profiles management ViewModel.</summary>
    public ProfilesViewModel ProfilesVm { get; }

    /// <summary>Gets the persistent settings configuration ViewModel.</summary>
    public SettingsViewModel SettingsVm { get; }

    /// <summary>Gets the application information and update ViewModel.</summary>
    public AboutViewModel AboutVm { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class with dependency-injected child ViewModels.
    /// </summary>
    public MainViewModel(
        ISettingsService settingsService,
        ScannerViewModel scannerVm,
        FilePreviewViewModel filePreviewVm,
        DocumentViewerViewModel documentViewerVm,
        ExportHistoryViewModel exportHistoryVm,
        ProfilesViewModel profilesVm,
        SettingsViewModel settingsVm,
        AboutViewModel aboutVm)
    {
        _settingsService = settingsService;
        ScannerVm = scannerVm;
        FilePreviewVm = filePreviewVm;
        DocumentViewerVm = documentViewerVm;
        ExportHistoryVm = exportHistoryVm;
        ProfilesVm = profilesVm;
        SettingsVm = settingsVm;
        AboutVm = aboutVm;

        _isDarkMode = _settingsService.CurrentSettings.ThemeMode == AppThemeMode.Dark;

        // Wire scanner results to file preview automatically
        ScannerVm.ScanCompleted += (s, files) =>
        {
            FilePreviewVm.SetFiles(files, ScannerVm.RootDirectory);
        };

        // Wire in-app document viewer navigation requests
        ScannerVm.OpenDocumentRequested += (s, path) => OpenDocumentInViewer(path);
        ExportHistoryVm.OpenDocumentRequested += (s, path) => OpenDocumentInViewer(path);
    }

    /// <summary>Loads an exported document into the embedded viewer and switches to the Viewer tab.</summary>
    public void OpenDocumentInViewer(string filePath)
    {
        DocumentViewerVm.LoadDocument(filePath);
        CurrentNavigationIndex = 2; // Navigate to Document Viewer tab
    }

    /// <summary>Toggles application visual theme between Light and Dark mode.</summary>
    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        var theme = IsDarkMode ? ApplicationTheme.Dark : ApplicationTheme.Light;
        ApplicationThemeManager.Apply(theme);

        _settingsService.CurrentSettings.ThemeMode = IsDarkMode ? AppThemeMode.Dark : AppThemeMode.Light;
        _settingsService.SaveSettings();
    }

    /// <summary>Switches active navigation tab index.</summary>
    /// <param name="indexStr">Target tab index as string or boxed int.</param>
    [RelayCommand]
    private void Navigate(string? indexStr)
    {
        if (int.TryParse(indexStr, out int index))
        {
            CurrentNavigationIndex = index;
        }
    }
}
