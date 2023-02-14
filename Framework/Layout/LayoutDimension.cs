namespace Macabresoft.Macabre2D.Framework.Layout;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A layout dimension. Defines size for rows, columns, and anything relatively sized under a <see cref="ILayoutArranger" />.
/// </summary>
[DataContract]
public class LayoutDimension {
    /// <summary>
    /// </summary>
    public static readonly LayoutDimension Empty = new();

    private float _amount;

    /// <summary>
    /// Occurs when dimensions have changed.
    /// </summary>
    public event EventHandler? DimensionChanged;

    /// <summary>
    /// Gets or sets the amount of a dimension this takes up. This value is technically a percentage, but the max value can be
    /// anything. For instance, if this is set to 1 and there is another dimension in its row with a dimension of 3, this will
    /// be 1/4 of the row.
    /// </summary>
    [DataMember]
    public float Amount {
        get => this._amount;
        set {
            this._amount = Math.Max(0f, value);
            this.DimensionChanged.SafeInvoke(this);
        }
    }

    /// <summary>
    /// Gets or sets the length. This is the size in units of this dimension (width for columns, height for rows).
    /// </summary>
    public float Length { get; set; }

    /// <summary>
    /// Gets or sets the position. This is the position in units of where this dimension starts (x coordinate for columns, y coordinate for rows).
    /// </summary>
    public float Position { get; set; }
}