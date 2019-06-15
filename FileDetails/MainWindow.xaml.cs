using System.Windows;

namespace FileDetails
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Contains the filepath
        /// </summary>
        private readonly string _filepath;

        /// <summary>
        /// Creates a new instance of the main window
        /// </summary>
        /// <param name="filepath">The path of the file</param>
        public MainWindow(string filepath)
        {
            InitializeComponent();
            _filepath = filepath;
        }

        /// <summary>
        /// Occurs when the form is loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel dataContext)
                dataContext.SetFile(_filepath);
        }
    }
}
