namespace Macabresoft.Macabre2D.Tests.Core {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;

    [TestFixture]
    public static class VectorExtensionsTests {

        [Test]
        [Category("Unit Test")]
        [TestCase(1f, 0f, 0f, 1f, 90f)]
        [TestCase(1f, 0f, -1f, 0f, 180f)]
        [TestCase(1f, 0f, 0f, -1f, 270f)]
        [TestCase(0f, 1f, 1f, 0f, -90f)]
        public static void VectorExtensions_RotateDegreesTest(float x1, float y1, float x2, float y2, float angle) {
            var originalVector = new Vector2(x1, y1);
            var expectedVector = new Vector2(x2, y2);
            var newVector = originalVector.RotateDegrees(angle);
            Assert.AreEqual(expectedVector.X, newVector.X, 0.01d);
            Assert.AreEqual(expectedVector.Y, newVector.Y, 0.01d);
        }
    }
}