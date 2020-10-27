using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace Macabresoft.Macabre2D.Tests.Framework {

    [TestFixture]
    public sealed class GameSceneTests {

        [Test]
        [Category("Unit Test")]
        public static void GameScene_RegistersComponent_WhenAddedAfterInitialization() {
            var scene = new GameScene();

            scene.Initialize(Substitute.For<IGame>());

            var entity = scene.AddChild();
            var spriteRenderer = entity.AddComponent<SpriteRenderComponent>();
            var frameRateComponent = entity.AddComponent<FrameRateComponent>();
            var camera = entity.AddChild().AddComponent<CameraComponent>();
            var animator = scene.AddChild().AddComponent<SpriteAnimationComponent>();

            using (new AssertionScope()) {
                scene.Game.Should().NotBe(BaseGame.Empty);
                scene.RenderableComponents.Any(x => x.Id == spriteRenderer.Id).Should().BeTrue();
                scene.RenderableComponents.Any(x => x.Id == animator.Id).Should().BeTrue();
                scene.UpdateableComponents.Any(x => x.Id == animator.Id).Should().BeTrue();
                scene.UpdateableComponents.Any(x => x.Id == frameRateComponent.Id).Should().BeTrue();
                scene.CameraComponents.Any(x => x.Id == camera.Id).Should().BeTrue();
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void GameScene_RegistersComponent_WhenInitialized() {
            var scene = new GameScene();
            var entity = scene.AddChild();
            var spriteRenderer = entity.AddComponent<SpriteRenderComponent>();
            var frameRateComponent = entity.AddComponent<FrameRateComponent>();
            var camera = entity.AddChild().AddComponent<CameraComponent>();
            var animator = scene.AddChild().AddComponent<SpriteAnimationComponent>();

            scene.Initialize(Substitute.For<IGame>());

            using (new AssertionScope()) {
                scene.Game.Should().NotBe(BaseGame.Empty);
                scene.RenderableComponents.Any(x => x.Id == spriteRenderer.Id).Should().BeTrue();
                scene.RenderableComponents.Any(x => x.Id == animator.Id).Should().BeTrue();
                scene.UpdateableComponents.Any(x => x.Id == animator.Id).Should().BeTrue();
                scene.UpdateableComponents.Any(x => x.Id == frameRateComponent.Id).Should().BeTrue();
                scene.CameraComponents.Any(x => x.Id == camera.Id).Should().BeTrue();
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void GameScene_UnregistersComponent_WhenRemoved() {
            var scene = new GameScene();
            var entity = scene.AddChild();
            var spriteRenderer = entity.AddComponent<SpriteRenderComponent>();
            var frameRateComponent = entity.AddComponent<FrameRateComponent>();
            var camera = entity.AddComponent<CameraComponent>();
            var animator = entity.AddComponent<SpriteAnimationComponent>();

            scene.Initialize(Substitute.For<IGame>());

            entity.RemoveComponent(spriteRenderer);
            entity.RemoveComponent(frameRateComponent);
            entity.RemoveComponent(camera);
            entity.RemoveComponent(animator);

            using (new AssertionScope()) {
                scene.RenderableComponents.Any(x => x.Id == spriteRenderer.Id).Should().BeFalse();
                scene.RenderableComponents.Any(x => x.Id == animator.Id).Should().BeFalse();
                scene.UpdateableComponents.Any(x => x.Id == animator.Id).Should().BeFalse();
                scene.UpdateableComponents.Any(x => x.Id == frameRateComponent.Id).Should().BeFalse();
                scene.CameraComponents.Any(x => x.Id == camera.Id).Should().BeFalse();
            }
        }
    }
}