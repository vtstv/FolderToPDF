// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Models;
using Microsoft.Win32;

namespace FolderToPDF.UI.ViewModels;

/// <summary>
/// Primary ViewModel for directory configuration, scanning execution, profile selection, and AI token analytics.
/// </summary>
public partial class ScannerViewModel : ObservableObject
{
    private readonly IFileScannerService _scannerService;
    private readonly IPdfExportService _pdfExportService;
    private readonly ITextExportService _textExportService;
    private readonly IMarkdownExportService _markdownExportService;
    private readonly IProfileService _profileService;
    private readonly ISettingsService _settingsService;
    private readonly IExportHistoryService _exportHistoryService;
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>Fires when a directory scan completes successfully with the list of scanned files.</summary>
    public event EventHandler<List<ScannedFile>>? ScanCompleted;

    /// <summary>Fires when a document export is requested to be opened in the embedded viewer.</summary>
    public event EventHandler<string>? OpenDocumentRequested;

    [ObservableProperty]
    private string _rootDirectory = string.Empty;

    [ObservableProperty]
    private string _fileTypesText = "*.cs, *.ts, *.tsx, *.js, *.py, *.json, *.md";

    [ObservableProperty]
    private string _excludeFoldersText = "node_modules, bin, obj, .git, .vs, dist, build, .next, __pycache__, .venv";

    [ObservableProperty]
    private string _excludeFilesText = "*.lock, package-lock.json, *.min.js, *.min.css, *.map, *.svg, *.png, *.jpg";

    [ObservableProperty]
    private string _includeFilesText = string.Empty;

    [ObservableProperty]
    private bool _removeComments = false;

    [ObservableProperty]
    private bool _redactSecrets = true;

    [ObservableProperty]
    private bool _includeFileTreeHeader = true;

    [ObservableProperty]
    private string _outputFileName = "codebase_context";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartScanCommand))]
    private bool _isBusy = false;

    [ObservableProperty]
    private double _progressPercentage = 0;

    [ObservableProperty]
    private string _statusMessage = "Ready. Select or drag a folder to scan.";

    [ObservableProperty]
    private int _totalFiles = 0;

    [ObservableProperty]
    private int _totalLines = 0;

    [ObservableProperty]
    private int _totalTokens = 0;

    [ObservableProperty]
    private string _totalSizeText = "0 KB";

    [ObservableProperty]
    private double _contextWindow128kPercent = 0;

    [ObservableProperty]
    private double _contextWindow200kPercent = 0;

    [ObservableProperty]
    private double _contextWindow1MPercent = 0;

    [ObservableProperty]
    private bool _hasResults = false;

    [ObservableProperty]
    private string _lastExportedFilePath = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Profile> _availableProfiles = new();

    [ObservableProperty]
    private Profile? _selectedProfile;

    private List<ScannedFile> _scannedFiles = new();
    private string _directoryTree = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScannerViewModel"/> class with required domain services.
    /// </summary>
    public ScannerViewModel(
        IFileScannerService scannerService,
        IPdfExportService pdfExportService,
        ITextExportService textExportService,
        IMarkdownExportService markdownExportService,
        IProfileService profileService,
        ISettingsService settingsService,
        IExportHistoryService exportHistoryService)
    {
        _scannerService = scannerService;
        _pdfExportService = pdfExportService;
        _textExportService = textExportService;
        _markdownExportService = markdownExportService;
        _profileService = profileService;
        _settingsService = settingsService;
        _exportHistoryService = exportHistoryService;

        LoadProfiles();

        if (!string.IsNullOrEmpty(_settingsService.CurrentSettings.LastDirectoryPath) &&
            Directory.Exists(_settingsService.CurrentSettings.LastDirectoryPath))
        {
            RootDirectory = _settingsService.CurrentSettings.LastDirectoryPath;
            UpdateDefaultOutputName();
        }
    }

    /// <summary>Populates available presets from profile service and selects the active profile.</summary>
    public void LoadProfiles()
    {
        AvailableProfiles.Clear();
        foreach (var p in _profileService.GetProfiles())
        {
            AvailableProfiles.Add(p);
        }

        var defaultProfile = AvailableProfiles.FirstOrDefault(p => p.Id == _settingsService.CurrentSettings.ActiveProfileId) 
                             ?? AvailableProfiles.FirstOrDefault();
        if (defaultProfile != null)
        {
            SelectedProfile = defaultProfile;
        }
    }

    partial void OnSelectedProfileChanged(Profile? value)
    {
        if (value == null) return;

        FileTypesText = string.Join(", ", value.FileTypes);
        ExcludeFoldersText = string.Join(", ", value.ExcludeFolders);
        ExcludeFilesText = string.Join(", ", value.ExcludeFiles);
        IncludeFilesText = string.Join(", ", value.IncludeFiles);
        RemoveComments = value.RemoveComments;
        RedactSecrets = value.RedactSecrets;
        IncludeFileTreeHeader = value.IncludeFileTreeHeader;

        _settingsService.CurrentSettings.ActiveProfileId = value.Id;
        _settingsService.SaveSettings();
    }

    partial void OnRootDirectoryChanged(string value)
    {
        UpdateDefaultOutputName();
        _settingsService.CurrentSettings.LastDirectoryPath = value;
        _settingsService.SaveSettings();
    }

    /// <summary>Generates a default archive output filename based on the selected directory's name.</summary>
    private void UpdateDefaultOutputName()
    {
        if (string.IsNullOrWhiteSpace(RootDirectory))
        {
            OutputFileName = "codebase_context";
            return;
        }

        try
        {
            var trimmed = RootDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var dirName = Path.GetFileName(trimmed);
            OutputFileName = string.IsNullOrWhiteSpace(dirName) ? "codebase_context" : $"{dirName}_context";
        }
        catch
        {
            OutputFileName = "codebase_context";
        }
    }

    /// <summary>Opens a folder picker dialog to select the codebase root directory.</summary>
    [RelayCommand]
    private void BrowseDirectory()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Folder to Package",
            InitialDirectory = Directory.Exists(RootDirectory) ? RootDirectory : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        };

        if (dialog.ShowDialog() == true)
        {
            RootDirectory = dialog.FolderName;
        }
    }

    private bool CanStartScan => !IsBusy;

    /// <summary>Executes background directory traversal, secret scrubbing, and token analysis.</summary>
    [RelayCommand(CanExecute = nameof(CanStartScan))]
    private async Task StartScanAsync()
    {
        if (string.IsNullOrWhiteSpace(RootDirectory) || !Directory.Exists(RootDirectory))
        {
            StatusMessage = "Please select a valid folder first.";
            return;
        }

        IsBusy = true;
        ProgressPercentage = 0;
        StatusMessage = "Starting directory scan...";
        _cancellationTokenSource = new CancellationTokenSource();

        var options = new ScanOptions
        {
            RootDirectory = RootDirectory,
            FileTypes = ParseTokens(FileTypesText),
            ExcludeFolders = ParseTokens(ExcludeFoldersText),
            ExcludeFiles = ParseTokens(ExcludeFilesText),
            IncludeFiles = ParseTokens(IncludeFilesText),
            RemoveComments = RemoveComments,
            RedactSecrets = RedactSecrets,
            IncludeFileTreeHeader = IncludeFileTreeHeader,
            Tokenizer = _settingsService.CurrentSettings.PreferredTokenizer,
            MaxContentLengthPerFile = _settingsService.CurrentSettings.TruncatedContentLength
        };

        var progress = new Progress<ScanProgressReport>(report =>
        {
            ProgressPercentage = report.ProgressPercentage;
            StatusMessage = report.StatusMessage;
        });

        try
        {
            var result = await _scannerService.ScanDirectoryAsync(options, progress, _cancellationTokenSource.Token);

            _scannedFiles = result.Files;
            _directoryTree = _scannerService.GenerateDirectoryTree(RootDirectory, _scannedFiles.Select(f => f.RelativePath));

            UpdateStatistics();
            HasResults = _scannedFiles.Count > 0;
            StatusMessage = $"Scan completed: {_scannedFiles.Count} files found in {result.Duration.TotalSeconds:F1}s";

            ScanCompleted?.Invoke(this, _scannedFiles);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Scan cancelled by user.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Cancels an ongoing scan operation.</summary>
    [RelayCommand]
    private void CancelScan()
    {
        _cancellationTokenSource?.Cancel();
    }

    /// <summary>Resets filter form fields and cleared statistics to default values.</summary>
    [RelayCommand]
    private void ClearForm()
    {
        RootDirectory = string.Empty;
        FileTypesText = "*.*";
        ExcludeFoldersText = "node_modules, bin, obj, .git, .vs";
        ExcludeFilesText = "*.lock";
        IncludeFilesText = string.Empty;
        RemoveComments = false;
        RedactSecrets = false;
        _scannedFiles.Clear();
        UpdateStatistics();
        HasResults = false;
        StatusMessage = "Filters reset to defaults.";
    }

    /// <summary>Calculates total file count, code lines, token metrics, and context window percentages.</summary>
    private void UpdateStatistics()
    {
        var included = _scannedFiles.Where(f => f.IsIncluded).ToList();
        TotalFiles = included.Count;
        TotalLines = included.Sum(f => f.LineCount);
        TotalTokens = included.Sum(f => f.TokenCount);

        long bytes = included.Sum(f => f.SizeInBytes);
        TotalSizeText = bytes < 1024 * 1024 
            ? $"{bytes / 1024.0:F1} KB" 
            : $"{bytes / (1024.0 * 1024.0):F2} MB";

        ContextWindow128kPercent = Math.Min(100.0, Math.Round((double)TotalTokens / 128_000 * 100.0, 1));
        ContextWindow200kPercent = Math.Min(100.0, Math.Round((double)TotalTokens / 200_000 * 100.0, 1));
        ContextWindow1MPercent = Math.Min(100.0, Math.Round((double)TotalTokens / 1_000_000 * 100.0, 2));
    }

    private static List<string> ParseTokens(string input)
    {
        return input.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
    }
}
