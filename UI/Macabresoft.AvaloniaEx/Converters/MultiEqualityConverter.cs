namespace Macabresoft.AvaloniaEx;

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

/// <summary>
/// Takes a collection of values and returns true if they are all equal.
/// </summary>
public class MultiEqualityConverter : IMultiValueConverter {
    /// <inheritdoc />
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
        var result = true;
        if (values.Count > 1) {
            for (var i = 1; i < values.Count; i++) {
                if (values[i - 1] != values[i]) {
                    result = false;
                    break;
                }
            }
        }

        return result;
    }
}