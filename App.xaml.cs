// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Windows;
using FolderToPDF.Core.Enums;
using FolderToPDF.Core.Interfaces;
using FolderToPDF.Core.Services;
using FolderToPDF.UI.ViewModels;
using FolderToPDF.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Appearance;

namespace FolderToPDF;

/// <summary>
/// Application entry point and Dependency Injection composition root.
/// Registers domain services, ViewModels, and views, and initializes application theming.
/// </summary>
public partial class App : Application
{
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Gets the application-wide DI service provider container.
    /// </summary>
    public static IServiceProvider Services => _serviceProvider ?? throw new InvalidOperationException("Services not initialized.");

    /// <summary>
    /// Configures dependency injection, resolves initial theme preferences, and displays the main window.
    /// </summary>
    /// <param name="e">Event args containing startup parameters.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Register Core Domain Services
        services.AddSingleton<IContentCleanerService, ContentCleanerService>();
        services.AddSingleton<ITokenCounterService, TokenCounterService>();
        services.AddSingleton<IFileScannerService, FileScannerService>();
        services.AddSingleton<IPdfExportService, PdfExportService>();
        services.AddSingleton<ITextExportService, TextExportService>();
        services.AddSingleton<IMarkdownExportService, MarkdownExportService>();
        services.AddSingleton<IProfileService, ProfileService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IExportHistoryService, ExportHistoryService>();

        // Register Presentation ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<ScannerViewModel>();
        services.AddSingleton<FilePreviewViewModel>();
        services.AddSingleton<DocumentViewerViewModel>();
        services.AddSingleton<ExportHistoryViewModel>();
        services.AddSingleton<ProfilesViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<AboutViewModel>();

        // Register Presentation Views
        services.AddSingleton<MainWindow>();
        services.AddTransient<DocumentViewerView>();
        services.AddTransient<ExportHistoryView>();

        _serviceProvider = services.BuildServiceProvider();

        // Resolve and apply persisted visual theme
        var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
        var themeMode = settingsService.CurrentSettings.ThemeMode;
        var theme = themeMode switch
        {
            AppThemeMode.Dark => ApplicationTheme.Dark,
            AppThemeMode.Light => ApplicationTheme.Light,
            _ => ApplicationThemeManager.GetSystemTheme() == SystemTheme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light
        };
        ApplicationThemeManager.Apply(theme);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

        if (e.Args.Contains("--screenshot"))
        {
            var scannerVm = _serviceProvider.GetRequiredService<ScannerViewModel>();
            scannerVm.RootDirectory = @"C:\Dev\FolderToPDF";
            scannerVm.OutputFileName = "FolderToPDF_v2_Release";
            scannerVm.TotalFiles = 57;
            scannerVm.TotalLines = 8450;
            scannerVm.TotalTokens = 92400;
            scannerVm.TotalSizeText = "2.35 MB";
            scannerVm.ContextWindow128kPercent = 72.2;
            scannerVm.ContextWindow200kPercent = 46.2;
            scannerVm.StatusMessage = "Scan completed. 57 files ready for packaging.";

            mainWindow.Loaded += async (_, _) =>
            {
                await System.Threading.Tasks.Task.Delay(1200);
                mainWindow.UpdateLayout();

                int width = (int)Math.Max(1200, mainWindow.ActualWidth);
                int height = (int)Math.Max(850, mainWindow.ActualHeight);

                var dv = new System.Windows.Media.DrawingVisual();
                using (var dc = dv.RenderOpen())
                {
                    dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x20, 0x20, 0x20)), null, new Rect(0, 0, width, height));
                    dc.DrawRectangle(new System.Windows.Media.VisualBrush(mainWindow), null, new Rect(0, 0, width, height));
                }

                var rtb = new System.Windows.Media.Imaging.RenderTargetBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
                rtb.Render(dv);

                var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));

                var assetsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets");
                var rootAssetsDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\assets"));
                var targetDir = System.IO.Directory.Exists(rootAssetsDir) ? rootAssetsDir : assetsDir;
                if (!System.IO.Directory.Exists(targetDir)) System.IO.Directory.CreateDirectory(targetDir);

                var outPath = System.IO.Path.Combine(targetDir, "app-preview.png");
                using (var fs = System.IO.File.Create(outPath))
                {
                    encoder.Save(fs);
                }

                Shutdown(0);
            };
        }

        mainWindow.Show();
    }
}
