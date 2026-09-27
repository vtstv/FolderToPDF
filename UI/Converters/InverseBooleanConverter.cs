// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Globalization;
using System.Windows.Data;

namespace FolderToPDF.UI.Converters;

/// <summary>
/// Inverts a boolean value for XAML UI bindings (e.g. disabling UI controls while an operation is busy).
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    /// <summary>
    /// Negates the boolean input value.
    /// </summary>
    /// <param name="value">The source boolean value.</param>
    /// <param name="targetType">The target binding property type.</param>
    /// <param name="parameter">Optional converter parameter.</param>
    /// <param name="culture">Culture info.</param>
    /// <returns>True if value is false; false if value is true.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolVal)
            return !boolVal;

        return false;
    }

    /// <summary>
    /// Negates the inverted boolean value back to the source property.
    /// </summary>
    /// <param name="value">The target boolean value.</param>
    /// <param name="targetType">The source binding property type.</param>
    /// <param name="parameter">Optional converter parameter.</param>
    /// <param name="culture">Culture info.</param>
    /// <returns>The inverted boolean value.</returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolVal)
            return !boolVal;

        return false;
    }
}
