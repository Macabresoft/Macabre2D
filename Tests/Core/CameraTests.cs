namespace Macabresoft.MonoGame.Tests.Core {

    using Macabresoft.MonoGame.Core2D;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;

    [TestFixture]
    public sealed class CameraTests {

        [Test]
        [Category("Unit Test")]
        [TestCase(10f, 2f, 2f, 1f, 9f, 0.2f, 0.2f, TestName = "Camera_ZoomTo_WorldPoint_1")]
        [TestCase(10f, 10f, 10f, 5f, 5f, 5f, 5f, TestName = "Camera_ZoomTo_WorldPoint_2")]
        public static void Camera_ZoomTo_WorldPointTest(
            float startingViewHeight,
            float zoomX,
            float zoomY,
            float zoomAmount,
            float expectedViewHeight,
            float expectedX,
            float expectedY) {
            var camera = new CameraComponent() {
                ViewHeight = startingViewHeight
            };

            camera.Initialize(new GameEntity());

            camera.ZoomTo(new Vector2(zoomX, zoomY), zoomAmount);
            Assert.AreEqual(expectedViewHeight, camera.ViewHeight);
            Assert.AreEqual(expectedX, camera.Entity.LocalPosition.X, 0.001f);
            Assert.AreEqual(expectedY, camera.Entity.LocalPosition.Y, 0.001f);
        }
    }
}