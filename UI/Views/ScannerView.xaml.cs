// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.IO;
using System.Windows;
using System.Windows.Controls;
using FolderToPDF.UI.ViewModels;

namespace FolderToPDF.UI.Views;

/// <summary>
/// Interaction logic for ScannerView.xaml.
/// Handles UI-level drag-and-drop events for folders and routes path changes to the ViewModel.
/// </summary>
public partial class ScannerView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScannerView"/> class.
    /// </summary>
    public ScannerView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Checks for dropped files/folders on the drag area and displays copy cursor.
    /// </summary>
    private void Border_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }

    /// <summary>
    /// Accepts dropped folders and forwards the directory path to the ViewModel.
    /// </summary>
    private void Border_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                var path = files[0];
                if (Directory.Exists(path))
                {
                    if (DataContext is ScannerViewModel vm)
                    {
                        vm.RootDirectory = path;
                    }
                }
            }
        }
    }
}
