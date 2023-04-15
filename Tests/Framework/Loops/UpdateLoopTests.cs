namespace Macabresoft.Macabre2D.Tests.Framework.Loops; 

using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class UpdateLoopTests {
    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldNotUpdate_WhenLayerOverrideDoesNotMatch() {
        var scene = new Scene();
        var updateLoop = scene.AddLoop<UpdateLoop>();
        updateLoop.LayersToUpdate.IsEnabled = true;
        updateLoop.LayersToUpdate.Value = Layers.Layer06;
        var entity = scene.AddChild<TestUpdateableEntity>();
        entity.Layers = Layers.Layer09;

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                entity.UpdateCount.Should().Be(0);
                updateLoop.Update(new FrameTime(), new InputState());
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldNotUpdateDisabled() {
        var scene = new Scene();
        var updateLoop = scene.AddLoop<UpdateLoop>();
        var disabled = scene.AddChild<TestUpdateableEntity>();
        disabled.IsEnabled = false;
        disabled.SleepAmountInMilliseconds = 0;

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                updateLoop.Update(new FrameTime(), new InputState());
                disabled.UpdateCount.Should().Be(0);
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdate_WhenLayerOverrideMatches() {
        var scene = new Scene();
        var updateLoop = scene.AddLoop<UpdateLoop>();
        updateLoop.LayersToUpdate.IsEnabled = true;
        updateLoop.LayersToUpdate.Value = Layers.Layer03;
        var entity = scene.AddChild<TestUpdateableEntity>();
        entity.Layers = Layers.Layer03;

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                entity.UpdateCount.Should().Be(i);
                updateLoop.Update(new FrameTime(), new InputState());
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdateAll_WhenNoLayerOverride() {
        var scene = new Scene();
        var updateLoop = scene.AddLoop<UpdateLoop>();
        var entities = new List<TestUpdateableEntity>();
        var layers = Enum.GetValues<Layers>();
        foreach (var layer in layers) {
            var child = scene.AddChild<TestUpdateableEntity>();
            child.Layers = layer;
            child.IsEnabled = true;
            entities.Add(child);
        }

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                foreach (var entity in entities) {
                    entity.UpdateCount.Should().Be(i);
                }

                updateLoop.Update(new FrameTime(), new InputState());
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdateAllEntities() {
        const int NumberOfChildren = 10;
        var scene = new Scene();
        var updateLoop = scene.AddLoop<UpdateLoop>();
        var entities = new List<TestUpdateableEntity>();

        for (var i = 0; i < NumberOfChildren; i++) {
            var entity = scene.AddChild<TestUpdateableEntity>();
            entity.SleepAmountInMilliseconds = 0;
            entities.Add(entity);
        }

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                foreach (var entity in entities) {
                    entity.UpdateCount.Should().Be(i);
                }

                updateLoop.Update(new FrameTime(), new InputState());
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdateNone_WhenLayerOverrideOfNone() {
        var scene = new Scene();
        var updateLoop = scene.AddLoop<UpdateLoop>();
        updateLoop.LayersToUpdate.IsEnabled = true;
        updateLoop.LayersToUpdate.Value = Layers.None;
        var entities = new List<TestUpdateableEntity>();
        var layers = Enum.GetValues<Layers>();
        foreach (var layer in layers) {
            var child = scene.AddChild<TestUpdateableEntity>();
            child.Layers = layer;
            child.IsEnabled = true;
            entities.Add(child);
        }

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        using (new AssertionScope()) {
            for (var i = 0; i <= NumberOfUpdates; i++) {
                foreach (var entity in entities) {
                    entity.UpdateCount.Should().Be(0);
                }

                updateLoop.Update(new FrameTime(), new InputState());
            }
        }
    }

    private const int NumberOfUpdates = 5;
}