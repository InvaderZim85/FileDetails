using FileDetails.Ui.View;
using System.Linq;
using System.Windows;

namespace FileDetails;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Occurs when the application starts
    /// </summary>
    /// <param name="sender">The <see cref="App"/></param>
    /// <param name="e">The event arguments (contains the arguments)</param>
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        // Get the path
        var path = e.Args.Any()
            ? e.Args.Length == 1
                ? e.Args.First()
                : string.Join(" ", e.Args)
            : string.Empty;

        // Create the main window and show it
        var mainWindow = new MainWindow(path);
        mainWindow.Show();
    }
}