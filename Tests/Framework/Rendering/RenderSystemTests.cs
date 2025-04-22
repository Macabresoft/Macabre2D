namespace Macabresoft.Macabre2D.Tests.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class RenderSystemTests {
    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldNotRenderDisabled() {
        const int NumberOfRenders = 100;
        var scene = new Scene();
        scene.AddChild<Camera>();
        var renderSystem = scene.AddSystem<RenderSystem>();
        var disabled = scene.AddChild<TestRenderableEntity>();
        disabled.IsEnabled = false;
        disabled.SleepAmountInMilliseconds = 0;

        var game = GameHelpers.CreateGameSubstitute();
        scene.Initialize(game, Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i < NumberOfRenders; i++) {
                renderSystem.Update(new FrameTime(), new InputState());
                disabled.RenderCount.Should().Be(0);
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldNotRenderExcludedLayers() {
        const int NumberOfRenders = 100;
        var game = Substitute.For<IGame>();
        var project = Substitute.For<IGameProject>();
        game.Project.Returns(project);
        var renderSystem = new RenderSystem();
        var scene = Substitute.For<IScene>();
        scene.Game.Returns(game);
        renderSystem.Initialize(scene);

        var includedRenderable = new TestRenderable { Layers = (Layers)1 };
        var excludedRenderable = new TestRenderable { Layers = (Layers)2 | (Layers)1 };
        scene.RenderableEntities.Returns(new[] { includedRenderable, excludedRenderable });

        var camera = new TestCamera();
        scene.Cameras.Returns(new[] { camera });

        using (new AssertionScope()) {
            for (var i = 0; i < NumberOfRenders; i++) {
                renderSystem.Update(new FrameTime(), new InputState());
                includedRenderable.RenderCount.Should().Be(i + 1);
                excludedRenderable.RenderCount.Should().Be(0);
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldNotRenderInvisible() {
        const int NumberOfRenders = 100;
        var scene = new Scene();
        scene.AddChild<Camera>();
        var renderSystem = scene.AddSystem<RenderSystem>();
        var disabled = scene.AddChild<TestRenderableEntity>();
        disabled.ShouldRender = false;
        disabled.SleepAmountInMilliseconds = 0;

        var game = GameHelpers.CreateGameSubstitute();
        scene.Initialize(game, Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i < NumberOfRenders; i++) {
                renderSystem.Update(new FrameTime(), new InputState());
                disabled.RenderCount.Should().Be(0);
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldRenderAllEntities() {
        const int NumberOfChildren = 10;
        const int NumberOfRenders = 100;
        var scene = new Scene();
        scene.AddChild<Camera>();
        var renderSystem = scene.AddSystem<RenderSystem>();
        var entities = new List<TestRenderableEntity>();

        for (var i = 0; i < NumberOfChildren; i++) {
            var entity = scene.AddChild<TestRenderableEntity>();
            entity.SleepAmountInMilliseconds = 0;
            entities.Add(entity);
        }

        var game = GameHelpers.CreateGameSubstitute();
        scene.Initialize(game, Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i < NumberOfRenders; i++) {
                foreach (var entity in entities) {
                    entity.RenderCount.Should().Be(i);
                }

                renderSystem.Update(new FrameTime(), new InputState());
            }
        }
    }

    private class TestCamera : Entity, ICamera {
        public event EventHandler BoundingAreaChanged;
        public event EventHandler RenderOrderChanged;
        public float ActualViewHeight => this.ViewHeight;
        public BoundingArea BoundingArea { get; } = new(new Vector2(-2, -2), new Vector2(2f, 2f));
        public Layers LayersToExcludeFromRender => (Layers)2;
        public Layers LayersToRender { get; set; } = (Layers)1;
        public OffsetOptions OffsetOptions { get; } = new();
        public BoundingArea SafeArea => this.BoundingArea;
        public float ViewHeight { get; set; } = 4f;
        public float ViewWidth => 4f;

        public Vector2 ConvertPointFromScreenSpaceToWorldSpace(Point point) => Vector2.Zero;

        public void Render(FrameTime frameTime, SpriteBatch spriteBatch, IReadonlyQuadTree<IRenderableEntity> renderTree) {
            var entities = renderTree
                .RetrievePotentialCollisions(this.BoundingArea)
                .Where(x => (x.Layers & this.LayersToExcludeFromRender) == Layers.None && (x.Layers & this.LayersToRender) != Layers.None)
                .ToList();

            foreach (var entity in entities) {
                entity.Render(frameTime, this.BoundingArea);
            }
        }
    }

    private class TestRenderable : Entity, IRenderableEntity {
        public event EventHandler BoundingAreaChanged;

        public event EventHandler ShouldRenderChanged;
        public BoundingArea BoundingArea { get; } = new(-Vector2.One, Vector2.One);

        public int RenderCount { get; private set; }
        public int RenderOrder { get; set; }

        public bool RenderOutOfBounds {
            get => true;
            set { }
        }

        public RenderPriority RenderPriority { get; } = default;
        public bool ShouldRender { get; set; } = true;

        public void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            this.RenderCount++;
        }

        public void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
            this.Render(frameTime, viewBoundingArea);
        }
    }
}