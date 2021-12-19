using System.Windows;
using System.Windows.Input;

namespace FileDetails.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The path of the file
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Creates a new instance of the <see cref="MainWindow"/>
        /// </summary>
        /// <param name="filePath">The path of the file</param>
        public MainWindow(string filePath)
        {
            InitializeComponent();

            _filePath = filePath;
        }

        /// <summary>
        /// Occurs when the file was loaded
        /// </summary>
        /// <param name="sender">The <see cref="MainWindow"/></param>
        /// <param name="e">The event arguments</param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.InitViewModel(_filePath);
        }

        /// <summary>
        /// Occurs when the users holds the left mouse button
        /// </summary>
        /// <param name="sender">The header grid</param>
        /// <param name="e">The mouse events</param>
        private void Header_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        /// <summary>
        /// Occurs when the user hits the close button
        /// </summary>
        /// <param name="sender">The close button</param>
        /// <param name="e">The event arguments</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
