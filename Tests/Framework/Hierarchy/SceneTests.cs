namespace Macabresoft.Macabre2D.Tests.Framework.Hierarchy;

using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public sealed class SceneTests {
    [Test]
    [Category("Unit Tests")]
    public static void FindEntity_ShouldFindChild() {
        var scene = new Scene();
        var child = scene.AddChild<Entity>();

        using (new AssertionScope()) {
            scene.FindEntity<IEntity>(child.Id).Should().Be(child);
            scene.FindEntity<Entity>(child.Id).Should().Be(child);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindEntity_ShouldFindChild_WhenSerialized() {
        var scene = new Scene();
        var child = scene.AddChild<Entity>();

        var serialized = Serializer.Instance.SerializeToString(scene);
        var deserialized = Serializer.Instance.DeserializeFromString<Scene>(serialized);

        using (new AssertionScope()) {
            deserialized.FindEntity<IEntity>(child.Id).Should().BeEquivalentTo(child);
            deserialized.FindEntity<Entity>(child.Id).Should().BeEquivalentTo(child);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindEntity_ShouldFindDeepChild() {
        const int Depth = 25;
        var scene = new Scene();
        var child = scene.AddChild<Entity>();

        for (var i = 1; i < Depth; i++) {
            child = child.AddChild<Entity>();
            child.Name = Guid.NewGuid().ToString(); // Make them different for comparison.
        }

        using (new AssertionScope()) {
            scene.FindEntity<IEntity>(child.Id).Should().BeEquivalentTo(child);
            scene.FindEntity<Entity>(child.Id).Should().BeEquivalentTo(child);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindEntity_ShouldFindDeepChild_WhenSerialized() {
        const int Depth = 25;
        var scene = new Scene();
        var child = scene.AddChild<Entity>();

        for (var i = 1; i < Depth; i++) {
            child = child.AddChild<Entity>();
            child.Name = Guid.NewGuid().ToString(); // Make them different for comparison.
        }

        var serialized = Serializer.Instance.SerializeToString(scene);
        var deserialized = Serializer.Instance.DeserializeFromString<Scene>(serialized);

        using (new AssertionScope()) {
            deserialized.FindEntity<IEntity>(child.Id).Should().BeEquivalentTo(child);
            deserialized.FindEntity<Entity>(child.Id).Should().BeEquivalentTo(child);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindEntity_ShouldFindSelf() {
        var scene = new Scene();

        using (new AssertionScope()) {
            scene.FindEntity<IScene>(scene.Id).Should().Be(scene);
            scene.FindEntity<Scene>(scene.Id).Should().Be(scene);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindEntity_ShouldFindSelf_WhenSerialized() {
        var scene = new Scene();
        var serialized = Serializer.Instance.SerializeToString(scene);
        var deserialized = Serializer.Instance.DeserializeFromString<Scene>(serialized);

        using (new AssertionScope()) {
            deserialized.FindEntity<IScene>(scene.Id).Should().Be(deserialized);
            deserialized.FindEntity<Scene>(scene.Id).Should().Be(deserialized);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindEntity_ShouldNotFindChild_WhenInvalidType() {
        var scene = new Scene();
        var child = scene.AddChild<Entity>();

        using (new AssertionScope()) {
            scene.FindEntity<TestRenderableEntity>(child.Id).Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindEntity_ShouldNotFindSelf_WhenInvalidType() {
        var scene = new Scene();

        using (new AssertionScope()) {
            scene.FindEntity<TestRenderableEntity>(scene.Id).Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindLoop_ShouldFindLoop() {
        var scene = new Scene();
        var loop = scene.AddLoop<RenderLoop>();

        using (new AssertionScope()) {
            scene.FindLoop<RenderLoop>(loop.Id).Should().Be(loop);
            scene.FindLoop<ILoop>(loop.Id).Should().Be(loop);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindLoop_ShouldFindLoop_WhenSerialized() {
        var scene = new Scene();
        var loop = scene.AddLoop<SimplePhysicsLoop>();

        var serialized = Serializer.Instance.SerializeToString(scene);
        var deserialized = Serializer.Instance.DeserializeFromString<Scene>(serialized);

        using (new AssertionScope()) {
            deserialized.FindLoop<SimplePhysicsLoop>(loop.Id).Should().BeEquivalentTo(loop);
            deserialized.FindLoop<ILoop>(loop.Id).Should().BeEquivalentTo(loop);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindLoop_ShouldNotFindLoop_WhenMismatched() {
        var scene = new Scene();
        var loop = scene.AddLoop<SimplePhysicsLoop>();

        using (new AssertionScope()) {
            scene.FindLoop<RenderLoop>(loop.Id).Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void ReorderSystem_FirstChildPlusOne_ShouldWork() {
        const int NumberOfChildren = 5;
        var scene = new Scene();

        for (var i = 0; i < NumberOfChildren; i++) {
            scene.AddLoop<UpdateLoop>();
        }

        var reordered = scene.Loops.ElementAt(0);
        scene.ReorderLoop(reordered, 1);

        using (new AssertionScope()) {
            scene.Loops.Count.Should().Be(NumberOfChildren);
            scene.Loops.ElementAt(1).Should().Be(reordered);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void ReorderSystem_LastChildMinusOne_ShouldWork() {
        const int NumberOfChildren = 5;
        const int OriginalIndex = NumberOfChildren - 1;
        const int NewIndex = NumberOfChildren - 2;
        var scene = new Scene();

        for (var i = 0; i < NumberOfChildren; i++) {
            scene.AddLoop<UpdateLoop>();
        }

        var reordered = scene.Loops.ElementAt(OriginalIndex);
        scene.ReorderLoop(reordered, NewIndex);

        using (new AssertionScope()) {
            scene.Loops.Count.Should().Be(NumberOfChildren);
            scene.Loops.ElementAt(NewIndex).Should().Be(reordered);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void Scene_RegistersEntity_WhenAddedAfterInitialization() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.After);
        test.AssertExistenceOfEntities(true);
    }

    [Test]
    [Category("Unit Tests")]
    public static void Scene_RegistersEntity_WhenInitialized() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.Before);
        test.AssertExistenceOfEntities(true);
    }

    [Test]
    [Category("Unit Tests")]
    public static void Scene_UnregistersEntity_WhenRemoved() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.Before);

        test.Scene.RemoveChild(test.RenderableEntity);
        test.Scene.RemoveChild(test.UpdateableEntity);
        test.Scene.RemoveChild(test.CameraEntity);
        test.Scene.RemoveChild(test.UpdateableAndRenderableEntity);
        test.AssertExistenceOfEntities(false);
    }
}