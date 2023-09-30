using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileDetails.Models;
using ZimLabs.TableCreator;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace FileDetails.Ui.ViewModel;

/// <summary>
/// Provides the logic for the <see cref="View.MainWindow"/>
/// </summary>
internal partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the loaded file
    /// </summary>
    [ObservableProperty]
    private FileEntry _file = new();

    /// <summary>
    /// Gets or sets the list with the hash types
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<IdNameEntry> _hashTypes = new();

    /// <summary>
    /// Gets or sets the selected hash type
    /// </summary>
    [ObservableProperty]
    private IdNameEntry? _selectedHashType;

    /// <summary>
    /// Gets or sets the inserted hash value
    /// </summary>
    [ObservableProperty]
    private string _hashInput = string.Empty;

    /// <summary>
    /// Gets or sets the hash compare result
    /// </summary>
    [ObservableProperty]
    private string _hashCompareResult = "/";

    /// <summary>
    /// Gets or sets the compare result color
    /// </summary>
    [ObservableProperty]
    private SolidColorBrush _compareResultColor = new(Colors.White);

    /// <summary>
    /// Occurs when the user changes the hash input value
    /// </summary>
    /// <param name="value">The value</param>
    partial void OnHashInputChanged(string value)
    {
        CompareHashValues();
    }

    /// <summary>
    /// Occurs when the user selects another hash type
    /// </summary>
    /// <param name="value">The selected value</param>
    partial void OnSelectedHashTypeChanged(IdNameEntry? value)
    {
        CompareHashValues();
    }

    /// <summary>
    /// Compares the hash values
    /// </summary>
    private void CompareHashValues()
    {
        if (string.IsNullOrEmpty(HashInput) || SelectedHashType == null)
        {
            ResetInputCheck();
            return;
        }

        var hashValue = SelectedHashType.Id switch
        {
            1 => File.HashMd5,
            2 => File.HashSha1,
            3 => File.HashSha256,
            4 => File.HashSha384,
            5 => File.HashSha512,
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(hashValue))
        {
            ResetInputCheck();
            return;
        }

        if (hashValue.Equals(HashInput, StringComparison.OrdinalIgnoreCase))
        {
            HashCompareResult = "Hash values equals.";
            CompareResultColor = new SolidColorBrush(Colors.Green);
        }
        else
        {
            HashCompareResult = "Hash values not equals.";
            CompareResultColor = new SolidColorBrush(Colors.Red);
        }

        return;

        void ResetInputCheck()
        {
            HashCompareResult = "/";
            CompareResultColor = new SolidColorBrush(Colors.White);
        }
    }

    /// <summary>
    /// Loads the data
    /// </summary>
    /// <param name="filepath">The path of the file</param>
    public async void LoadData(string filepath)
    {
        File = await FileHelper.LoadFileDataAsync(filepath) ?? new FileEntry();

        HashTypes = new ObservableCollection<IdNameEntry>(new List<IdNameEntry>
        {
            new(1, "MD5"),
            new(2, "SHA1"),
            new(3, "SHA-256"),
            new(4, "SHA-384"),
            new(5, "SHA-512")
        });

        SelectedHashType = HashTypes.FirstOrDefault(f => f.Id == 1);
    }

    /// <summary>
    /// Occurs when the user hits the copy button (detail tab) and copies the content to the clipboard
    /// </summary>
    [RelayCommand]
    private void CopyDetails()
    {
        var entry = new
        {
            File.Name,
            File.Path,
            File.Size,
            File.CreationDateTime,
            File.LastAccessDateTime,
            File.LastWriteDateTime
        };

        CopyToClipboard(entry.CreateValueTable());
    }

    /// <summary>
    /// Occurs when the user hits the copy button (hash tab) and copies the content to the clipboard
    /// </summary>
    [RelayCommand]
    private void CopyHashValues()
    {
        var entry = new
        {
            Md5 = File.HashMd5,
            Sha1 = File.HashSha1,
            Sha256 = File.HashSha256,
            Sha384 = File.HashSha384,
            Sha512 = File.HashSha512
        };

        CopyToClipboard(entry.CreateValueTable());
    }
}