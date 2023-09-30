using ControlzEx.Theming;
using Microsoft.WindowsAPICodePack.Taskbar;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FileDetails.Common;

/// <summary>
/// Provides several helper functions
/// </summary>
internal static class Helper
{
    /// <summary>
    /// Contains the instance of the taskbar manager
    /// </summary>
    private static TaskbarManager? _taskbarInstance;

    /// <summary>
    /// Init the helper
    /// </summary>
    /// <param name="verbose">true to create verbose log, otherwise false</param>
    public static void InitHelper(bool verbose)
    {
        const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        if (verbose)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File("log/log_.log", rollingInterval: RollingInterval.Day, outputTemplate: template)
                .CreateLogger();
        }
        else
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log/log_.log", rollingInterval: RollingInterval.Day, outputTemplate: template)
                .CreateLogger();
        }

        if (TaskbarManager.IsPlatformSupported)
            _taskbarInstance = TaskbarManager.Instance;
    }

    /// <summary>
    /// Sets the base color and the color scheme
    /// </summary>
    /// <param name="baseColor">The base color</param>
    /// <param name="colorScheme">The scheme which should be set</param>
    public static void SetColorTheme(string baseColor, string colorScheme)
    {
        ThemeManager.Current.ChangeThemeColorScheme(Application.Current, colorScheme);
        ThemeManager.Current.ChangeThemeBaseColor(Application.Current, baseColor);
    }

    #region Various
    /// <summary>
    /// Opens the explorer and selects the specified file
    /// </summary>
    /// <param name="path">The path of the file</param>
    public static void ShowInExplorer(string path)
    {
        var arguments = Path.HasExtension(path) ? $"/n, /e, /select, \"{path}\"" : $"/n, /e, \"{path}\"";
        Process.Start("explorer.exe", arguments);
    }

    /// <summary>
    /// Opens the specified link
    /// </summary>
    /// <param name="url">The url of the link</param>
    public static void OpenLink(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
    #endregion

    #region Taskbar
    /// <summary>
    /// Sets the taskbar into an indeterminate state
    /// </summary>
    /// <param name="enable"><see langword="true"/> to set the indeterminate state, otherwise <see langword="false"/></param>
    public static void SetTaskbarIndeterminate(bool enable)
    {
        SetTaskbarState(enable, TaskbarProgressBarState.Indeterminate);
    }

    /// <summary>
    /// Sets the taskbar into an error state
    /// </summary>
    /// <param name="enable"><see langword="true"/> to set the indeterminate state, otherwise <see langword="false"/></param>
    public static void SetTaskbarError(bool enable)
    {
        SetTaskbarState(enable, TaskbarProgressBarState.Error);
    }

    /// <summary>
    /// Sets the taskbar into an pause state
    /// </summary>
    /// <param name="enable"><see langword="true"/> to set the indeterminate state, otherwise <see langword="false"/></param>
    public static void SetTaskbarPause(bool enable)
    {
        SetTaskbarState(enable, TaskbarProgressBarState.Paused);
    }

    /// <summary>
    /// Sets the taskbar state
    /// </summary>
    /// <param name="enabled"><see langword="true"/> to set the state, <see langword="false"/> to set <see cref="TaskbarProgressBarState.NoProgress"/></param>
    /// <param name="state">The desired state</param>
    private static void SetTaskbarState(bool enabled, TaskbarProgressBarState state)
    {
        try
        {
            _taskbarInstance?.SetProgressState(enabled ? state : TaskbarProgressBarState.NoProgress);
            switch (enabled)
            {
                case true when state != TaskbarProgressBarState.Indeterminate:
                    _taskbarInstance?.SetProgressValue(100, 100);
                    break;
                case false:
                    _taskbarInstance?.SetProgressValue(0, 0);
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Can't change taskbar state");
        }
    }
    #endregion

    #region Extensions
    /// <summary>
    /// Converts the value into a readable size
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="divider">The divider (optional)</param>
    /// <returns>The converted size</returns>
    public static string ConvertSize(this long value, int divider = 1024)
    {
        var result = value switch
        {
            _ when value < divider => $"{value:N0} Bytes",
            _ when value >= divider && value < Math.Pow(divider, 2) => $"{value / divider:N2} KB",
            _ when value >= Math.Pow(divider, 2) && value < Math.Pow(divider, 3) =>
                $"{value / Math.Pow(divider, 2):N2} MB",
            _ when value >= Math.Pow(divider, 3) && value <= Math.Pow(divider, 4) => $"{value / Math.Pow(divider, 3):N2} GB",
            _ when value >= Math.Pow(divider, 4) => $"{value / Math.Pow(divider, 4)} TB",
            _ => value.ToString("N0")
        };

        return value < divider ? result : $"{result} ({value:N0} bytes)";
    }

    /// <summary>
    /// Converts the date / time value into a formatted string (format: dd.MM.yyyy HH:mm:ss)
    /// </summary>
    /// <param name="value">The date / time value</param>
    /// <returns>The formatted string</returns>
    public static string ToStringDate(this DateTime value)
    {
        return value.ToString("dd.MM.yyyy HH:mm:ss");
    }

    /// <summary>
    /// Finds all visual elements of a specified type
    /// </summary>
    /// <typeparam name="T">The desired type</typeparam>
    /// <param name="depObj">The main dependency object</param>
    /// <returns>The list with the elements</returns>
    public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj) where T : DependencyObject
    {
        foreach (var rawChild in LogicalTreeHelper.GetChildren(depObj))
        {
            if (rawChild is not DependencyObject child)
                continue;

            if (child is T obj)
                yield return obj;

            foreach (var subChild in FindLogicalChildren<T>(child))
            {
                yield return subChild;
            }
        }
    }
    #endregion
}