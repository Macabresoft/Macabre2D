namespace Macabre2D.Framework.Tests.Physics {

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
            var raycastLayer = Layers.Custom1;

            gameSettings.Layers.Returns(layerSettings);
            GameSettings.Instance = gameSettings;

            if (!layersCompatible) {
                raycastLayer = Layers.Custom2;
            }

            var physicsModule = new SimplePhysicsModule();

            using (var circleBody = new SimpleBodyComponent()) {
                circleBody.SetWorldPosition(Vector2.Zero);
                circleBody.Collider = new CircleCollider(1f);
                circleBody.Layers = Layers.Custom1;
                circleBody.Initialize(scene);

                scene.GetAllComponentsOfType<IPhysicsBody>().Returns(new List<IPhysicsBody>(new[] { circleBody }));

                physicsModule.PreInitialize();
                physicsModule.Initialize(scene);
                physicsModule.PostInitialize();

                var result = physicsModule.TryRaycast(new Vector2(raycastX, raycastY), new Vector2(directionX, directionY), 5f, raycastLayer, out var hit);
                Assert.AreEqual(raycastHit, result);
            }
        }

        [TestCase(0f, 0.6499903f, 0f, -1f, 0.666667f, true, TestName = "Raycast to Line Collider - Collision #1")]
        public static void RaycastLineTest(float raycastX, float raycastY, float directionX, float directionY, float distance, bool raycastHit) {
            var scene = Substitute.For<IScene>();
            var layerSettings = new LayerSettings();
            var gameSettings = Substitute.For<IGameSettings>();
            gameSettings.Layers.Returns(layerSettings);
            GameSettings.Instance = gameSettings;

            var physicsModule = new SimplePhysicsModule();

            using (var lineBody = new SimpleBodyComponent()) {
                lineBody.SetWorldPosition(Vector2.Zero);
                lineBody.Collider = new LineCollider(new Vector2(-1f, 0f), new Vector2(1f, 0f));
                lineBody.Layers = Layers.Default;
                lineBody.Initialize(scene);

                scene.GetAllComponentsOfType<IPhysicsBody>().Returns(new List<IPhysicsBody>(new[] { lineBody }));

                physicsModule.PreInitialize();
                physicsModule.Initialize(scene);
                physicsModule.PostInitialize();

                physicsModule.FixedPreUpdate();

                var result = physicsModule.TryRaycast(new Vector2(raycastX, raycastY), new Vector2(directionX, directionY), distance, Layers.Default, out var hit);
                Assert.AreEqual(raycastHit, result);
                result = physicsModule.TryRaycast(new Vector2(raycastX, raycastY), new Vector2(directionX, directionY), distance, Layers.Default, out hit);
                Assert.AreEqual(raycastHit, result);

                physicsModule.FixedPostUpdate();

                physicsModule.FixedPreUpdate();
                result = physicsModule.TryRaycast(new Vector2(raycastX, raycastY), new Vector2(directionX, directionY), distance, Layers.Default, out hit);
                Assert.AreEqual(raycastHit, result);

                physicsModule.FixedPostUpdate();
            }
        }
    }
}