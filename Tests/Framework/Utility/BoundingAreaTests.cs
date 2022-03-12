namespace Macabresoft.Macabre2D.Tests.Framework.Utility;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public static class BoundingAreaTests {
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