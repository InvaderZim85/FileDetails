using System.IO;

namespace FileDetails.DataObjects
{
    /// <summary>
    /// Represents a file
    /// </summary>
    internal class FileEntry
    {
        /// <summary>
        /// Gets the name of the file
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the path of the file
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the size of the file
        /// </summary>
        public string Size { get; }

        /// <summary>
        /// Gets the MD5 hash
        /// </summary>
        public string HashMd5 { get; }

        /// <summary>
        /// Gets the SHA1 hash
        /// </summary>
        public string HashSha1 { get; }

        /// <summary>
        /// Gets the SHA256 hash
        /// </summary>
        public string HashSha256 { get; }

        /// <summary>
        /// Gets the creation date
        /// </summary>
        public string CreationDate { get; }

        /// <summary>
        /// Gets the last write date
        /// </summary>
        public string LastWrite { get; }

        /// <summary>
        /// Gets the last access date
        /// </summary>
        public string LastAccess { get; }

        /// <summary>
        /// Gets the attributes of the file
        /// </summary>
        public string Attributes { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="FileEntry"/>
        /// </summary>
        /// <param name="file">The original file</param>
        public FileEntry(FileInfo file)
        {
            Name = file.Name;
            Path = file.FullName;
            Size = Helper.ConvertFileSize(file.Length);
            CreationDate = file.CreationTime.ConvertDate();
            LastWrite = file.LastWriteTime.ConvertDate();
            LastAccess = file.LastAccessTime.ConvertDate();

            // Hash values
            var (md5, sha1, sha256) = Helper.GetHash(file);
            HashMd5 = md5;
            HashSha1 = sha1;
            HashSha256 = sha256;

            // Attributes
            Attributes = file.Attributes.ToString();
        }
    }
}
