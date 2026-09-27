// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderToPDF.Core.Enums;
using FolderToPDF.Core.Interfaces;
using Wpf.Ui.Appearance;

namespace FolderToPDF.UI.ViewModels;

/// <summary>
/// ViewModel managing the application settings screen, font configurations, redaction rules, and theme toggling.
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _titleFont;

    [ObservableProperty]
    private int _titleFontSize;

    [ObservableProperty]
    private string _contentFont;

    [ObservableProperty]
    private int _contentFontSize;

    [ObservableProperty]
    private bool _pdfIncludeLineNumbers;

    [ObservableProperty]
    private bool _pdfIncludeTableOfContents;

    [ObservableProperty]
    private int _truncatedContentLength;

    [ObservableProperty]
    private TokenizerModel _preferredTokenizer;

    [ObservableProperty]
    private AppThemeMode _themeMode;

    [ObservableProperty]
    private bool _redactApiKeys;

    [ObservableProperty]
    private bool _redactPasswords;

    [ObservableProperty]
    private bool _redactEmails;

    [ObservableProperty]
    private bool _redactIps;

    [ObservableProperty]
    private bool _redactPrivateKeys;

    [ObservableProperty]
    private bool _redactJwtTokens;

    [ObservableProperty]
    private string _defaultOutputDirectory;

    [ObservableProperty]
    private bool _autoOpenOutputFolder;

    [ObservableProperty]
    private bool _autoOpenExportedFile;

    [ObservableProperty]
    private bool _normalizeEmptyLines;

    [ObservableProperty]
    private string _statusMessage = "Settings loaded.";

    /// <summary>Gets the list of selectable theme modes.</summary>
    public IReadOnlyList<AppThemeMode> AvailableThemeModes { get; } = Enum.GetValues<AppThemeMode>();

    /// <summary>Gets the list of selectable tokenizer algorithms.</summary>
    public IReadOnlyList<TokenizerModel> AvailableTokenizers { get; } = Enum.GetValues<TokenizerModel>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="settingsService">Settings service for persisting configuration.</param>
    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        var s = _settingsService.CurrentSettings;

        _defaultOutputDirectory = s.DefaultOutputDirectory;
        _autoOpenOutputFolder = s.AutoOpenOutputFolder;
        _autoOpenExportedFile = s.AutoOpenExportedFile;
        _normalizeEmptyLines = s.NormalizeEmptyLines;
        _titleFont = s.TitleFont;
        _titleFontSize = s.TitleFontSize;
        _contentFont = s.ContentFont;
        _contentFontSize = s.ContentFontSize;
        _pdfIncludeLineNumbers = s.PdfIncludeLineNumbers;
        _pdfIncludeTableOfContents = s.PdfIncludeTableOfContents;
        _truncatedContentLength = s.TruncatedContentLength;
        _preferredTokenizer = s.PreferredTokenizer;
        _themeMode = s.ThemeMode;
        _redactApiKeys = s.RedactApiKeys;
        _redactPasswords = s.RedactPasswords;
        _redactEmails = s.RedactEmails;
        _redactIps = s.RedactIps;
        _redactPrivateKeys = s.RedactPrivateKeys;
        _redactJwtTokens = s.RedactJwtTokens;
    }

    /// <summary>Opens a folder browser dialog to pick a custom default export destination directory.</summary>
    [RelayCommand]
    private void BrowseOutputDirectory()
    {
        var initialDir = Directory.Exists(DefaultOutputDirectory)
            ? DefaultOutputDirectory
            : AppDomain.CurrentDomain.BaseDirectory;

        var dialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "Select Default Export Directory",
            InitialDirectory = initialDir
        };

        if (dialog.ShowDialog() == true)
        {
            DefaultOutputDirectory = dialog.FolderName;
            Save();
        }
    }

    /// <summary>Opens the currently configured output folder in Windows File Explorer.</summary>
    [RelayCommand]
    private void OpenCurrentOutputFolder()
    {
        var target = Path.IsPathRooted(DefaultOutputDirectory)
            ? DefaultOutputDirectory
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultOutputDirectory);

        if (!Directory.Exists(target))
        {
            Directory.CreateDirectory(target);
        }

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = target,
            UseShellExecute = true
        });
    }

    /// <summary>Commits current values to the settings service and applies theme changes immediately.</summary>
    [RelayCommand]
    private void Save()
    {
        var s = _settingsService.CurrentSettings;
        s.DefaultOutputDirectory = DefaultOutputDirectory;
        s.AutoOpenOutputFolder = AutoOpenOutputFolder;
        s.AutoOpenExportedFile = AutoOpenExportedFile;
        s.NormalizeEmptyLines = NormalizeEmptyLines;
        s.TitleFont = TitleFont;
        s.TitleFontSize = TitleFontSize;
        s.ContentFont = ContentFont;
        s.ContentFontSize = ContentFontSize;
        s.PdfIncludeLineNumbers = PdfIncludeLineNumbers;
        s.PdfIncludeTableOfContents = PdfIncludeTableOfContents;
        s.TruncatedContentLength = TruncatedContentLength;
        s.PreferredTokenizer = PreferredTokenizer;
        s.ThemeMode = ThemeMode;
        s.RedactApiKeys = RedactApiKeys;
        s.RedactPasswords = RedactPasswords;
        s.RedactEmails = RedactEmails;
        s.RedactIps = RedactIps;
        s.RedactPrivateKeys = RedactPrivateKeys;
        s.RedactJwtTokens = RedactJwtTokens;

        _settingsService.SaveSettings();

        var appTheme = ThemeMode switch
        {
            AppThemeMode.Dark => ApplicationTheme.Dark,
            AppThemeMode.Light => ApplicationTheme.Light,
            _ => ApplicationThemeManager.GetSystemTheme() == SystemTheme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light
        };
        ApplicationThemeManager.Apply(appTheme);

        StatusMessage = "Settings saved successfully!";
    }

    /// <summary>Restores default settings preferences.</summary>
    [RelayCommand]
    private void ResetDefaults()
    {
        DefaultOutputDirectory = "output";
        AutoOpenOutputFolder = true;
        AutoOpenExportedFile = false;
        NormalizeEmptyLines = false;
        TitleFont = "Segoe UI";
        TitleFontSize = 11;
        ContentFont = "Consolas";
        ContentFontSize = 8;
        PdfIncludeLineNumbers = true;
        PdfIncludeTableOfContents = true;
        TruncatedContentLength = 250_000;
        PreferredTokenizer = TokenizerModel.O200kBase;
        ThemeMode = AppThemeMode.System;
        RedactApiKeys = true;
        RedactPasswords = true;
        RedactEmails = true;
        RedactIps = true;
        RedactPrivateKeys = true;
        RedactJwtTokens = true;

        Save();
        StatusMessage = "Default settings restored.";
    }
}
