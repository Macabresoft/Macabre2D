﻿namespace Macabresoft.Macabre2D.Tests.Framework.Layout;

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Framework.Layout;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class LayoutGridTests {
    [Category("Unit Tests")]
    [Test]
    [TestCase(2, 2, 0f, 0f, 10f, 10f, 1, 1, 5f, 0f, 5f, 5f)]
    [TestCase(3, 3, 0f, 0f, 9, 6f, 0, 1, 3f, 4f, 3f, 2f)]
    public static void GetBoundingArea_ShouldReturnCorrectBoundingArea_WithEqualLengths_AndProportionalDimensions(
        int numberOfRows,
        int numberOfColumns,
        float x,
        float y,
        float width,
        float height,
        int row,
        int column,
        float expectedX,
        float expectedY,
        float expectedWidth,
        float expectedHeight) {
        var boundable = new TestingBoundable {
            BoundingArea = new BoundingArea(new Vector2(x, y), width, height)
        };

        var grid = new LayoutGrid();
        grid.Initialize(Substitute.For<IScene>(), boundable);

        for (var i = 0; i < numberOfRows; i++) {
            grid.AddRow().DimensionType = LayoutDimensionType.Proportional;
        }

        for (var i = 0; i < numberOfColumns; i++) {
            grid.AddColumn().DimensionType = LayoutDimensionType.Proportional;
        }

        using (new AssertionScope()) {
            var boundingArea = grid.GetBoundingArea(row, column);
            boundingArea.Minimum.X.Should().Be(expectedX);
            boundingArea.Minimum.Y.Should().Be(expectedY);
            boundingArea.Width.Should().Be(expectedWidth);
            boundingArea.Height.Should().Be(expectedHeight);
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void GetBoundingArea_ShouldReturnEmpty_WhenNoRowsOrColumns() {
        var boundable = new TestingBoundable {
            BoundingArea = new BoundingArea(Vector2.Zero, 10f, 10f)
        };

        var grid = new LayoutGrid();
        grid.Initialize(Substitute.For<IScene>(), boundable);

        using (new AssertionScope()) {
            grid.GetBoundingArea(0, 0).Should().Be(BoundingArea.Empty);
        }
    }

    [Category("Unit Tests")]
    [Test]
    [TestCase(LayoutDimensionType.Auto)]
    [TestCase(LayoutDimensionType.Proportional)]
    [TestCase(LayoutDimensionType.Units)]
    public static void GetBoundingArea_ShouldReturnEmpty_WhenNotInitialized(LayoutDimensionType dimensionType) {
        var grid = new LayoutGrid();
        grid.AddRow().DimensionType = dimensionType;
        grid.AddColumn().DimensionType = dimensionType;

        using (new AssertionScope()) {
            grid.GetBoundingArea(0, 0).Should().Be(BoundingArea.Empty);
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void GetBoundingArea_ShouldReturnFullBoundingArea_WhenOnlyOneRowOneColumn_AndProportionalDimensions() {
        var boundable = new TestingBoundable {
            BoundingArea = new BoundingArea(Vector2.Zero, 10f, 10f)
        };

        var grid = new LayoutGrid();
        grid.Initialize(Substitute.For<IScene>(), boundable);
        grid.AddRow().DimensionType = LayoutDimensionType.Proportional;
        grid.AddColumn().DimensionType = LayoutDimensionType.Proportional;

        using (new AssertionScope()) {
            grid.GetBoundingArea(0, 0).Should().Be(boundable.BoundingArea);
        }
    }

    [Category("Unit Tests")]
    [Test]
    [TestCase(1, 1, 1, 1)]
    [TestCase(2, 5, 3, 10)]
    [TestCase(10, 10, 10, 10)]
    public static void GetBoundingArea_ShouldReturnFullBoundingArea_WithRowSpan_AndProportionalDimensions(int rows, int columns, int rowSpan, int columnSpan) {
        var boundable = new TestingBoundable {
            BoundingArea = new BoundingArea(Vector2.Zero, 10f, 10f)
        };

        var grid = new LayoutGrid();
        grid.Initialize(Substitute.For<IScene>(), boundable);

        for (var i = 0; i < rows; i++) {
            grid.AddRow().DimensionType = LayoutDimensionType.Proportional;
        }

        for (var i = 0; i < columns; i++) {
            grid.AddColumn().DimensionType = LayoutDimensionType.Proportional;
        }

        using (new AssertionScope()) {
            grid.GetBoundingArea(0, 0, rowSpan, columnSpan).Should().Be(boundable.BoundingArea);
        }
    }

    private class TestingBoundable : Entity, IBoundable {
        private BoundingArea _boundingArea;
        public event EventHandler BoundingAreaChanged;

        public BoundingArea BoundingArea {
            get => this._boundingArea;
            set {
                this._boundingArea = value;
                this.BoundingAreaChanged.SafeInvoke(this);
            }
        }
    }
}