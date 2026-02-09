namespace Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Macabre2D.Framework;

public class MetadataToNameConverter : IValueConverter {

    /// <summary>
    /// Gets an instance that converts <see cref="ContentMetadata" /> into its full path.
    /// </summary>
    public static readonly IValueConverter FullPath = new MetadataToNameConverter(MetadataNameKind.FullPath);

    /// <summary>
    /// Gets an instance that converts <see cref="ContentMetadata" /> into its name with the file extension.
    /// </summary>
    public static readonly IValueConverter WithExtension = new MetadataToNameConverter(MetadataNameKind.WithExtension);

    /// <summary>
    /// Gets an instance that converts <see cref="ContentMetadata" /> into its name without the file extension.
    /// </summary>
    public static readonly IValueConverter WithoutExtension = new MetadataToNameConverter();

    private readonly MetadataNameKind _kind;

    public MetadataToNameConverter() : this(MetadataNameKind.WithExtension) {
    }

    private MetadataToNameConverter(MetadataNameKind kind) {
        this._kind = kind;
    }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is ContentMetadata metadata) {
            return this._kind switch {
                MetadataNameKind.WithoutExtension => metadata.GetFileNameWithoutExtension(),
                MetadataNameKind.WithExtension => metadata.GetFileName(),
                MetadataNameKind.FullPath => metadata.GetContentPath(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    private enum MetadataNameKind {
        WithoutExtension,
        WithExtension,
        FullPath
    }
}