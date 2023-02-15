namespace Macabresoft.Macabre2D.Framework.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A grid for layout purposes.
/// </summary>
public class LayoutGrid : Entity, ILayoutArranger {
    [DataMember]
    private readonly List<LayoutDimension> _columns = new();

    [DataMember]
    private readonly List<LayoutDimension> _rows = new();

    private IBoundable? _boundable;
    private BoundingArea _boundingArea = BoundingArea.Empty;

    /// <summary>
    /// Adds a column.
    /// </summary>
    /// <returns>The added column.</returns>
    public LayoutDimension AddColumn() {
        var dimension = new LayoutDimension();
        this._columns.Add(dimension);
        return dimension;
    }

    /// <summary>
    /// Adds a row.
    /// </summary>
    /// <returns>The added row.</returns>
    public LayoutDimension AddRow() {
        var dimension = new LayoutDimension();
        this._rows.Add(dimension);
        return dimension;
    }

    /// <inheritdoc />
    public BoundingArea GetBoundingArea(int row, int column) {
        if (!this._boundingArea.IsEmpty && this.TryGetColumn(column, out var columnDimension) && this.TryGetRow(row, out var rowDimension)) {
            var start = new Vector2(columnDimension.Position, rowDimension.Position);
            return new BoundingArea(start, columnDimension.Length, rowDimension.Length);
        }

        return BoundingArea.Empty;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this._boundable != null) {
            this._boundable.BoundingAreaChanged -= this.Boundable_BoundingAreaChanged;
        }

        if (this.TryGetParentEntity(out this._boundable)) {
            this._boundable.BoundingAreaChanged += this.Boundable_BoundingAreaChanged;
        }

        this.Rearrange();
    }

    /// <summary>
    /// Removes the specified column.
    /// </summary>
    /// <param name="column">The column.</param>
    public void RemoveColumn(LayoutDimension column) {
        this._columns.Remove(column);
        this.Rearrange();
    }

    /// <summary>
    /// Removes a column at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    public void RemoveColumnAt(int index) {
        if (index > 0 && index < this._columns.Count) {
            this._columns.RemoveAt(index);
            this.Rearrange();
        }
    }

    /// <summary>
    /// Removes the specified row.
    /// </summary>
    /// <param name="row">The row.</param>
    public void RemoveRow(LayoutDimension row) {
        this._rows.Remove(row);
        this.Rearrange();
    }

    /// <summary>
    /// Removes a row at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    public void RemoveRowAt(int index) {
        if (index > 0 && index < this._rows.Count) {
            this._rows.RemoveAt(index);
            this.Rearrange();
        }
    }

    /// <inheritdoc />
    public void RequestRearrange(ILayoutArrangeable requester) {
        if (this.IsInitialized && this.ShouldRearrange(requester)) {
            this.Rearrange();
        }
    }

    private void Boundable_BoundingAreaChanged(object? sender, EventArgs e) {
        this.Rearrange();
    }

    private static void ClearDimensions(IEnumerable<LayoutDimension> dimensions) {
        foreach (var dimension in dimensions) {
            dimension.Length = 0f;
            dimension.Position = 0f;
        }
    }

    private static void EvaluateDimensions(IReadOnlyCollection<LayoutDimension> dimensions, float startingPosition, float totalLength, bool increasingStartingPosition) {
        var totalAmount = dimensions.Sum(x => x.Amount);
        if (totalAmount > 0f) {
            foreach (var dimension in dimensions) {
                dimension.Length = totalLength * (dimension.Amount / totalAmount);
                dimension.Position = startingPosition;

                if (increasingStartingPosition) {
                    startingPosition += dimension.Length;
                }
                else {
                    startingPosition -= dimension.Length;
                }
            }
        }
        else {
            ClearDimensions(dimensions);
        }
    }

    private void Rearrange() {
        this._boundingArea = this._boundable?.BoundingArea ?? BoundingArea.Empty;

        if (this._boundingArea.IsEmpty) {
            ClearDimensions(this._rows);
            ClearDimensions(this._columns);
        }
        else {
            EvaluateDimensions(this._rows, this._boundingArea.Maximum.Y, this._boundingArea.Height, false);
            EvaluateDimensions(this._columns, this._boundingArea.Minimum.X, this._boundingArea.Width, true);
        }

        // TODO: reposition children of type ILayoutArrangeable
    }

    private bool ShouldRearrange(ILayoutArrangeable requester) {
        return this.TryGetRow(requester.Row, out _) || this.TryGetColumn(requester.Column, out _);
    }

    private bool TryGetColumn(int column, out LayoutDimension dimension) {
        if (column < 0 || column > this._columns.Count) {
            dimension = LayoutDimension.Empty;
            return false;
        }

        dimension = this._columns[column];
        return true;
    }

    private bool TryGetRow(int row, out LayoutDimension dimension) {
        if (row < 0 || row > this._rows.Count) {
            dimension = LayoutDimension.Empty;
            return false;
        }

        dimension = this._rows[row];
        return true;
    }
}