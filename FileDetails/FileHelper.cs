using FileDetails.Models;
using Serilog;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace FileDetails;

/// <summary>
/// Provides the functions for the interaction with a file
/// </summary>
internal static class FileHelper
{
    /// <summary>
    /// Loads the file information
    /// </summary>
    /// <param name="filepath">The path of the file</param>
    /// <returns>The file information</returns>
    public static async Task<FileEntry?> LoadFileDataAsync(string filepath)
    {
        if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
            return null;

        var file = new FileInfo(filepath);

        // Create a new file entry
        var result = new FileEntry(file);

        // Load the hash values
        await LoadHashValuesAsync(result);

        return result;
    }

    /// <summary>
    /// Loads the hash values
    /// </summary>
    /// <param name="file">The file</param>
    /// <returns>The awaitable task</returns>
    private static async Task LoadHashValuesAsync(FileEntry file)
    {
        try
        {
            var bytes = await File.ReadAllBytesAsync(file.Path);

            file.HashMd5 = await GetMd5HashAsync(bytes);
            file.HashSha1 = await GetSha1HashAsync(bytes);
            file.HashSha256 = await GetSha256HashAsync(bytes);
            file.HashSha384 = await GetSha384HashAsync(bytes);
            file.HashSha512 = await GetSha512HashAsync(bytes);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error has occurred while loading the Hash values of the file '{path}'", file.Path);
        }
    }

    /// <summary>
    /// Calculates the MD5 hash
    /// </summary>
    /// <param name="bytes">The file bytes</param>
    /// <returns>The hash value</returns>
    private static async Task<string> GetMd5HashAsync(byte[] bytes)
    {
        var result = await ComputeHashAsync(MD5.HashDataAsync, bytes);
        return ConvertToString(result);
    }

    /// <summary>
    /// Calculates the SHA1 hash
    /// </summary>
    /// <param name="bytes">The file bytes</param>
    /// <returns>The hash value</returns>
    private static async Task<string> GetSha1HashAsync(byte[] bytes)
    {
        var result = await ComputeHashAsync(SHA1.HashDataAsync, bytes);
        return ConvertToString(result);
    }

    /// <summary>
    /// Calculates the SHA256 hash
    /// </summary>
    /// <param name="bytes">The file bytes</param>
    /// <returns>The hash value</returns>
    private static async Task<string> GetSha256HashAsync(byte[] bytes)
    {
        var result = await ComputeHashAsync(SHA256.HashDataAsync, bytes);
        return ConvertToString(result);
    }

    /// <summary>
    /// Calculates the SHA384 hash
    /// </summary>
    /// <param name="bytes">The file bytes</param>
    /// <returns>The hash value</returns>
    private static async Task<string> GetSha384HashAsync(byte[] bytes)
    {
        var result = await ComputeHashAsync(SHA384.HashDataAsync, bytes);
        return ConvertToString(result);
    }

    /// <summary>
    /// Calculates the SHA512 hash
    /// </summary>
    /// <param name="bytes">The file bytes</param>
    /// <returns>The hash value</returns>
    private static async Task<string> GetSha512HashAsync(byte[] bytes)
    {
        var result = await ComputeHashAsync(SHA512.HashDataAsync, bytes);
        return ConvertToString(result);
    }

    /// <summary>
    /// Computes the hash of the given bytes with the provides hash function
    /// </summary>
    /// <param name="computeHash">The hash function</param>
    /// <param name="bytes">The bytes of the file</param>
    /// <returns>The hash value</returns>
    private static async Task<byte[]> ComputeHashAsync(Func<Stream, CancellationToken, ValueTask<byte[]>> computeHash,
        byte[] bytes)
    {
        await using var stream = new MemoryStream(bytes);
        return await computeHash(stream, default);
    }

    /// <summary>
    /// Converts the hash bytes into a readable string (the dashes will be removed)
    /// </summary>
    /// <param name="bytes">The hash bytes</param>
    /// <returns>The readable string</returns>
    private static string ConvertToString(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}