namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia.Data.Converters;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// Converts from a <see cref="Type" /> or <see cref="Enum" /> to a display name.
/// </summary>
public class ToDisplayNameConverter : IValueConverter {
    /// <summary>
    /// A static instance of this converter.
    /// </summary>
    public static readonly ToDisplayNameConverter Instance = new(Resolver.Resolve<IProjectService>());

    private readonly IProjectService _projectService;

    private ToDisplayNameConverter(IProjectService projectService) {
        this._projectService = projectService;
    }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            Enum enumValue => this.GetEnumName(enumValue),
            Type type => type.GetTypeDisplayName(),
            INameable nameable => nameable.Name,
            _ => value?.GetType().GetTypeDisplayName() ?? string.Empty
        };
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }

    private string GetEnumName(Enum enumValue) {
        var enumType = enumValue.GetType();
        if (enumType.GetCustomAttribute<FlagsAttribute>() != null) {
            var displayName = string.Empty;
            var isLayers = enumType == typeof(Layers) && this._projectService.CurrentProject != null;
            foreach (var value in Enum.GetValues(enumType).OfType<Enum>().Where(enumValue.HasFlag)) {
                var enumName = isLayers ? this._projectService.CurrentProject.Settings.LayerSettings.GetName((Layers)value) : value.GetEnumDisplayName();
                displayName = string.IsNullOrEmpty(displayName) ? enumName : $"{displayName}, {enumName}";
            }

            return displayName;
        }

        return enumValue.GetEnumDisplayName();
    }
}