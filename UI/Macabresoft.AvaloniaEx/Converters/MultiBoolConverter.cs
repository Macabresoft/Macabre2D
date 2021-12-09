namespace Macabresoft.AvaloniaEx;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

/// <summary>
/// Converts multiple boolean values into a single bool.
/// </summary>
public class MultiBoolConverter : IMultiValueConverter {
    /// <summary>
    /// Gets or sets a value indicating whether this requires all values to return true. If set to false, only a single bool
    /// will need to be true; otherwise, all will need to be true.
    /// </summary>
    public bool RequireAllTrue { get; set; }

    /// <inheritdoc />
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
        var boolValues = values.OfType<bool>().ToList();

        if (boolValues.Any()) {
            return this.RequireAllTrue ? boolValues.All(x => x) : boolValues.Any(x => x);
        }

        return false;
    }
}