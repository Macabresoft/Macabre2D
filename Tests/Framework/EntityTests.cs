namespace Macabresoft.Macabre2D.Tests.Framework {
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    [TestFixture]
    public static class EntityTests {
        [Test]
        [Category("Unit Tests")]
        public static void Entity_RegistersChild_WhenMoved() {
            var test = new SceneTestContainer(SceneTestContainer.InitializationMode.After);
            test.Scene.RemoveChild(test.RenderableEntity);
            test.UpdateableAndRenderableEntity.AddChild(test.RenderableEntity);
            test.AssertExistenceOfEntities(true);
        }

        [Test]
        [Category("Unit Tests")]
        public static void Entity_UnregistersChild_WhenRemovedFromSceneTree() {
            var test = new SceneTestContainer(SceneTestContainer.InitializationMode.After);
            test.Scene.RemoveChild(test.RenderableEntity);

            using (new AssertionScope()) {
                test.Scene.RenderableEntities.Any(x => x.Id == test.RenderableEntity.Id).Should().BeFalse();
                test.Scene.UpdateableEntities.Any(x => x.Id == test.UpdateableEntity.Id).Should().BeFalse();
                test.Scene.Cameras.Any(x => x.Id == test.CameraEntity.Id).Should().BeFalse();

                test.Scene.RenderableEntities.Any(x => x.Id == test.UpdateableAndRenderableEntity.Id).Should().BeTrue();
                test.Scene.UpdateableEntities.Any(x => x.Id == test.UpdateableAndRenderableEntity.Id).Should().BeTrue();
            }
        }
    }
}