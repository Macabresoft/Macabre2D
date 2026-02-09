namespace Macabre2D.UI.Common;

using System;
using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

/// <summary>
/// Converter that returns a value based on whether or not there are any items in the collection.
/// </summary>
public class CollectionHasItemsConverter : IValueConverter {

    /// <summary>
    /// A version of the converter that returns true when the collection has at least one item.
    /// </summary>
    public static readonly IValueConverter HasItems = new CollectionHasItemsConverter();

    /// <summary>
    /// A version of the converter that returns true when the collection has no items.
    /// </summary>
    public static readonly IValueConverter HasNoItems = new CollectionHasItemsConverter(false);

    private readonly bool _returnTrueForAny;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionHasItemsConverter" /> class.
    /// </summary>
    public CollectionHasItemsConverter() : this(true) {
    }

    private CollectionHasItemsConverter(bool returnTrueForAny) {
        this._returnTrueForAny = returnTrueForAny;
    }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is ICollection collection && collection.Count > 0 == this._returnTrueForAny;

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}