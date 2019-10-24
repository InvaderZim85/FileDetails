using System;
using System.Windows;
using System.Windows.Controls;

namespace FileDetails
{
    /// <summary>
    /// Interaction logic for CompareWindow.xaml
    /// </summary>
    public partial class CompareWindow : Window
    {
        /// <summary>
        /// Contains the hash values
        /// </summary>
        private (string md5, string sha1, string sha256) _values;

        /// <summary>
        /// Creates a new instance of the <see cref="CompareWindow"/>
        /// </summary>
        /// <param name="values">The different hash values</param>
        public CompareWindow((string md5, string sha1, string sha256) values)
        {
            InitializeComponent();

            _values = values;
        }

        /// <summary>
        /// Occurs when the window was loaded
        /// </summary>
        private void CompareWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CompareWindowViewModel viewModel)
                viewModel.InitViewModel(_values);
        }

        /// <summary>
        /// Occurs when the user changes the text of the compare with box
        /// </summary>
        private void TextBoxCompareValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is CompareWindowViewModel viewModel)
                viewModel.CompareValues();
        }

        /// <summary>
        /// Occurs when the user hits the close button
        /// </summary>
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
