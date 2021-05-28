namespace Macabresoft.Macabre2D.Tests {

    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;
    using System.Linq;

    internal class GameSceneTestContainer {

        public GameSceneTestContainer(InitializationMode initializationMode) {
            this.Scene = new Scene() {
                Name = "Test Scene"
            };

            if (initializationMode == InitializationMode.Before) {
                this.Scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());
            }

            this.RenderableEntity = this.Scene.AddChild<SpriteRenderer>();
            this.RenderableEntity.Name = $"{nameof(this.RenderableEntity)} / {nameof(this.UpdateableEntity)}";
            this.UpdateableEntity = this.RenderableEntity.AddChild<FrameRateEntity>();
            this.CameraEntity = this.UpdateableEntity.AddChild<Camera>();
            this.CameraEntity.Name = nameof(this.CameraEntity);
            this.UpdateableAndRenderableEntity = this.Scene.AddChild<SpriteAnimator>();
            this.UpdateableAndRenderableEntity.Name = nameof(this.UpdateableAndRenderableEntity);

            if (initializationMode == InitializationMode.After) {
                this.Scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());
            }
        }

        internal enum InitializationMode {
            Before,
            After,
            None
        }
        
        public IEntity CameraEntity { get; }

        public IEntity RenderableEntity { get; }

        public IScene Scene { get; }

        public IEntity UpdateableAndRenderableEntity { get; }
        
        public IEntity UpdateableEntity { get; }

        internal void AssertExistenceOfEntities(bool shouldExist) {
            using (new AssertionScope()) {
                this.Scene.RenderableEntities.Any(x => x.Id == this.RenderableEntity.Id).Should().Be(shouldExist);
                this.Scene.RenderableEntities.Any(x => x.Id == this.UpdateableAndRenderableEntity.Id).Should().Be(shouldExist);
                this.Scene.UpdateableEntities.Any(x => x.Id == this.UpdateableAndRenderableEntity.Id).Should().Be(shouldExist);
                this.Scene.UpdateableEntities.Any(x => x.Id == this.UpdateableEntity.Id).Should().Be(shouldExist);
                this.Scene.Cameras.Any(x => x.Id == this.CameraEntity.Id).Should().Be(shouldExist);
            }
        }
    }
}