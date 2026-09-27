// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Globalization;
using System.Windows.Data;

namespace FolderToPDF.UI.Converters;

/// <summary>
/// Converts raw file size byte values into human-readable formatted strings (B, KB, MB).
/// </summary>
public class FileSizeConverter : IValueConverter
{
    /// <summary>
    /// Converts a file size in bytes to a formatted B, KB, or MB string representation.
    /// </summary>
    /// <param name="value">The byte size (long).</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">Optional converter parameter.</param>
    /// <param name="culture">The culture to use for numeric formatting.</param>
    /// <returns>A formatted string such as "14.2 KB" or "2.50 MB".</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var targetCulture = culture ?? CultureInfo.InvariantCulture;
        if (value is long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";
            if (bytes < 1024 * 1024)
                return (bytes / 1024.0).ToString("F1", targetCulture) + " KB";
            return (bytes / (1024.0 * 1024.0)).ToString("F2", targetCulture) + " MB";
        }
        return "0 B";
    }

    /// <summary>
    /// Reverse conversion is not supported for file size display strings.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
