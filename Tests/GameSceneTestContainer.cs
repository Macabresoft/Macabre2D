namespace Macabresoft.Macabre2D.Tests {

    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;
    using System.Linq;

    internal class GameSceneTestContainer {

        public GameSceneTestContainer(InitializationMode initializationMode) {
            this.Scene = new GameScene() {
                Name = "Test Scene"
            };

            if (initializationMode == InitializationMode.Before) {
                this.Scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());
            }

            this.RenderableEntity = this.Scene.AddChild();
            this.RenderableEntity.Name = $"{nameof(this.RenderableEntity)} / {nameof(this.UpdateableEntity)}";
            this.RenderableComponent = this.RenderableEntity.AddComponent<SpriteRenderComponent>();
            this.UpdateableEntity = this.RenderableEntity;
            this.UpdateableComponent = this.UpdateableEntity.AddComponent<FrameRateComponent>();
            this.CameraEntity = this.UpdateableEntity.AddChild();
            this.CameraEntity.Name = nameof(this.CameraEntity);
            this.CameraComponent = this.CameraEntity.AddComponent<CameraComponent>();
            this.UpdateableAndRenderableEntity = this.Scene.AddChild();
            this.UpdateableAndRenderableEntity.Name = nameof(this.UpdateableAndRenderableEntity);
            this.UpdateableAndRenderableComponent = this.UpdateableAndRenderableEntity.AddComponent<SpriteAnimatorComponent>();

            if (initializationMode == InitializationMode.After) {
                this.Scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());
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