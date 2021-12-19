using System;
using System.Windows;
using FileDetails.Ui;

namespace FileDetails
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Occurs when the application was started
        /// </summary>
        /// <param name="sender">The <see cref="App"/></param>
        /// <param name="e">The startup arguments</param>
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                if (e.Args.Length <= 0)
                    return;

                var path = Helper.GetPath(e.Args);

                if (!Helper.CanStart(path))
                {
                    MessageBox.Show($"Can't read the given path: {path}", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Current.Shutdown();
                }

                var mainWindow = new MainWindow(path);
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error has occurred:\r\n{ex.Message}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
