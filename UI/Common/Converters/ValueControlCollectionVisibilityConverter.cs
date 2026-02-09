namespace Macabre2D.UI.Common;

using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

/// <summary>
/// A converter used to filter <see cref="ValueControlCollection" />.
/// </summary>
public class ValueControlCollectionVisibilityConverter : IMultiValueConverter {
    /// <summary>
    /// Gets an instance of <see cref="ValueControlCollectionVisibilityConverter" />.
    /// </summary>
    public static ValueControlCollectionVisibilityConverter Instance { get; } = new();

    /// <inheritdoc />
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
        if (values.Count == 2 && values[0] is ValueControlCollection collection && values[1] is string filterText) {
            return string.IsNullOrEmpty(filterText) ||
                   collection.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase) ||
                   collection.ValueControls.Any(x => x.Title.Contains(filterText, StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }
}