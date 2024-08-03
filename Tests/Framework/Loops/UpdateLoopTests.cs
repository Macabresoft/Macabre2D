namespace Macabresoft.Macabre2D.Tests.Framework.Loops; 

using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
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
        updateLoop.LayersToUpdate.Value = (Layers)1;
        var entity = scene.AddChild<TestUpdateableEntity>();
        entity.Layers = (Layers)2;

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
        updateLoop.LayersToUpdate.Value = (Layers)1;
        var entity = scene.AddChild<TestUpdateableEntity>();
        entity.Layers = (Layers)1;

        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

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
        scene.AddLoop<UpdateLoop>();
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

                scene.Update(new FrameTime(), new InputState());
            }
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void Update_ShouldUpdateAllEntities() {
        const int NumberOfChildren = 10;
        var scene = new Scene();
        scene.AddLoop<UpdateLoop>();
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

                scene.Update(new FrameTime(), new InputState());
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

                scene.Update(new FrameTime(), new InputState());
            }
        }
    }

    private const int NumberOfUpdates = 5;
}