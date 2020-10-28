namespace Macabresoft.Macabre2D.Tests.Framework {

    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public static class GameEntityTests {

        [Test]
        [Category("Unit Test")]
        public static void GameEntity_UnregistersComponent_WhenRemovedFromSceneTree() {
            var test = new GameSceneTestContainer(GameSceneTestContainer.InitializationMode.After);

            test.Scene.RemoveChild(test.RenderableEntity);

            using (new AssertionScope()) {
                test.Scene.RenderableComponents.Any(x => x.Id == test.RenderableComponent.Id).Should().BeFalse();
                test.Scene.UpdateableComponents.Any(x => x.Id == test.UpdateableComponent.Id).Should().BeFalse();
                test.Scene.CameraComponents.Any(x => x.Id == test.CameraComponent.Id).Should().BeFalse();

                test.Scene.RenderableComponents.Any(x => x.Id == test.UpdateableAndRenderableComponent.Id).Should().BeTrue();
                test.Scene.UpdateableComponents.Any(x => x.Id == test.UpdateableAndRenderableComponent.Id).Should().BeTrue();
            }
        }
    }
}