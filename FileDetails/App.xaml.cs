using System;
using System.IO;
using System.Windows;

namespace FileDetails
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Occurs when the application is started
        /// </summary>
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                if (e.Args.Length <= 0)
                    return;

                var path = GetPath(e.Args);

                if (!CanStart(path))
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
                MessageBox.Show($"An error has occured:\r\n{ex.Message}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Gets the path
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The path</returns>
        private string GetPath(string[] args)
        {
            var result = "";

            foreach (var entry in args)
            {
                result += $"{entry} ";
            }

            return result.Trim();
        }

        /// <summary>
        /// Checks if the given path is valid
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>true when the path is valid, otherwise false</returns>
        private bool CanStart(string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
                return true;

            return false;
        }
    }
}
