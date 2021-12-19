using System.Windows;
using System.Windows.Controls;

namespace FileDetails.Ui
{
    /// <summary>
    /// Interaction logic for SeparatorControl.xaml
    /// </summary>
    public partial class SeparatorControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SeparatorControl"/>
        /// </summary>
        public SeparatorControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The dependency property of <see cref="SeparatorText"/>
        /// </summary>
        public static readonly DependencyProperty SeparatorTextProperty = DependencyProperty.Register(
            nameof(SeparatorText), typeof(string), typeof(SeparatorControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// Gets or sets the separator text
        /// </summary>
        public string SeparatorText
        {
            get => (string)GetValue(SeparatorTextProperty);
            set => SetValue(SeparatorTextProperty, value);
        }
    }
}
