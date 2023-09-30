using FileDetails.Common;
using System.IO;

namespace FileDetails.Models;

/// <summary>
/// Represents a file
/// </summary>
internal sealed class FileEntry
{
    /// <summary>
    /// Gets the name of the file
    /// </summary>
    public string Name { get; } = "/";

    /// <summary>
    /// Gets the path of the file
    /// </summary>
    public string Path { get; } = "/";

    /// <summary>
    /// Gets the size of the file in a readable format
    /// </summary>
    public string Size { get; } = "/";

    /// <summary>
    /// Gets the creation date / time
    /// </summary>
    public string CreationDateTime { get; } = "/";

    /// <summary>
    /// Gets the last write date / time
    /// </summary>
    public string LastWriteDateTime { get; } = "/";

    /// <summary>
    /// Gets the last access date / time
    /// </summary>
    public string LastAccessDateTime { get; } = "/";

    /// <summary>
    /// Gets or sets the MD5 hash
    /// </summary>
    public string HashMd5 { get; set; } = "/";

    /// <summary>
    /// Gets or sets the SHA1 hash
    /// </summary>
    public string HashSha1 { get; set; } = "/";

    /// <summary>
    /// Gets or sets the SHA256 hash
    /// </summary>
    public string HashSha256 { get; set; } = "/";

    /// <summary>
    /// Gets or sets the SHA384 hash
    /// </summary>
    public string HashSha384 { get; set; } = "/";

    /// <summary>
    /// Gets or sets the SHA512 hash
    /// </summary>
    public string HashSha512 { get; set; } = "/";

    /// <summary>
    /// Creates a new, empty instance of the <see cref="FileEntry"/>
    /// </summary>
    public FileEntry() { }

    /// <summary>
    /// Creates a new instance of the <see cref="FileEntry"/>
    /// </summary>
    /// <param name="file">The file</param>
    public FileEntry(FileInfo file)
    {
        Name = file.Name;
        Path = file.FullName;
        Size = file.Length.ConvertSize();
        CreationDateTime = file.CreationTime.ToStringDate();
        LastWriteDateTime = file.LastWriteTime.ToStringDate();
        LastAccessDateTime = file.LastAccessTime.ToStringDate();
    }
}