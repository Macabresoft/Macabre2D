namespace Macabre2D.Tests.Framework;

using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NUnit.Framework;

[TestFixture]
public static class VectorExtensionsTests {
    [Test]
    [Category("Unit Tests")]
    [TestCase(1f, 0f, 0f, 1f, 90f)]
    [TestCase(1f, 0f, -1f, 0f, 180f)]
    [TestCase(1f, 0f, 0f, -1f, 270f)]
    [TestCase(0f, 1f, 1f, 0f, -90f)]
    public static void VectorExtensions_RotateDegreesTest(float x1, float y1, float x2, float y2, float angle) {
        var originalVector = new Vector2(x1, y1);
        var expectedVector = new Vector2(x2, y2);
        var newVector = originalVector.RotateDegrees(angle);
        Assert.That(expectedVector.X, Is.EqualTo(newVector.X).Within(0.01d));
        Assert.That(expectedVector.Y, Is.EqualTo(newVector.Y).Within(0.01d));
    }
}