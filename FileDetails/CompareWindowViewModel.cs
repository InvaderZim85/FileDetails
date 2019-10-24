using System;
using System.Windows.Media;
using ZimLabs.WpfBase;

namespace FileDetails
{
    public class CompareWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Contains the md5 values
        /// </summary>
        private (string md5, string sha1, string sha256) _values = ("", "", "");

        /// <summary>
        /// Backing field for <see cref="Md5"/>
        /// </summary>
        private bool _md5 = true;

        /// <summary>
        /// Gets or sets the value which indicates if the SHA-256 type is selected
        /// </summary>
        public bool Md5
        {
            get => _md5;
            set
            {
                SetField(ref _md5, value);
                SetHashValue();
            }
        }

        /// <summary>
        /// Backing field for <see cref="Sha1"/>
        /// </summary>
        private bool _sha1;

        /// <summary>
        /// Gets or sets the value which indicates if the SHA-256 type is selected
        /// </summary>
        public bool Sha1
        {
            get => _sha1;
            set 
            { 
                SetField(ref _sha1, value);
                SetHashValue();
            }
        }

        /// <summary>
        /// Backing field for <see cref="Sha256"/>
        /// </summary>
        private bool _sha256;

        /// <summary>
        /// Gets or sets the value which indicates if the SHA-256 type is selected
        /// </summary>
        public bool Sha256
        {
            get => _sha256;
            set
            {
                SetField(ref _sha256, value);
                SetHashValue();
            }
        }

        /// <summary>
        /// Backing field for <see cref="HashValue"/>
        /// </summary>
        private string _hashValue;

        /// <summary>
        /// Gets or sets the hash value of the file
        /// </summary>
        public string HashValue
        {
            get => _hashValue;
            set => SetField(ref _hashValue, value);
        }

        /// <summary>
        /// Backing field for <see cref="CompareValue"/>
        /// </summary>
        private string _compareValue;

        /// <summary>
        /// Gets or sets the compare value
        /// </summary>
        public string CompareValue
        {
            get => _compareValue;
            set => SetField(ref _compareValue, value);
        }

        /// <summary>
        /// Backing field for <see cref="Info"/>
        /// </summary>
        private string _info;

        /// <summary>
        /// Gets or sets the info value
        /// </summary>
        public string Info
        {
            get => _info;
            set => SetField(ref _info, value);
        }

        /// <summary>
        /// Backing field for <see cref="InfoColor"/>
        /// </summary>
        private SolidColorBrush _infoColor = new SolidColorBrush(Colors.Green);

        /// <summary>
        /// Gets or sets the color of the info text
        /// </summary>
        public SolidColorBrush InfoColor
        {
            get => _infoColor;
            set => SetField(ref _infoColor, value);
        }

        /// <summary>
        /// Init the view model
        /// </summary>
        /// <param name="values">The values</param>
        public void InitViewModel((string md5, string sha1, string sha256) values)
        {
            _values = values;

            HashValue = values.md5;
        }

        /// <summary>
        /// Sets the has value
        /// </summary>
        private void SetHashValue()
        {
            if (Md5)
            {
                HashValue = _values.md5;
            }
            else if (Sha1)
            {
                HashValue = _values.sha1;
            }
            else if (Sha256)
            {
                HashValue = _values.sha256;
            }

            if (!string.IsNullOrEmpty(CompareValue))
                CompareValues();
        }

        /// <summary>
        /// Compares the values
        /// </summary>
        public void CompareValues()
        {
            // prepare the compare value
            var value = CompareValue.Replace("-", "").ToLower();

            if (string.IsNullOrEmpty(value))
            {
                Info = "";
                return;
            }


            if (HashValue.Equals(value))
            {
                Info = "Values equal";
                InfoColor = new SolidColorBrush(Colors.Green);
            }
            else
            {
                Info = "Values not equal";
                InfoColor = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
