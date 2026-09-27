// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using System.Globalization;
using System.Windows.Data;

namespace FolderToPDF.UI.Converters;

/// <summary>
/// Converts equality of a value and parameter to a boolean for XAML UI bindings (e.g. navigation tab selection),
/// and supports two-way ConvertBack binding to update bound properties when a radio or tab is clicked.
/// </summary>
public class EqualityToBooleanConverter : IValueConverter
{
    /// <summary>
    /// Checks whether the bound value equals the converter parameter string.
    /// </summary>
    /// <param name="value">The source value produced by the binding.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The comparison parameter value.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>True if value equals parameter; otherwise false.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null && parameter == null)
            return true;

        if (value == null || parameter == null)
            return false;

        return string.Equals(value.ToString(), parameter.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Converts a true boolean state back to the parameter value when a control becomes selected.
    /// </summary>
    /// <param name="value">The boolean value indicating selection.</param>
    /// <param name="targetType">The target type to convert back into.</param>
    /// <param name="parameter">The parameter value representing this item.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The parameter converted to targetType, or <see cref="Binding.DoNothing"/> if unselected.</returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolVal && boolVal && parameter != null)
        {
            if (targetType == typeof(int) && int.TryParse(parameter.ToString(), out int intVal))
            {
                return intVal;
            }
            return parameter;
        }

        return Binding.DoNothing;
    }
}
