namespace Macabresoft.Macabre2D.Tests.Framework {

    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;
    using NUnit.Framework;
    using System.Linq;

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

        private class GameSceneTestContainer {

            public GameSceneTestContainer(InitializationMode initializationMode) {
                this.Scene = new GameScene();

                if (initializationMode == InitializationMode.Before) {
                    this.Scene.Initialize(Substitute.For<IGame>());
                }

                this.RenderableEntity = this.Scene.AddChild();
                this.RenderableComponent = this.RenderableEntity.AddComponent<SpriteRenderComponent>();
                this.UpdateableEntity = this.RenderableEntity;
                this.UpdateableComponent = this.UpdateableEntity.AddComponent<FrameRateComponent>();
                this.CameraEntity = this.UpdateableEntity.AddChild();
                this.CameraComponent = this.CameraEntity.AddComponent<CameraComponent>();
                this.UpdateableAndRenderableEntity = this.Scene.AddChild();
                this.UpdateableAndRenderableComponent = this.UpdateableAndRenderableEntity.AddComponent<SpriteAnimationComponent>();

                if (initializationMode == InitializationMode.After) {
                    this.Scene.Initialize(Substitute.For<IGame>());
                }
            }

            internal enum InitializationMode {
                Before,
                After,
                None
            }

            public IGameComponent CameraComponent { get; }

            public IGameEntity CameraEntity { get; }

            public IGameComponent RenderableComponent { get; }

            public IGameEntity RenderableEntity { get; }

            public IGameScene Scene { get; }

            public IGameComponent UpdateableAndRenderableComponent { get; }

            public IGameEntity UpdateableAndRenderableEntity { get; }

            public IGameComponent UpdateableComponent { get; }

            public IGameEntity UpdateableEntity { get; }

            internal void AssertExistanceOfComponents(bool shouldExist) {
                using (new AssertionScope()) {
                    this.Scene.RenderableComponents.Any(x => x.Id == this.RenderableComponent.Id).Should().Be(shouldExist);
                    this.Scene.RenderableComponents.Any(x => x.Id == this.UpdateableAndRenderableComponent.Id).Should().Be(shouldExist);
                    this.Scene.UpdateableComponents.Any(x => x.Id == this.UpdateableAndRenderableComponent.Id).Should().Be(shouldExist);
                    this.Scene.UpdateableComponents.Any(x => x.Id == this.UpdateableComponent.Id).Should().Be(shouldExist);
                    this.Scene.CameraComponents.Any(x => x.Id == this.CameraComponent.Id).Should().Be(shouldExist);
                }
            }
        }
    }
}