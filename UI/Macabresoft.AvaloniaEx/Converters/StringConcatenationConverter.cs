namespace Macabresoft.AvaloniaEx;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

public class StringConcatenationConverter : IMultiValueConverter {
    public char? ConcatenationCharacter { get; set; }

    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
        var stringValues = values.OfType<string>().ToList();
        if (stringValues.Any()) {
            return this.ConcatenationCharacter != null ? string.Join(this.ConcatenationCharacter.Value, stringValues) : string.Concat(stringValues);
        }

        return AvaloniaProperty.UnsetValue;
    }
}