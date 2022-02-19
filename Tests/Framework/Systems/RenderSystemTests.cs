namespace Macabresoft.Macabre2D.Tests.Framework;

using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class RenderSystemTests {
    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldNotUpdateDisabled() {
        // TODO: need to be able to mock sprite batch
        const int NumberOfRenders = 100;
        var scene = new Scene();
        scene.AddChild<Camera>();
        var renderSystem = scene.AddSystem<RenderSystem>();
        var disabled = scene.AddChild<TestRenderableEntity>();
        disabled.IsEnabled = false;
        disabled.SleepAmountInMilliseconds = 0;

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i < NumberOfRenders; i++) {
                renderSystem.Update(new FrameTime(), new InputState());
                disabled.RenderCount.Should().Be(0);
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdateAllEntities() {
        // TODO: need to be able to mock sprite batch
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

        var game = Substitute.For<IGame>();
        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i < NumberOfRenders; i++) {
                foreach (var entity in entities) {
                    entity.RenderCount.Should().Be(i);
                }

                renderSystem.Update(new FrameTime(), new InputState());
            }
        }
    }
}