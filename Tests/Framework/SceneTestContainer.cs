namespace Macabre2D.Tests;

using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabre2D.Framework;
using Macabre2D.Tests.Framework;
using NSubstitute;

internal class SceneTestContainer {
    public SceneTestContainer(InitializationMode initializationMode) {
        this.Scene = new Scene {
            Name = "Test Scene"
        };

        if (initializationMode == InitializationMode.Before) {
            this.Scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());
        }

        this.RenderableEntity = this.Scene.AddChild<SpriteRenderer>();
        this.RenderableEntity.Name = $"{nameof(this.RenderableEntity)} / {nameof(this.UpdateableEntity)}";
        this.UpdateableEntity = this.RenderableEntity.AddChild<FrameRateEntity>();
        this.CameraEntity = this.UpdateableEntity.AddChild<Camera>();
        this.CameraEntity.Name = nameof(this.CameraEntity);
        this.UpdateableAndRenderableEntity = this.Scene.AddChild<BoundingAreaDrawer>();
        this.UpdateableAndRenderableEntity.Name = nameof(this.UpdateableAndRenderableEntity);

        if (initializationMode == InitializationMode.After) {
            this.Scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());
        }
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

    internal enum InitializationMode {
        Before,
        After,
        None
    }
}