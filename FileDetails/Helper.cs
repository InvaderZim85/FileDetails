using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Serialization.Advanced;

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
        /// Gets the md5 hash of the provided file
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>The md5 hash</returns>
        public static string GetMd5Hash(FileInfo file)
        {
            if (file == null)
                return "";

            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(file.FullName))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash);
                    }
                }
            }
            catch (Exception)
            {
                return "";
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
            var directories = Directory.GetDirectories(directory.FullName, "*", SearchOption.TopDirectoryOnly);

            var error = false;
            var size = 0L;
            var fileCount = 0;
            foreach (var dir in directories)
            {
                try
                {
                    var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                    fileCount += files.Length;

                    Parallel.ForEach(files, file => { size += new FileInfo(file).Length; });
                }
                catch (Exception)
                {
                    error = true;
                }
            }

            return (size, fileCount, directories.Length, error);
        }
    }
}
