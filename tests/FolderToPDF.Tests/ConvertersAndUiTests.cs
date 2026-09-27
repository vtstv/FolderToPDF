// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Globalization;
using FolderToPDF.Core.Enums;
using FolderToPDF.Core.Models;
using FolderToPDF.Core.Services;
using FolderToPDF.UI.Converters;
using FolderToPDF.UI.ViewModels;
using Xunit;

namespace FolderToPDF.Tests;

public class ConvertersAndUiTests : IDisposable
{
    private readonly string _testSettingsFile;
    private readonly string _testProfilesFile;

    public ConvertersAndUiTests()
    {
        _testSettingsFile = Path.Combine(Path.GetTempPath(), "test_settings_" + Guid.NewGuid().ToString("N") + ".json");
        _testProfilesFile = Path.Combine(Path.GetTempPath(), "test_profiles_" + Guid.NewGuid().ToString("N") + ".json");
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_testSettingsFile)) File.Delete(_testSettingsFile);
            if (File.Exists(_testProfilesFile)) File.Delete(_testProfilesFile);
        }
        catch { }
    }

    [Fact]
    public void ControlAppearance_ValuesCheck()
    {
        var names = Enum.GetNames(typeof(Wpf.Ui.Controls.ControlAppearance));
        Assert.NotEmpty(names);
        Assert.Contains("Transparent", names);
    }

    [Fact]
    public void MainWindow_InstantiatesWithoutXamlErrors_InStaThread()
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                App app;
                if (System.Windows.Application.Current is App existing)
                {
                    app = existing;
                }
                else
                {
                    app = new App();
                }
                app.InitializeComponent();

                var cleaner = new ContentCleanerService();
                var tokens = new TokenCounterService();
                var scanner = new FileScannerService(cleaner, tokens);
                var pdf = new PdfExportService();
                var txt = new TextExportService();
                var md = new MarkdownExportService();
                var profiles = new ProfileService(_testProfilesFile);
                var settings = new SettingsService(_testSettingsFile);
                var exportHistory = new ExportHistoryService("test_exports.json");

                var scannerVm = new ScannerViewModel(scanner, pdf, txt, md, profiles, settings, exportHistory);
                var previewVm = new FilePreviewViewModel();
                var docViewerVm = new DocumentViewerViewModel();
                var exportHistoryVm = new ExportHistoryViewModel(exportHistory);
                var profilesVm = new ProfilesViewModel(profiles, settings, scannerVm);
                var settingsVm = new SettingsViewModel(settings);
                var aboutVm = new AboutViewModel();
                var mainVm = new MainViewModel(settings, scannerVm, previewVm, docViewerVm, exportHistoryVm, profilesVm, settingsVm, aboutVm);

                var window = new FolderToPDF.UI.Views.MainWindow(mainVm);
                Assert.NotNull(window);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception != null)
        {
            throw new InvalidOperationException($"MainWindow initialization failed: {exception.Message}", exception);
        }
    }

    [Fact]
    public void InverseBooleanConverter_InvertsBooleanCorrectly()
    {
        var converter = new InverseBooleanConverter();

        Assert.Equal(false, converter.Convert(true, typeof(bool), null, CultureInfo.InvariantCulture));
        Assert.Equal(true, converter.Convert(false, typeof(bool), null, CultureInfo.InvariantCulture));
        Assert.Equal(false, converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture));

        Assert.Equal(false, converter.ConvertBack(true, typeof(bool), null, CultureInfo.InvariantCulture));
        Assert.Equal(true, converter.ConvertBack(false, typeof(bool), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityToBooleanConverter_MatchesEqualityAndConvertsBack()
    {
        var converter = new EqualityToBooleanConverter();

        Assert.Equal(true, converter.Convert(0, typeof(bool), "0", CultureInfo.InvariantCulture));
        Assert.Equal(false, converter.Convert(1, typeof(bool), "0", CultureInfo.InvariantCulture));
        Assert.Equal(true, converter.Convert("test", typeof(bool), "TEST", CultureInfo.InvariantCulture));

        var backResult = converter.ConvertBack(true, typeof(int), "2", CultureInfo.InvariantCulture);
        Assert.Equal(2, backResult);
    }

    [Fact]
    public void FileSizeConverter_FormatsBytesCorrectly()
    {
        var converter = new FileSizeConverter();

        Assert.Equal("500 B", converter.Convert(500L, typeof(string), null, CultureInfo.InvariantCulture));
        Assert.Equal("1.5 KB", converter.Convert(1536L, typeof(string), null, CultureInfo.InvariantCulture));
        Assert.Equal("2.00 MB", converter.Convert(2 * 1024 * 1024L, typeof(string), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void NumberFormatConverter_FormatsNumbersWithSeparators()
    {
        var converter = new NumberFormatConverter();

        Assert.Equal("1,234,567", converter.Convert(1234567, typeof(string), null, CultureInfo.InvariantCulture));
        Assert.Equal("9,876,543", converter.Convert(9876543L, typeof(string), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ScannerViewModel_CanStartScan_IsTrueWhenIdleAndFalseWhenBusy()
    {
        var cleaner = new ContentCleanerService();
        var tokens = new TokenCounterService();
        var scanner = new FileScannerService(cleaner, tokens);
        var pdf = new PdfExportService();
        var txt = new TextExportService();
        var md = new MarkdownExportService();
        var profiles = new ProfileService(_testProfilesFile);
        var settings = new SettingsService(_testSettingsFile);
        var exportHistory = new ExportHistoryService("test_exports2.json");

        var vm = new ScannerViewModel(scanner, pdf, txt, md, profiles, settings, exportHistory);

        Assert.False(vm.IsBusy);
        Assert.True(vm.StartScanCommand.CanExecute(null));

        vm.IsBusy = true;
        Assert.False(vm.StartScanCommand.CanExecute(null));
    }

    [Fact]
    public void SettingsViewModel_SaveAndResetDefaults_UpdatesSettingsState()
    {
        var settingsService = new SettingsService(_testSettingsFile);
        var vm = new SettingsViewModel(settingsService)
        {
            TitleFont = "CustomTitleFont",
            TitleFontSize = 18,
            ContentFont = "CustomCodeFont",
            ContentFontSize = 12,
            PdfIncludeLineNumbers = false,
            PdfIncludeTableOfContents = false,
            PreferredTokenizer = TokenizerModel.Claude,
            ThemeMode = AppThemeMode.Light
        };

        vm.SaveCommand.Execute(null);

        Assert.Equal("CustomTitleFont", settingsService.CurrentSettings.TitleFont);
        Assert.Equal(18, settingsService.CurrentSettings.TitleFontSize);
        Assert.False(settingsService.CurrentSettings.PdfIncludeLineNumbers);
        Assert.Equal(TokenizerModel.Claude, settingsService.CurrentSettings.PreferredTokenizer);

        vm.ResetDefaultsCommand.Execute(null);

        Assert.Equal("Segoe UI", vm.TitleFont);
        Assert.Equal(11, vm.TitleFontSize);
        Assert.True(vm.PdfIncludeLineNumbers);
        Assert.Equal(TokenizerModel.O200kBase, vm.PreferredTokenizer);
    }

    [Fact]
    public void DocumentViewerViewModel_LoadDocument_DetectsPdfAndTextProperly()
    {
        var tempFile = Path.GetTempFileName();
        var tempPdf = Path.ChangeExtension(tempFile, ".pdf");
        var tempTxt = Path.ChangeExtension(tempFile, ".txt");

        try
        {
            File.WriteAllText(tempPdf, "%PDF-1.4 dummy content");
            File.WriteAllText(tempTxt, "Hello from plain text archive");

            var vm = new DocumentViewerViewModel();
            Assert.False(vm.HasDocument);

            // Test PDF loading
            vm.LoadDocument(tempPdf);
            Assert.True(vm.HasDocument);
            Assert.True(vm.IsPdf);
            Assert.False(vm.IsText);
            Assert.Equal("PDF Document", vm.DocumentFormat);
            Assert.Equal(Path.GetFileName(tempPdf), vm.DocumentTitle);

            // Test Text loading
            vm.LoadDocument(tempTxt);
            Assert.True(vm.HasDocument);
            Assert.False(vm.IsPdf);
            Assert.True(vm.IsText);
            Assert.Equal("Plain Text Archive", vm.DocumentFormat);
            Assert.Equal("Hello from plain text archive", vm.TextContent);

            // Test Close
            vm.CloseDocumentCommand.Execute(null);
            Assert.False(vm.HasDocument);
            Assert.Null(vm.CurrentFilePath);
        }
        finally
        {
            if (File.Exists(tempPdf)) File.Delete(tempPdf);
            if (File.Exists(tempTxt)) File.Delete(tempTxt);
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void ExportHistoryViewModel_PreviewItem_FiresOpenDocumentRequested()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var historyService = new ExportHistoryService("test_history_preview.json");
            var vm = new ExportHistoryViewModel(historyService);

            string? requestedPath = null;
            vm.OpenDocumentRequested += (s, path) => requestedPath = path;

            var item = new ExportHistoryItem
            {
                Id = "item1",
                ProjectName = "TestProject",
                OutputFilePath = tempFile,
                Format = "PDF"
            };

            vm.PreviewItemCommand.Execute(item);
            Assert.Equal(tempFile, requestedPath);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void MainViewModel_OpenDocumentInViewer_NavigatesToViewerTab()
    {
        var tempPdf = Path.ChangeExtension(Path.GetTempFileName(), ".pdf");
        try
        {
            File.WriteAllText(tempPdf, "%PDF-1.4 dummy");

            var cleaner = new ContentCleanerService();
            var tokens = new TokenCounterService();
            var scanner = new FileScannerService(cleaner, tokens);
            var pdf = new PdfExportService();
            var txt = new TextExportService();
            var md = new MarkdownExportService();
            var profiles = new ProfileService(_testProfilesFile);
            var settings = new SettingsService(_testSettingsFile);
            var history = new ExportHistoryService("test_history_nav.json");

            var scannerVm = new ScannerViewModel(scanner, pdf, txt, md, profiles, settings, history);
            var previewVm = new FilePreviewViewModel();
            var docViewerVm = new DocumentViewerViewModel();
            var historyVm = new ExportHistoryViewModel(history);
            var profilesVm = new ProfilesViewModel(profiles, settings, scannerVm);
            var settingsVm = new SettingsViewModel(settings);
            var aboutVm = new AboutViewModel();

            var mainVm = new MainViewModel(settings, scannerVm, previewVm, docViewerVm, historyVm, profilesVm, settingsVm, aboutVm);

            Assert.Equal(0, mainVm.CurrentNavigationIndex);

            // Trigger preview via MainViewModel helper
            mainVm.OpenDocumentInViewer(tempPdf);

            Assert.Equal(2, mainVm.CurrentNavigationIndex); // Result Viewer tab is index 2
            Assert.True(docViewerVm.HasDocument);
            Assert.Equal(tempPdf, docViewerVm.CurrentFilePath);
        }
        finally
        {
            if (File.Exists(tempPdf)) File.Delete(tempPdf);
        }
    }
}
