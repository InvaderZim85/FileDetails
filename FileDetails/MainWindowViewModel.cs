using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using WpfUtility.Services;

namespace FileDetails
{
    public class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Contains the file
        /// </summary>
        private FileInfo _file;
        /// <summary>
        /// Contains the directory
        /// </summary>
        private DirectoryInfo _directory;

        /// <summary>
        /// Backing field for <see cref="IsDirectory"/>
        /// </summary>
        private bool _isDirectory;
        /// <summary>
        /// Gets or sets the value which indicates if the current path is a directory or a file
        /// </summary>
        public bool IsDirectory
        {
            get => _isDirectory;
            set => SetField(ref _isDirectory, value);
        }

        /// <summary>
        /// Gets the is value which indicates if the current path is a file
        /// </summary>
        public bool IsFile => !IsDirectory;

        /// <summary>
        /// Gets the file name
        /// </summary>
        public string FileName => IsDirectory ? _directory?.Name ?? "" : _file?.Name ?? "";

        /// <summary>
        /// Gets the file path
        /// </summary>
        public string Path => IsDirectory ? _directory?.FullName ?? "" : _file?.FullName ?? "";

        /// <summary>
        /// Backing field for <see cref="Size"/>
        /// </summary>
        private string _size = "Loading...";
        /// <summary>
        /// Gets or sets the file(s) size
        /// </summary>
        public string Size
        {
            get => _size;
            set => SetField(ref _size, value);
        }

        /// <summary>
        /// Backing field for <see cref="FileCount"/>
        /// </summary>
        private string _fileCount = "Loading...";
        /// <summary>
        /// Gets or sets the file count
        /// </summary>
        public string FileCount
        {
            get => _fileCount;
            set => SetField(ref _fileCount, value);
        }

        /// <summary>
        /// Gets the creation date
        /// </summary>
        public string CreationDate => IsDirectory
            ? _directory?.CreationTime.ToString("G") ?? ""
            : _file?.CreationTime.ToString("G") ?? "";

        /// <summary>
        /// Gets the last write date
        /// </summary>
        public string WriteDate => IsDirectory
            ? _directory?.LastWriteTime.ToString("G") ?? ""
            : _file?.LastWriteTime.ToString("G") ?? "";

        /// <summary>
        /// Gets the last access date
        /// </summary>
        public string AccessDate => IsDirectory
            ? _directory?.LastAccessTime.ToString("G") ?? ""
            : _file?.LastAccessTime.ToString("G") ?? "";

        /// <summary>
        /// Gets the md5 hash of the file
        /// </summary>
        public string Md5Hash => _file == null ? "" : Helper.GetMd5Hash(_file);

        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
        public string Attributes => $"{(IsDirectory ? _directory?.Attributes : _file?.Attributes)}";


        /// <summary>
        /// Sets the file
        /// </summary>
        /// <param name="path">The path of the file</param>
        public async void SetFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            IsDirectory = Helper.IsDirectory(path);

            (string size, string count) result;
            if (IsDirectory)
            {
                _directory = new DirectoryInfo(path);
            }
            else
            {
                _file = new FileInfo(path);
            }

            var properties = Helper.GetProperties(this, Helper.PropertyAccessType.Read);
            foreach (var property in properties)
            {
                OnPropertyChanged(property);
            }


            if (IsDirectory)
            {
                result = await Task.Run(() => Helper.GetSizeFileCount(_directory));
            }
            else
            {
                result = await Task.Run(() => Helper.GetSizeFileCount(_file));
            }

            Size = result.size;
            FileCount = result.count;
        }

        /// <summary>
        /// The command to copy the data to the clipboard
        /// </summary>
        public ICommand CopyCommand => new DelegateCommand(CopyToClipboard);
        /// <summary>
        /// The command to save the data
        /// </summary>
        public ICommand SaveCommand => new DelegateCommand(SaveData);
        /// <summary>
        /// The command to close the window
        /// </summary>
        public ICommand CloseCommand => new DelegateCommand(() => Application.Current.Shutdown());

        /// <summary>
        /// Copies the data to the clipboard
        /// </summary>
        private void CopyToClipboard()
        {
            Clipboard.SetText(CreateText());
        }

        /// <summary>
        /// Saves the data into a file
        /// </summary>
        private void SaveData()
        {
            var dialog = new SaveFileDialog
            {
                FileName = $"{_file.Name} Details",
                Filter = "Text (*.txt)|*.txt|All (*.*)|*.*",
                DefaultExt = ".txt"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, CreateText());

                    MessageBox.Show("File saved successfully.", "Save", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An error has occured:\r\n{e.Message}", "Save - Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Creates the text for the file / clipboard
        /// </summary>
        /// <returns>The string with the data</returns>
        private string CreateText()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"FileName: {FileName}");
            sb.AppendLine($"Path: {Path}");
            sb.AppendLine($"Size: {Size}");
            if (IsDirectory)
                sb.AppendLine($"Files: {FileCount}");
            sb.AppendLine($"Creation date: {CreationDate}");
            sb.AppendLine($"Last write date: {WriteDate}");
            sb.AppendLine($"Last access date: {AccessDate}");
            if (IsFile)
                sb.AppendLine($"MD5 hash: {Md5Hash}");

            return sb.ToString();
        }
    }
}
