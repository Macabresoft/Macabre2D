namespace Macabresoft.Macabre2D.Tests.Framework;

using NUnit.Framework;

[TestFixture]
public sealed class SceneTests {
    [Test]
    [Category("Unit Tests")]
    public static void Scene_RegistersEntity_WhenAddedAfterInitialization() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.After);
        test.AssertExistenceOfEntities(true);
    }

    [Test]
    [Category("Unit Tests")]
    public static void Scene_RegistersEntity_WhenInitialized() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.Before);
        test.AssertExistenceOfEntities(true);
    }

    [Test]
    [Category("Unit Tests")]
    public static void Scene_UnregistersEntity_WhenRemoved() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.Before);

        test.Scene.RemoveChild(test.RenderableEntity);
        test.Scene.RemoveChild(test.UpdateableEntity);
        test.Scene.RemoveChild(test.CameraEntity);
        test.Scene.RemoveChild(test.UpdateableAndRenderableEntity);
        test.AssertExistenceOfEntities(false);
    }
}