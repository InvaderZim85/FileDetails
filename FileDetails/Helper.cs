using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using FileDetails.DataObjects;
using ZimLabs.TableCreator;

namespace FileDetails
{
    /// <summary>
    /// Provides several helper functions
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        /// Gets the hash of the given file according to the given type
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>the hash value</returns>
        public static (string md5, string sha1, string sha256) GetHash(FileInfo file)
        {
            static string ConvertToString(byte[] byteArray)
            {
                return BitConverter.ToString(byteArray);
            }

            try
            {
                var fileBytes = File.ReadAllBytes(file.FullName);

                var md5 = GetMd5Hash(fileBytes);
                var sha1 = GetSha1Hash(fileBytes);
                var sha256 = GetSha256Hash(fileBytes);

                return (ConvertToString(md5), ConvertToString(sha1), ConvertToString(sha256));
            }
            catch
            {
                return ("", "", "");
            }
        }

        /// <summary>
        /// Computes the MD5 hash for the given stream
        /// </summary>
        /// <param name="byteArray">The byte array of the file</param>
        /// <returns>The hash bytes</returns>
        private static byte[] GetMd5Hash(byte[] byteArray)
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(byteArray);
        }

        /// <summary>
        /// Computes the SHA1 hash for the given stream
        /// </summary>
        /// <param name="byteArray">The byte array of the file</param>
        /// <returns>The hash bytes</returns>
        private static byte[] GetSha1Hash(byte[] byteArray)
        {
            using var sha = SHA1.Create();
            return sha.ComputeHash(byteArray);
        }

        /// <summary>
        /// Computes the SHA256 hash for the given stream
        /// </summary>
        /// <param name="byteArray">The byte array of the file</param>
        /// <returns>The hash bytes</returns>
        private static byte[] GetSha256Hash(byte[] byteArray)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(byteArray);
        }

        /// <summary>
        /// Converts the file size into a readable format
        /// </summary>
        /// <param name="size">The size</param>
        /// <returns>The readable size</returns>
        public static string ConvertFileSize(long size)
        {
            return size switch
            {
                < 1024 => $"{size} Bytes",
                _ when size < Math.Pow(1024, 2) => $"{size / 1024d:N2} KB",
                _ when size >= Math.Pow(1024, 2) && size < Math.Pow(1024, 3) => $"{size / Math.Pow(1024, 2):N2} MB",
                _ when size >= Math.Pow(1024, 3) => $"{size / Math.Pow(1024, 3):N2} GB",
                _ => size.ToString()
            };
        }

        /// <summary>
        /// Converts the date into a readable format (Format: yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="date">The date</param>
        /// <returns>The formatted date</returns>
        public static string ConvertDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Gets the path
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The path</returns>
        public static string GetPath(IEnumerable<string> args)
        {
            var result = args.Aggregate("", (current, entry) => current + $"{entry} ");

            return result.Trim();
        }

        /// <summary>
        /// Checks if the given path is valid
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>true when the path is valid, otherwise false</returns>
        public static bool CanStart(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        /// <summary>
        /// Copies the content of the file to the clip board
        /// </summary>
        /// <param name="file">The file</param>
        public static void CopyToClipboard(FileEntry file)
        {
            var properties = file.GetType().GetProperties();

            var content = properties.Select(s => new
            {
                Key = s.Name,
                Value = s.GetValue(file)
            }).CreateTable();

            Clipboard.SetText(content, TextDataFormat.Text);
        }
    }
}
