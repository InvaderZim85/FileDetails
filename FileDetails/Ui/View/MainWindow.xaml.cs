using FileDetails.Common;
using FileDetails.Ui.ViewModel;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace FileDetails.Ui.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    /// <summary>
    /// The path of the file
    /// </summary>
    private readonly string _path;

    /// <summary>
    /// Creates a new instance of the <see cref="MainWindow"/>
    /// </summary>
    /// <param name="path">The path of the file</param>
    public MainWindow(string path)
    {
        InitializeComponent();

        _path = path;
    }

    /// <summary>
    /// Occurs when the window was loaded
    /// </summary>
    /// <param name="sender">The <see cref="MainWindow"/></param>
    /// <param name="e">The event arguments</param>
    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        // Adds the double click event to all text boxes
        AddDoubleClickEvent();

        // This method is needed to use the logger and taskbar manager!
        Helper.InitHelper(false);

        if (DataContext is MainWindowViewModel viewModel)
            viewModel.LoadData(_path);
    }

    /// <summary>
    /// Adds the double click event to all text boxes
    /// </summary>
    private void AddDoubleClickEvent()
    {
        // Load all text box controls
        var controls = this.FindLogicalChildren<TextBox>();

        // Create the context menu
        var copyContextMenu = new ContextMenu();
        // Create & add the menu item
        var copyItem = new MenuItem
        {
            Header = "Copy to clipboard"
        };
        copyItem.Click += CopyToClipboard;
        copyContextMenu.Items.Add(copyItem);

        // Iterate through the controls and add the context menu & event
        foreach (var entry in controls)
        {
            entry.MouseDoubleClick += CopyToClipboard;

            entry.ContextMenu = copyContextMenu;
        }

        return;

        static void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            switch (sender)
            {
                case TextBox textBox:
                    Clipboard.SetText(textBox.Text);
                    break;
                case MenuItem { Parent: ContextMenu { Parent: TextBox textBoxContextMenu } }:
                    Clipboard.SetText(textBoxContextMenu.Text);
                    break;
            }
        }
    }
}