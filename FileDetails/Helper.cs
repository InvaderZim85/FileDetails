using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace FileDetails
{
    public static class Helper
    {
        /// <summary>
        /// The different property access types
        /// </summary>
        [Flags]
        public enum PropertyAccessType
        {
            /// <summary>
            /// All properties
            /// </summary>
            Any = 0,
            /// <summary>
            /// Only properties which can be read
            /// </summary>
            Read = 1,
            /// <summary>
            /// Only properties which can be write
            /// </summary>
            Write = 2
        }

        /// <summary>
        /// Gets the hash of the given file according to the given type
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>the hash value</returns>
        public static (string md5, string sha1, string sha256) GetHash(FileInfo file)
        {
            if (file == null)
                return ("", "", "");

            string ConvertToString(byte[] byteArray)
            {
                return BitConverter.ToString(byteArray).Replace("-", "").ToLower();
            }

            try
            {
                using (var stream = File.OpenRead(file.FullName))
                {
                    var md5 = GetMd5Hash(stream);
                    var sha1 = GetSha1Hash(stream);
                    var sha256 = GetSha256Hash(stream);

                    return (ConvertToString(md5), ConvertToString(sha1),
                        ConvertToString(sha256));
                }
            }
            catch
            {
                return ("", "", "");
            }
        }

        /// <summary>
        /// Computes the SHA-256 hash for the given stream
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <returns>The hash bytes</returns>
        private static byte[] GetMd5Hash(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(stream);
            }
        }

        /// <summary>
        /// Computes the SHA-256 hash for the given stream
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <returns>The hash bytes</returns>
        private static byte[] GetSha1Hash(Stream stream)
        {
            using (var sha = new SHA1Managed())
            {
                return sha.ComputeHash(stream);
            }
        }

        /// <summary>
        /// Computes the SHA-256 hash for the given stream
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <returns>The hash bytes</returns>
        private static byte[] GetSha256Hash(Stream stream)
        {
            using (var sha = new SHA256Managed())
            {
                return sha.ComputeHash(stream);
            }
        }

        /// <summary>
        /// Gets all properties of the deriving class
        /// </summary>
        /// <param name="value">The class value</param>
        /// <param name="accessType">The <see cref="PropertyAccessType"/> of the properties (optional, default = Read | Write)</param>
        /// <param name="onlyPublic"><see langword="true"/> to get only public properties, otherwise <see langword="false"/> (optional, default = <see langword="true"/></param>
        /// <returns></returns>
        public static List<string> GetProperties<T>(T value,
            PropertyAccessType accessType = PropertyAccessType.Read | PropertyAccessType.Write, bool onlyPublic = true)
            where T : class
        {
            if (value == null)
                return new List<string>();

            if (!value.GetType().IsClass)
                return new List<string>();

            List<PropertyInfo> properties;
            switch ((int) accessType)
            {
                case 1:
                    properties = value.GetType().GetProperties().Where(w => w.CanRead).ToList();
                    break;
                case 2:
                    properties = value.GetType().GetProperties().Where(w => w.CanWrite).ToList();
                    break;
                case 3:
                    properties = value.GetType().GetProperties().Where(w => w.CanRead && w.CanWrite).ToList();
                    break;
                default:
                    properties = value.GetType().GetProperties().ToList();
                    break;
            }

            return onlyPublic
                ? properties.Where(w => w.GetMethod.IsPublic).Select(s => s.Name).ToList()
                : properties.Select(s => s.Name).ToList();
        }

        /// <summary>
        /// Checks if the path is a file or a directory
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>true when the path is a directory, otherwise false</returns>
        public static bool IsDirectory(string path)
        {
            var attributes = File.GetAttributes(path);

            return attributes.HasFlag(FileAttributes.Directory);
        }

        /// <summary>
        /// Gets the size of a file / directory
        /// </summary>
        /// <param name="info">The file / directory info object</param>
        /// <returns>The size</returns>
        public static (string size, string count) GetSizeFileCount(object info)
        {
            try
            {
                var size = 0L;
                var count = "1";
                var error = false;
                switch (info)
                {
                    case DirectoryInfo dirInfo:
                        var dirResult = GetDirectorySizeFileCount(dirInfo);
                        count = $"{dirResult.count:N0} file(s), {dirResult.dirCount:N0} folder(s)";
                        size = dirResult.size;
                        error = dirResult.error;
                        break;
                    case FileInfo fileInfo:
                        size = fileInfo.Length;
                        break;
                }

                var sizeValue = ConvertFileSize(size);
                if (error)
                    sizeValue = $"~{size:N0} bytes";

                return (sizeValue, count);
            }
            catch (Exception)
            {
                return ("0", "0");
            }
        }

        /// <summary>
        /// Converts the file size into a readable format
        /// </summary>
        /// <param name="size">The size</param>
        /// <returns>The readable size</returns>
        private static string ConvertFileSize(long size)
        {
            switch (size)
            {
                case var _ when size < 1024:
                    return $"{size} Bytes";
                case var _ when size >= 1024 && size < Math.Pow(1024, 2):
                    return $"{size / 1024:N2} KB";
                case var _ when size >= Math.Pow(1024, 2) && size < Math.Pow(1024, 3):
                    return $"{size / Math.Pow(1024, 2):N2} MB";
                case var _ when size >= Math.Pow(1024, 3):
                    return $"{size / Math.Pow(1024, 3):N2} GB";
                default:
                    return size.ToString();
            }
        }

        /// <summary>
        /// Gets the file size for a directory
        /// </summary>
        /// <param name="directory">The directory</param>
        /// <returns>The data</returns>
        private static (long size, int count, int dirCount, bool error) GetDirectorySizeFileCount(
            DirectoryInfo directory)
        {
            // Get the files of the main folder
            var mainFiles = directory.GetFiles("*.*", SearchOption.AllDirectories);
            var fileCount = mainFiles.Length;
            var size = 0L;

            var error = false;

            foreach (var file in mainFiles)
            {
                try
                {
                    size += file.Length;
                }
                catch
                {
                    error = true;
                }
            }

            var directories = Directory.GetDirectories(directory.FullName, "*", SearchOption.AllDirectories);

            return (size, fileCount, directories.Length, error);
        }
    }
}
