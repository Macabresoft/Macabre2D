namespace Macabresoft.Macabre2D.Tests.Framework {

    using NUnit.Framework;

    [TestFixture]
    public sealed class GameSceneTests {

        [Test]
        [Category("Unit Test")]
        public static void GameScene_RegistersComponent_WhenAddedAfterInitialization() {
            var test = new GameSceneTestContainer(GameSceneTestContainer.InitializationMode.After);
            test.AssertExistanceOfComponents(true);
        }

        [Test]
        [Category("Unit Test")]
        public static void GameScene_RegistersComponent_WhenInitialized() {
            var test = new GameSceneTestContainer(GameSceneTestContainer.InitializationMode.Before);
            test.AssertExistanceOfComponents(true);
        }

        [Test]
        [Category("Unit Test")]
        public static void GameScene_UnregistersComponent_WhenRemoved() {
            var test = new GameSceneTestContainer(GameSceneTestContainer.InitializationMode.Before);

            test.RenderableEntity.RemoveComponent(test.RenderableComponent);
            test.UpdateableEntity.RemoveComponent(test.UpdateableComponent);
            test.CameraEntity.RemoveComponent(test.CameraComponent);
            test.UpdateableAndRenderableEntity.RemoveComponent(test.UpdateableAndRenderableComponent);

            test.AssertExistanceOfComponents(false);
        }
    }
}