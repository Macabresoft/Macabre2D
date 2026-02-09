namespace Macabre2D.Tests.Framework.Systems; 

using System;
using System.Collections.Generic;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class UpdateSystemTests {
    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldNotUpdate_WhenLayerOverrideDoesNotMatch() {
        var scene = new Scene();
        var updateSystem = scene.AddSystem<UpdateSystem>();
        updateSystem.LayersToUpdate.IsEnabled = true;
        updateSystem.LayersToUpdate.Value = (Layers)1;
        var entity = scene.AddChild<TestUpdateableEntity>();
        entity.Layers = (Layers)2;

        scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                entity.UpdateCount.Should().Be(0);
                updateSystem.Update(new FrameTime(), new InputState());
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldNotUpdateDisabled() {
        var scene = new Scene();
        var updateSystem = scene.AddSystem<UpdateSystem>();
        var disabled = scene.AddChild<TestUpdateableEntity>();
        disabled.IsEnabled = false;
        disabled.SleepAmountInMilliseconds = 0;

        scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                updateSystem.Update(new FrameTime(), new InputState());
                disabled.UpdateCount.Should().Be(0);
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdate_WhenLayerOverrideMatches() {
        var scene = new Scene();
        var updateSystem = scene.AddSystem<UpdateSystem>();
        updateSystem.LayersToUpdate.IsEnabled = true;
        updateSystem.LayersToUpdate.Value = (Layers)1;
        var entity = scene.AddChild<TestUpdateableEntity>();
        entity.Layers = (Layers)1;

        scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                entity.UpdateCount.Should().Be(i);
                scene.Update(new FrameTime(), new InputState());
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdateAll_WhenNoLayerOverride() {
        var scene = new Scene();
        scene.AddSystem<UpdateSystem>();
        var entities = new List<TestUpdateableEntity>();
        var layers = Enum.GetValues<Layers>();
        foreach (var layer in layers) {
            var child = scene.AddChild<TestUpdateableEntity>();
            child.Layers = layer;
            child.IsEnabled = true;
            entities.Add(child);
        }

        scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                foreach (var entity in entities) {
                    entity.UpdateCount.Should().Be(i);
                }

                scene.Update(new FrameTime(), new InputState());
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdateAllEntities() {
        const int NumberOfChildren = 10;
        var scene = new Scene();
        scene.AddSystem<UpdateSystem>();
        var entities = new List<TestUpdateableEntity>();

        for (var i = 0; i < NumberOfChildren; i++) {
            var entity = scene.AddChild<TestUpdateableEntity>();
            entity.SleepAmountInMilliseconds = 0;
            entities.Add(entity);
        }

        scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                foreach (var entity in entities) {
                    entity.UpdateCount.Should().Be(i);
                }

                scene.Update(new FrameTime(), new InputState());
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdateNone_WhenLayerOverrideOfNone() {
        var scene = new Scene();
        var updateSystem = scene.AddSystem<UpdateSystem>();
        updateSystem.LayersToUpdate.IsEnabled = true;
        updateSystem.LayersToUpdate.Value = Layers.None;
        var entities = new List<TestUpdateableEntity>();
        var layers = Enum.GetValues<Layers>();
        foreach (var layer in layers) {
            var child = scene.AddChild<TestUpdateableEntity>();
            child.Layers = layer;
            child.IsEnabled = true;
            entities.Add(child);
        }

        scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                foreach (var entity in entities) {
                    entity.UpdateCount.Should().Be(0);
                }

                scene.Update(new FrameTime(), new InputState());
            }
        }
    }

    private const int NumberOfUpdates = 5;
}