using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ZimLabs.WpfBase;

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
        /// The timer for the info message
        /// </summary>
        private readonly Timer _infoTimer = new Timer(5000);

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
        /// Backing field for <see cref="Md5"/>
        /// </summary>
        private string _md5;

        /// <summary>
        /// Gets or sets the MD5 hash value
        /// </summary>
        public string Md5
        {
            get => _md5;
            set => SetField(ref _md5, value);
        }

        /// <summary>
        /// Backing field for <see cref="Sha1"/>
        /// </summary>
        private string _sha1;

        /// <summary>
        /// Gets or sets the SHA1 hash value
        /// </summary>
        public string Sha1
        {
            get => _sha1;
            set => SetField(ref _sha1, value);
        }

        /// <summary>
        /// Backing field for <see cref="Sha256"/>
        /// </summary>
        private string _sha256;

        /// <summary>
        /// Gets or sets the SHA-256 hash value
        /// </summary>
        public string Sha256
        {
            get => _sha256;
            set => SetField(ref _sha256, value);
        }

        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
        public string Attributes => $"{(IsDirectory ? _directory?.Attributes : _file?.Attributes)}";

        /// <summary>
        /// Backing field for <see cref="Info"/>
        /// </summary>
        private string _info;

        /// <summary>
        /// Gets or sets the info field
        /// </summary>
        public string Info
        {
            get => _info;
            set => SetField(ref _info, value);
        }


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

                var (md5, sha1, sha256) = Helper.GetHash(_file);
                Md5 = md5;
                Sha1 = sha1;
                Sha256 = sha256;
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
        public ICommand CopyCommand => new RelayCommand<SaveType>(CopyToClipboard);
        
        /// <summary>
        /// The command to save the data
        /// </summary>
        public ICommand SaveCommand => new RelayCommand<SaveType>(SaveData);

        /// <summary>
        /// The command to close the window
        /// </summary>
        public ICommand CloseCommand => new DelegateCommand(() => Application.Current.Shutdown());

        /// <summary>
        /// Copies the data to the clipboard
        /// </summary>
        /// <param name="type">The type of the file</param>
        private void CopyToClipboard(SaveType type)
        {
            Clipboard.SetText(CreateText(type));
            ShowInfo();
        }

        /// <summary>
        /// Saves the data into a file
        /// </summary>
        /// <param name="type">The type of the file</param>
        private void SaveData(SaveType type)
        {
            var dialog = new SaveFileDialog
            {
                FileName = $"{FileName.Replace(" ", "_")}-Details",
                Filter = type == SaveType.Markdown ? "Markdown (*.md)|*.md|All (*.*)|*.*" : "Text (*.txt)|*.txt|All (*.*)|*.*",
                DefaultExt = type == SaveType.Markdown ? ".md" : ".txt"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, CreateText(type));

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
        /// <param name="type">The type of the file</param>
        /// <returns>The string with the data</returns>
        private string CreateText(SaveType type)
        {
            var sb = new StringBuilder();

            if (type == SaveType.Text)
            {
                sb.AppendLine($"FileName: {FileName}");
                sb.AppendLine($"Path: {Path}");
                sb.AppendLine($"Size: {Size}");
                if (IsDirectory)
                    sb.AppendLine($"Files: {FileCount}");
                sb.AppendLine($"Creation date: {CreationDate}");
                sb.AppendLine($"Last write date: {WriteDate}");
                sb.AppendLine($"Last access date: {AccessDate}");
                if (IsFile)
                {
                    sb.AppendLine($"MD5 hash: {Md5}");
                    sb.AppendLine($"SHA1 hash: {Sha1}");
                    sb.AppendLine($"SHA-256 hash: {Sha256}");
                }
            }
            else
            {
                sb.AppendLine($"# {FileName} details\r\n");
                sb.AppendLine("## Details\r\n");
                sb.AppendLine("| Type | Value |");
                sb.AppendLine("|---|---|");
                sb.AppendLine($"| Filename | {FileName} |");
                sb.AppendLine($"| Path | `{Path}` |");
                sb.AppendLine($"| Size | {Size} |");
                if (IsDirectory)
                    sb.AppendLine($"| Files | {FileCount} |");
                sb.AppendLine($"| Creation date | {CreationDate} |");
                sb.AppendLine($"| Last write date | {WriteDate} |");
                sb.AppendLine($" |Last access date | {AccessDate} |");
                if (IsFile)
                {
                    sb.AppendLine($"| MD5 hash | {Md5} |");
                    sb.AppendLine($"| SHA1 hash | {Sha1} |");
                    sb.AppendLine($"| SHA-256 hash | {Sha256} |");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Shows the info
        /// </summary>
        private void ShowInfo()
        {
            if (_infoTimer.Enabled)
                return;

            _infoTimer.Start();
            Info = "Infos copied...";

            _infoTimer.Elapsed += (o, s) =>
            {
                _infoTimer.Stop();
                Info = "";
            };
        }
    }
}
