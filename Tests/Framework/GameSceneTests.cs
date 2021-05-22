namespace Macabresoft.Macabre2D.Tests.Framework {
    using NUnit.Framework;

    [TestFixture]
    public sealed class GameSceneTests {
        [Test]
        [Category("Unit Tests")]
        public static void GameScene_RegistersEntity_WhenAddedAfterInitialization() {
            var test = new GameSceneTestContainer(GameSceneTestContainer.InitializationMode.After);
            test.AssertExistenceOfEntities(true);
        }

        [Test]
        [Category("Unit Tests")]
        public static void GameScene_RegistersEntity_WhenInitialized() {
            var test = new GameSceneTestContainer(GameSceneTestContainer.InitializationMode.Before);
            test.AssertExistenceOfEntities(true);
        }

        [Test]
        [Category("Unit Tests")]
        public static void GameScene_UnregistersEntity_WhenRemoved() {
            var test = new GameSceneTestContainer(GameSceneTestContainer.InitializationMode.Before);

            test.Scene.RemoveChild(test.RenderableEntity);
            test.Scene.RemoveChild(test.UpdateableEntity);
            test.Scene.RemoveChild(test.CameraEntity);
            test.Scene.RemoveChild(test.UpdateableAndRenderableEntity);
            test.AssertExistenceOfEntities(false);
        }
    }
}