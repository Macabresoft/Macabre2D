namespace Macabre2D.Tests.Physics {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NSubstitute;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public static class SimplePhysicsModuleTests {

        [Test]
        [Category("Unit Tests")]
        [TestCase(-2f, 0f, 1f, 0f, true, true, TestName = "Raycast to Circle Collider - Collision")]
        [TestCase(-2f, 0f, 1f, 0f, false, false, TestName = "Raycast to Circle Collider - Different Layers")]
        [TestCase(-2f, 2f, 1f, 0f, true, false, TestName = "Raycast to Circle Collider - No Collision")]
        [TestCase(-2f, 2f, 1f, 0f, false, false, TestName = "Raycast to Circle Collider - No Collision / Different Layers")]
        public static void RaycastCircleTest(float raycastX, float raycastY, float directionX, float directionY, bool layersCompatible, bool raycastHit) {
            var scene = Substitute.For<IScene>();
            var layerSettings = new LayerSettings();
            var gameSettings = Substitute.For<IGameSettings>();
            gameSettings.Layers.Returns(layerSettings);
            GameSettings.Instance = gameSettings;

            if (!layersCompatible) {
                layerSettings.ToggleShouldCollide(Layers.Layer01, Layers.Layer02);
                layerSettings.ToggleShouldCollide(Layers.Layer02, Layers.Layer01);
            }

            var physicsModule = new SimplePhysicsModule();

            using (var circleBody = new SimpleBodyComponent()) {
                circleBody.SetWorldPosition(Vector2.Zero);
                circleBody.Collider = new CircleCollider(1f);
                circleBody.Layers = Layers.Layer02;
                circleBody.Initialize(scene);

                scene.GetAllComponentsOfType<IPhysicsBody>().Returns(new List<IPhysicsBody>(new[] { circleBody }));

                physicsModule.PreInitialize();
                physicsModule.Initialize(scene);
                physicsModule.PostInitialize();

                var result = physicsModule.TryRaycast(new Vector2(raycastX, raycastY), new Vector2(directionX, directionY), 5f, Layers.Layer01, out var hit);
                Assert.AreEqual(raycastHit, result);
            }
        }
    }
}