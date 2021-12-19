using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using FileDetails.DataObjects;
using FileDetails.Ui.Helper;

namespace FileDetails.Ui
{
    /// <summary>
    /// Provides the logic of the <see cref="MainWindow"/>
    /// </summary>
    internal class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Backing field for <see cref="File"/>
        /// </summary>
        private FileEntry? _file;

        /// <summary>
        /// Gets or sets the file
        /// </summary>
        public FileEntry? File
        {
            get => _file;
            private set => SetField(ref _file, value);
        }

        /// <summary>
        /// The command to copy the content to the clip board
        /// </summary>
        public ICommand CopyCommand => new DelegateCommand(CopyToClipboard);

        /// <summary>
        /// Init the view model
        /// </summary>
        /// <param name="filePath">The path of the file</param>
        public void InitViewModel(string filePath)
        {
            File = new FileEntry(new FileInfo(filePath));
        }

        /// <summary>
        /// Copies the information to the clipboard
        /// </summary>
        private void CopyToClipboard()
        {
            if (_file == null)
                return;

            FileDetails.Helper.CopyToClipboard(_file);
        }
    }
}
