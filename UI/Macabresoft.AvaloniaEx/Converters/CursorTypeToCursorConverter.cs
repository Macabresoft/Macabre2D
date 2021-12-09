namespace Macabresoft.AvaloniaEx;

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Input;

/// <summary>
/// Converts from a <see cref="StandardCursorType" /> to a <see cref="Cursor" />.
/// </summary>
public class CursorTypeToCursorConverter : IValueConverter {
    private readonly Dictionary<StandardCursorType, Cursor> _cursorTypeToCursor = new() {
        { StandardCursorType.None, null }
    };

    /// <summary>
    /// Gets a singleton instance.
    /// </summary>
    public static CursorTypeToCursorConverter Instance { get; } = new();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is StandardCursorType cursorType) {
            if (!this._cursorTypeToCursor.TryGetValue(cursorType, out var cursor)) {
                cursor = new Cursor(cursorType);
                this._cursorTypeToCursor.Add(cursorType, cursor);
            }

            return cursor;
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}