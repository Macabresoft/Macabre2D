namespace Macabresoft.Macabre2D.Tests.Framework.Layout;

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
    public static void GetBoundingArea_ShouldReturnEmpty_WhenNotInitialized() {
        var grid = new LayoutGrid();
        grid.AddRow();
        grid.AddColumn();

        using (new AssertionScope()) {
            grid.GetBoundingArea(0, 0).Should().Be(BoundingArea.Empty);
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
    public static void GetBoundingArea_ShouldReturnFullBoundingArea_WhenOnlyOneRowOneColumn() {
        var boundable = new TestingBoundable {
            BoundingArea = new BoundingArea(Vector2.Zero, 10f, 10f)
        };

        var grid = new LayoutGrid();
        grid.Initialize(Substitute.For<IScene>(), boundable);
        grid.AddRow();
        grid.AddColumn();

        using (new AssertionScope()) {
            grid.GetBoundingArea(0, 0).Should().Be(boundable.BoundingArea);
        }
    }

    [Category("Unit Tests")]
    [Test]
    [TestCase(2, 2, 0f, 0f, 10f, 10f, 1, 1, 5f, 0f, 5f, 5f)]
    [TestCase(3, 3, 0f, 0f, 9, 6f, 0, 1, 3f, 4f, 3f, 2f)]
    public static void GetBoundingArea_ShouldReturnCorrectBoundingArea_WithEqualLengths(
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
            grid.AddRow();
        }
        
        for (var i = 0; i < numberOfColumns; i++) {
            grid.AddColumn();
        }

        using (new AssertionScope()) {
            var boundingArea = grid.GetBoundingArea(row, column);
            boundingArea.Minimum.X.Should().Be(expectedX);
            boundingArea.Minimum.Y.Should().Be(expectedY);
            boundingArea.Width.Should().Be(expectedWidth);
            boundingArea.Height.Should().Be(expectedHeight);
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