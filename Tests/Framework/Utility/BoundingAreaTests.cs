namespace Macabre2D.Tests.Framework.Utility;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public static class BoundingAreaTests {
    [Test]
    [Category("Unit Tests")]
    [TestCase(0f, 0f, 1f, 1f)]
    [TestCase(-1f, -1f, 1f, 1f)]
    [TestCase(100f, 100f, 200f, 200f)]
    [TestCase(-100f, -100f, -200f, -200f)]
    public static void Combine_ShouldReturnCurrent_WhenOtherIsEmpty(float minimumX, float minimumY, float maximumX, float maximumY) {
        var boundingArea = new BoundingArea(minimumX, maximumX, minimumY, maximumY);

        using (new AssertionScope()) {
            boundingArea.Combine(BoundingArea.Empty).Should().Be(boundingArea);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void Combine_ShouldReturnEmpty_WhenBothAreEmpty() {
        using (new AssertionScope()) {
            BoundingArea.Empty.Combine(BoundingArea.Empty).Should().Be(BoundingArea.Empty);
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(0f, 0f, 1f, 1f)]
    [TestCase(-1f, -1f, 1f, 1f)]
    [TestCase(100f, 100f, 200f, 200f)]
    [TestCase(-100f, -100f, -200f, -200f)]
    public static void Combine_ShouldReturnOther_WhenCurrentIsEmpty(float minimumX, float minimumY, float maximumX, float maximumY) {
        var boundingArea = new BoundingArea(minimumX, maximumX, minimumY, maximumY);

        using (new AssertionScope()) {
            BoundingArea.Empty.Combine(boundingArea).Should().Be(boundingArea);
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(0f, 0f, 1f, 1f, 0f, 0f, 1f, 1f, true)]
    [TestCase(-1f, -1f, 1f, 1f, 0f, 0f, 0.5f, 0.5f, true)]
    [TestCase(0f, 0f, 1f, 1f, 0f, 0f, 2f, 1f, false)]
    [TestCase(0f, 0f, 1f, 1f, 0f, 0f, 2f, 2f, false)]
    [TestCase(0f, 0f, 1f, 1f, -1f, -1f, 2f, 2f, false)]
    public static void Contains_ShouldWork(
        float minimumX,
        float minimumY,
        float maximumX,
        float maximumY,
        float otherMinX,
        float otherMinY,
        float otherMaxX,
        float otherMaxY,
        bool shouldContain) {
        var boundingArea = new BoundingArea(minimumX, maximumX, minimumY, maximumY);
        var other = new BoundingArea(otherMinX, otherMaxX, otherMinY, otherMaxY);

        using (new AssertionScope()) {
            boundingArea.Contains(other).Should().Be(shouldContain);
        }
    }
}