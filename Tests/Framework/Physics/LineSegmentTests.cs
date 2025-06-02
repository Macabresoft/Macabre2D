namespace Macabresoft.Macabre2D.Tests.Framework.Physics;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NUnit.Framework;

[TestFixture]
public static class LineSegmentTests {
    [Test]
    [Category("Unit Tests")]
    public static void Contains_ShouldReturnFalse_WhenEmptyLineSegment() {
        var lineSegment = LineSegment.Empty;
        var point = Vector2.Zero;

        using (new AssertionScope()) {
            lineSegment.Contains(point).Should().BeFalse();
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(0f, 0f, 1f, 0f, 0.5f, 1f)]
    [TestCase(0f, 0f, 1f, 1f, 2f, 2f)]
    [TestCase(0f, 0f, 1f, 0f, 0f, 1f)]
    [TestCase(0f, 0f, 1f, 0f, 1f, 1f)]
    [TestCase(0f, 0f, 1f, 0f, 0.5f, 0.1f)]
    [TestCase(0f, 0f, 1f, 1f, 0.51f, 0.5f)]
    public static void Contains_ShouldReturnFalse_WhenNotOnSegment(
        float startX,
        float startY,
        float endX,
        float endY,
        float testX,
        float testY) {
        var lineSegment = new LineSegment(new Vector2(startX, startY), new Vector2(endX, endY));
        var testPoint = new Vector2(testX, testY);

        using (new AssertionScope()) {
            lineSegment.Contains(testPoint).Should().BeFalse();
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(0f, 0f, 1f, 0f, 0.5f, 0f)]
    [TestCase(0f, 0f, 1f, 1f, 0.5f, 0.5f)]
    [TestCase(0f, 0f, 1f, 0f, 0f, 0f)]
    [TestCase(0f, 0f, 1f, 0f, 1f, 0f)]
    public static void Contains_ShouldReturnTrue_WhenOnSegment(
        float startX,
        float startY,
        float endX,
        float endY,
        float testX,
        float testY) {
        var lineSegment = new LineSegment(new Vector2(startX, startY), new Vector2(endX, endY));
        var testPoint = new Vector2(testX, testY);

        using (new AssertionScope()) {
            lineSegment.Contains(testPoint).Should().BeTrue();
        }
    }
}