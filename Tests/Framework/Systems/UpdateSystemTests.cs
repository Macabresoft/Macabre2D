namespace Macabresoft.Macabre2D.Tests.Framework;

using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class UpdateSystemTests {
    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldNotUpdateDisabled() {
        const int NumberOfUpdates = 10;
        var scene = new Scene();
        var updateSystem = scene.AddSystem<UpdateSystem>();
        var disabled = scene.AddChild<TestUpdateableEntity>();
        disabled.IsEnabled = false;

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i < NumberOfUpdates; i++) {
                updateSystem.Update(new FrameTime(), new InputState());
                disabled.UpdateCount.Should().Be(0);
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdateAllEntities() {
        const int NumberOfChildren = 10;
        const int NumberOfUpdates = 10;
        var scene = new Scene();
        var updateSystem = scene.AddSystem<UpdateSystem>();
        var entities = new List<TestUpdateableEntity>();

        for (var i = 0; i < NumberOfChildren; i++) {
            entities.Add(scene.AddChild<TestUpdateableEntity>());
        }

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i < NumberOfUpdates; i++) {
                foreach (var entity in entities) {
                    entity.UpdateCount.Should().Be(i);
                }

                updateSystem.Update(new FrameTime(), new InputState());
            }
        }
    }
}