// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.IO;
using System.Windows.Controls;
using FolderToPDF.UI.ViewModels;

namespace FolderToPDF.UI.Views;

/// <summary>
/// Interaction logic for DocumentViewerView.xaml.
/// Hosts an embedded WebView2 PDF viewer and monospace code text previewer.
/// </summary>
public partial class DocumentViewerView : UserControl
{
    private bool _isWebViewInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentViewerView"/> class.
    /// </summary>
    public DocumentViewerView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is DocumentViewerViewModel oldVm)
        {
            oldVm.DocumentLoaded -= OnDocumentLoaded;
        }

        if (e.NewValue is DocumentViewerViewModel newVm)
        {
            newVm.DocumentLoaded += OnDocumentLoaded;
            if (newVm.HasDocument && !string.IsNullOrEmpty(newVm.CurrentFilePath))
            {
                OnDocumentLoaded(this, newVm.CurrentFilePath);
            }
        }
    }

    private async void OnDocumentLoaded(object? sender, string filePath)
    {
        if (DataContext is not DocumentViewerViewModel vm)
            return;

        if (vm.IsPdf && File.Exists(filePath))
        {
            try
            {
                if (!_isWebViewInitialized)
                {
                    await PdfWebView.EnsureCoreWebView2Async();
                    _isWebViewInitialized = true;
                }

                PdfWebView.CoreWebView2?.Navigate(new Uri(filePath).AbsoluteUri);
            }
            catch
            {
                // Gracefully ignore WebView2 environment errors (e.g. headless unit testing environments)
            }
        }
    }
}
