namespace Macabresoft.Macabre2D.Framework.Layout;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;

public enum LayoutDimensionType {
    /// <summary>
    /// The dimension is automatically sized to its content.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// The dimension is sized as a weighted proportion of remaining space.
    /// </summary>
    Proportional = 1,

    /// <summary>
    /// The dimension is sized in game units.
    /// </summary>
    Units = 2
}

/// <summary>
/// A layout dimension. Defines size for rows, columns, and anything relatively sized under a <see cref="ILayoutArranger" />.
/// </summary>
[DataContract]
public class LayoutDimension {
    /// <summary>
    /// An empty <see cref="LayoutDimension" />.
    /// </summary>
    public static readonly LayoutDimension Empty = new();

    private float _amount = 1f;
    private LayoutDimensionType _dimensionType;

    /// <summary>
    /// Occurs when dimensions have changed.
    /// </summary>
    public event EventHandler? DimensionChanged;

    /// <summary>
    /// Gets or sets the actual length. This is the size in units of this dimension (width for columns, height for rows).
    /// </summary>
    public float ActualLength { get; set; }

    /// <summary>
    /// Gets or sets the amount of a dimension this takes up. For <see cref="LayoutDimensionType.Auto" />,
    /// this value does nothing. For <see cref="LayoutDimensionType.Proportional" /> this is a weighted
    /// proportion of remaining space in the layout. For <see cref="LayoutDimensionType.Units" />, this is
    /// the length in units.
    /// </summary>
    [DataMember]
    public float Amount {
        get => this._amount;
        set {
            this._amount = Math.Max(0f, value);

            if (this.DimensionType == LayoutDimensionType.Units) {
                this.ActualLength = this._amount;
            }

            this.DimensionChanged.SafeInvoke(this);
        }
    }

    /// <summary>
    /// Gets or sets the dimension type.
    /// </summary>
    [DataMember]
    public LayoutDimensionType DimensionType {
        get => this._dimensionType;
        set {
            if (value != this._dimensionType) {
                this._dimensionType = value;

                if (this._dimensionType == LayoutDimensionType.Proportional) {
                    this._amount = 1f;
                }

                this.DimensionChanged.SafeInvoke(this);
            }
        }
    }

    /// <summary>
    /// Gets or sets the position. This is the position in units of where this dimension starts (x coordinate for columns, y coordinate for rows).
    /// </summary>
    public float Position { get; set; }
}