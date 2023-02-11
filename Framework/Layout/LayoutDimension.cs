namespace Macabresoft.Macabre2D.Framework.Layout;

using System;
using Macabresoft.Core;

/// <summary>
/// A layout dimension. Defines size for rows, columns, and anything relatively sized under a <see cref="ILayoutArranger" />.
/// </summary>
public class LayoutDimension {
    private float _amount;
    private bool _isAuto;

    /// <summary>
    /// Occurs when dimensions have changed.
    /// </summary>
    public event EventHandler DimensionChanged;

    /// <summary>
    /// Gets or sets the amount of a dimension this takes up. This value is technically a percentage, but the max value can be
    /// anything. For instance, if this is set to 1 and there is another dimension in its row with a dimension of 3, this will
    /// be 1/4 of the row.
    /// </summary>
    public float Amount {
        get => this._amount;
        set {
            this._amount = value;
            this.DimensionChanged.SafeInvoke(this);
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not this dimension is automatically sized based on the size of its contents.
    /// </summary>
    public bool IsAuto {
        get => this._isAuto;
        set {
            if (this._isAuto != value) {
                this._isAuto = value;
                this.DimensionChanged.SafeInvoke(this);
            }
        }
    }
}