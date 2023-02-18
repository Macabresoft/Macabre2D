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
        this.Rearrange(); // TODO: only arrange columns here.
        dimension.DimensionChanged += this.Column_DimensionChanged;
        return dimension;
    }

    /// <summary>
    /// Adds a row.
    /// </summary>
    /// <returns>The added row.</returns>
    public LayoutDimension AddRow() {
        var dimension = new LayoutDimension();
        this._rows.Add(dimension);
        this.Rearrange(); // TODO: only arrange rows here.
        dimension.DimensionChanged += this.Row_DimensionChanged;
        return dimension;
    }

    /// <inheritdoc />
    public BoundingArea GetBoundingArea(int row, int column) {
        if (!this._boundingArea.IsEmpty && this.TryGetColumn(column, out var columnDimension) && this.TryGetRow(row, out var rowDimension)) {
            var start = new Vector2(columnDimension.Position, rowDimension.Position);
            return new BoundingArea(start, columnDimension.ActualLength, rowDimension.ActualLength);
        }

        return BoundingArea.Empty;
    }

    /// <inheritdoc />
    public BoundingArea GetBoundingArea(int row, int column, int rowSpan, int columnSpan) {
        var result = BoundingArea.Empty;

        if (rowSpan > 0 && columnSpan > 0 && row < this._rows.Count && column < this._columns.Count) {
            rowSpan = Math.Min(this._rows.Count - row, rowSpan);
            columnSpan = Math.Min(this._columns.Count - column, columnSpan);
            if (rowSpan == 1 && columnSpan == 1) {
                result = this.GetBoundingArea(row, column);
            }
            else {
                var startingBoundingArea = this.GetBoundingArea(row, column);
                var endingBoundingArea = this.GetBoundingArea(row + rowSpan - 1, column + columnSpan - 1);
                result = new BoundingArea(
                    startingBoundingArea.Minimum.X,
                    endingBoundingArea.Maximum.X,
                    endingBoundingArea.Minimum.Y,
                    startingBoundingArea.Maximum.Y);
            }
        }

        return result;
    }

    /// <inheritdoc />
    public BoundingArea GetBoundingArea(ILayoutArrangeable arrangeable) {
        return this.GetBoundingArea(arrangeable.Row, arrangeable.Column, arrangeable.RowSpan, arrangeable.ColumnSpan);
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
        column.DimensionChanged -= this.Column_DimensionChanged;
        this.Rearrange(); // TODO: only arrange columns here.
    }

    /// <summary>
    /// Removes a column at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    public void RemoveColumnAt(int index) {
        if (index > 0 && index < this._columns.Count) {
            this.RemoveColumn(this._columns[index]);
        }
    }

    /// <summary>
    /// Removes the specified row.
    /// </summary>
    /// <param name="row">The row.</param>
    public void RemoveRow(LayoutDimension row) {
        this._rows.Remove(row);
        row.DimensionChanged -= this.Row_DimensionChanged;
        this.Rearrange(); // TODO: only arrange rows here.
    }

    /// <summary>
    /// Removes a row at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    public void RemoveRowAt(int index) {
        if (index > 0 && index < this._rows.Count) {
            this.RemoveRow(this._rows[index]);
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
            dimension.ActualLength = 0f;
            dimension.Position = 0f;
        }
    }

    private void Column_DimensionChanged(object? sender, EventArgs e) {
        this.Rearrange(); // TODO: only arrange columns here.
    }

    private void EvaluateColumns(IReadOnlyCollection<ILayoutArrangeable> arrangeables) {
        var totalLength = this._boundingArea.Width;
        for (var i = 0; i < this._columns.Count; i++) {
            var column = this._columns[i];

            if (column.DimensionType == LayoutDimensionType.Auto) {
                var entities = arrangeables.Where(x => x.Column == i).ToArray();
                if (entities.Any()) {
                    column.ActualLength = entities.Max(x => x.BoundingArea.Width);
                    totalLength -= column.ActualLength;
                }
            }
            else if (column.DimensionType == LayoutDimensionType.Units) {
                totalLength -= column.ActualLength;
            }
        }

        EvaluateDimensions(this._columns, this._boundingArea.Minimum.X, totalLength, true);
    }

    private static void EvaluateDimensions(IReadOnlyCollection<LayoutDimension> dimensions, float startingPosition, float totalLength, bool increasingStartingPosition) {
        var totalAmount = dimensions.Where(x => x.DimensionType == LayoutDimensionType.Proportional).Sum(x => x.Amount);
        if (totalAmount > 0f) {
            foreach (var dimension in dimensions) {
                if (dimension.DimensionType == LayoutDimensionType.Proportional) {
                    dimension.ActualLength = totalLength * (dimension.Amount / totalAmount);
                }

                if (!increasingStartingPosition) {
                    startingPosition -= dimension.ActualLength;
                }

                dimension.Position = startingPosition;

                if (increasingStartingPosition) {
                    startingPosition += dimension.ActualLength;
                }
            }
        }
        else {
            ClearDimensions(dimensions);
        }
    }

    private void EvaluateRows(IReadOnlyCollection<ILayoutArrangeable> arrangeables) {
        var totalLength = this._boundingArea.Height;
        for (var i = 0; i < this._rows.Count; i++) {
            var row = this._rows[i];

            if (row.DimensionType == LayoutDimensionType.Auto) {
                var entities = arrangeables.Where(x => x.Row == i).ToArray();
                if (entities.Any()) {
                    row.ActualLength = entities.Max(x => x.BoundingArea.Height);
                    totalLength -= row.ActualLength;
                }
            }
            else if (row.DimensionType == LayoutDimensionType.Units) {
                totalLength -= row.ActualLength;
            }
        }

        EvaluateDimensions(this._rows, this._boundingArea.Maximum.Y, totalLength, false);
    }

    private void Rearrange() {
        if (!this.IsInitialized || this._boundable == null) {
            this._boundingArea = BoundingArea.Empty;
            ClearDimensions(this._rows);
            ClearDimensions(this._columns);
            return;
        }

        this._boundingArea = this._boundable.BoundingArea;

        if (this._boundingArea.IsEmpty) {
            ClearDimensions(this._rows);
            ClearDimensions(this._columns);
        }
        else {
            var arrangeables = this.Children.OfType<ILayoutArrangeable>().ToList();
            this.EvaluateRows(arrangeables);
            this.EvaluateColumns(arrangeables);
            // TODO: reposition children of type ILayoutArrangeable
        }
    }

    private void Row_DimensionChanged(object? sender, EventArgs e) {
        this.Rearrange(); // TODO: only arrange rows here.
    }

    private bool ShouldRearrange(ILayoutArrangeable requester) {
        return this.TryGetRow(requester.Row, out _) || this.TryGetColumn(requester.Column, out _);
    }

    private bool TryGetColumn(int column, out LayoutDimension dimension) {
        if (column < 0 || column >= this._columns.Count) {
            dimension = LayoutDimension.Empty;
            return false;
        }

        dimension = this._columns[column];
        return true;
    }

    private bool TryGetRow(int row, out LayoutDimension dimension) {
        if (row < 0 || row >= this._rows.Count) {
            dimension = LayoutDimension.Empty;
            return false;
        }

        dimension = this._rows[row];
        return true;
    }
}