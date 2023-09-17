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
        var displayName = string.Empty;

        if (enumType.GetCustomAttribute<FlagsAttribute>() != null) {
            if (enumType == typeof(Layers) && this._projectService.CurrentProject != null) {
                foreach (var value in Enum.GetValues(enumType).OfType<Enum>().Where(enumValue.HasFlag)) {
                    var enumName = this._projectService.CurrentProject.LayerSettings.GetName((Layers)value);
                    displayName = string.IsNullOrEmpty(displayName) ? enumName : $"{displayName}, {enumName}";
                }
            }
            else {
                foreach (var value in Enum.GetValues(enumType).OfType<Enum>().Where(enumValue.HasFlag)) {
                    var enumName = value.GetEnumDisplayName();
                    displayName = string.IsNullOrEmpty(displayName) ? enumName : $"{displayName}, {enumName}";
                }
            }
        }
        else if (enumType == typeof(InputAction) && this._projectService.CurrentProject != null) {
            displayName = this._projectService.CurrentProject.InputSettings.GetName((InputAction)enumValue);
        }
        else {
            displayName = enumValue.GetEnumDisplayName();
        }

        return displayName;
    }
}