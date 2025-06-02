namespace Macabresoft.Macabre2D.Tests.Framework.Hierarchy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public sealed class SceneTests {

    [Test]
    public static void Deinitialize_DeinitializesAllChildren() {
        var game = Substitute.For<IGame>();
        game.State.Returns(x => new GameState());
        var assetManager = Substitute.For<IAssetManager>();
        var scene = new Scene();
        var idToCount = new Dictionary<Guid, int>();

        for (var i = 0; i < 10; i++) {
            var child = new Entity();
            child.Id = Guid.NewGuid();
            scene.AddChild(child);
            
            for (var j = 0; j < 10; j++) {
                var secondaryChild = Substitute.For<IEntity>();
                secondaryChild.Id = Guid.NewGuid();
                
                secondaryChild.Id = Guid.NewGuid();
                idToCount[secondaryChild.Id] = 0;
                secondaryChild.When(x => x.Deinitialize()).Do(
                    x => {
                        idToCount[secondaryChild.Id] += 1;
                    });

                child.AddChild(secondaryChild);
            }
        }
        
        for (var i = 1; i <= 10; i++) {
            scene.Initialize(game, assetManager);
            scene.Deinitialize();

            foreach (var value in idToCount.Values) {
                Assert.That(value == i);
            }
        }
    }
    
    
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
    public static void FindSystem_ShouldFindSystem() {
        var scene = new Scene();
        var system = scene.AddSystem<RenderSystem>();

        using (new AssertionScope()) {
            scene.FindSystem<RenderSystem>(system.Id).Should().Be(system);
            scene.FindSystem<IGameSystem>(system.Id).Should().Be(system);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindSystem_ShouldFindSystem_WhenSerialized() {
        var scene = new Scene();
        var system = scene.AddSystem<SimplePhysicsGameSystem>();

        var serialized = Serializer.Instance.SerializeToString(scene);
        var deserialized = Serializer.Instance.DeserializeFromString<Scene>(serialized);

        using (new AssertionScope()) {
            deserialized.FindSystem<SimplePhysicsGameSystem>(system.Id).Should().BeEquivalentTo(system);
            deserialized.FindSystem<IGameSystem>(system.Id).Should().BeEquivalentTo(system);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindSystem_ShouldNotFindSystem_WhenMismatched() {
        var scene = new Scene();
        var system = scene.AddSystem<SimplePhysicsGameSystem>();

        using (new AssertionScope()) {
            scene.FindSystem<RenderSystem>(system.Id).Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void ReorderSystem_FirstChildPlusOne_ShouldWork() {
        const int NumberOfChildren = 5;
        var scene = new Scene();

        for (var i = 0; i < NumberOfChildren; i++) {
            scene.AddSystem<UpdateSystem>();
        }

        var reordered = scene.Systems.ElementAt(0);
        scene.ReorderSystem(reordered, 1);

        using (new AssertionScope()) {
            scene.Systems.Count.Should().Be(NumberOfChildren);
            scene.Systems.ElementAt(1).Should().Be(reordered);
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
            scene.AddSystem<UpdateSystem>();
        }

        var reordered = scene.Systems.ElementAt(OriginalIndex);
        scene.ReorderSystem(reordered, NewIndex);

        using (new AssertionScope()) {
            scene.Systems.Count.Should().Be(NumberOfChildren);
            scene.Systems.ElementAt(NewIndex).Should().Be(reordered);
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
    public static void Scene_RegistersEntity_WhenUpdated() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.Before);
        test.Scene.Update(FrameTime.Zero, new InputState());
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
        test.Scene.Update(new FrameTime(), new InputState()); // Must update to clear the cache
        test.AssertExistenceOfEntities(false);
    }
}