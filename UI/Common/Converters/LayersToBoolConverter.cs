namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;

public class LayersToBoolConverter : IValueConverter {
    private Layers _componentLayers;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var result = false;
        if ((value is Layers componentLayers || Enum.TryParse(value?.ToString(), out componentLayers)) && (parameter is Layers layerBit || Enum.TryParse(parameter?.ToString(), out layerBit))) {
            this._componentLayers = componentLayers;
            result = componentLayers.HasFlag(layerBit);
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is bool hasBit && (parameter is Layers layerBit || Enum.TryParse(parameter.ToString(), out layerBit))) {
            if (hasBit) {
                this._componentLayers |= layerBit;
            }
            else {
                this._componentLayers &= ~layerBit;
            }
        }
        else {
            throw new ArgumentException($"'{nameof(parameter)}' is not valid. It should be a member of '{nameof(Layers)}'");
        }

        return this._componentLayers;
    }
}