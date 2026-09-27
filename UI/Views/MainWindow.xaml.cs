// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.UI.ViewModels;
using Wpf.Ui.Controls;

namespace FolderToPDF.UI.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// Main application FluentWindow container with Mica backdrop support.
/// </summary>
public partial class MainWindow : FluentWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class with the root ViewModel.
    /// </summary>
    /// <param name="viewModel">The root MainViewModel data context.</param>
    public MainWindow(MainViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
