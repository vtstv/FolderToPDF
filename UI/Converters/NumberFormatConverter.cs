// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Globalization;
using System.Windows.Data;

namespace FolderToPDF.UI.Converters;

/// <summary>
/// Formats numeric values (integer, long, double) with thousands separators and optional decimals.
/// </summary>
public class NumberFormatConverter : IValueConverter
{
    /// <summary>
    /// Formats a numeric value as a localized string with thousands digit grouping (e.g. 12,345).
    /// </summary>
    /// <param name="value">The numeric value to format.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">Optional formatting parameter.</param>
    /// <param name="culture">The culture to use for grouping and decimal formatting.</param>
    /// <returns>A formatted numeric string.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intVal)
            return intVal.ToString("N0", culture);
        if (value is long longVal)
            return longVal.ToString("N0", culture);
        if (value is double dblVal)
            return dblVal.ToString("N1", culture);

        return value?.ToString() ?? "0";
    }

    /// <summary>
    /// Reverse conversion is not supported for display-only number strings.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
