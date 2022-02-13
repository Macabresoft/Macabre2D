namespace Macabresoft.Macabre2D.Tests.Framework;

using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public static class EntityTests {
    [Test]
    [Category("Unit Tests")]
    public static void Entity_RegistersChild_WhenMoved() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.After);
        test.Scene.RemoveChild(test.RenderableEntity);
        test.UpdateableAndRenderableEntity.AddChild(test.RenderableEntity);
        test.AssertExistenceOfEntities(true);
    }


    [Test]
    [Category("Unit Tests")]
    public static void Entity_UnregistersChild_WhenRemovedFromSceneTree() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.After);
        test.Scene.RemoveChild(test.RenderableEntity);

        using (new AssertionScope()) {
            test.Scene.RenderableEntities.Any(x => x.Id == test.RenderableEntity.Id).Should().BeFalse();
            test.Scene.UpdateableEntities.Any(x => x.Id == test.UpdateableEntity.Id).Should().BeFalse();
            test.Scene.Cameras.Any(x => x.Id == test.CameraEntity.Id).Should().BeFalse();

            test.Scene.RenderableEntities.Any(x => x.Id == test.UpdateableAndRenderableEntity.Id).Should().BeTrue();
            test.Scene.UpdateableEntities.Any(x => x.Id == test.UpdateableAndRenderableEntity.Id).Should().BeTrue();
        }
    }

    [Test]
    [Repeat(20)]
    [Category("Unit Tests")]
    public static void FindChildId_FindsDeepChild() {
        const int NumberOfChildren = 10;
        var rand = new Random();
        var depth = rand.Next(5, NumberOfChildren);
        var topLevel = rand.Next(0, NumberOfChildren);
        var entity = new Entity();
        var childToFind = new Entity();

        for (var i = 0; i < NumberOfChildren; i++) {
            var child = entity.AddChild<Entity>();
            for (var d = 0; d < NumberOfChildren; d++) {
                if (i == topLevel && d == depth) {
                    child.AddChild(childToFind);
                }
                else {
                    child.AddChild<Entity>();
                }
            }
        }

        using (new AssertionScope()) {
            entity.FindChild(childToFind.Id).Should().Be(childToFind);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void FindChildId_FindsImmediateChild() {
        const int NumberOfChildren = 100;
        var rand = new Random();
        var entity = new Entity();

        for (var i = 0; i < NumberOfChildren; i++) {
            entity.AddChild<Entity>();
        }

        var childToFind = entity.Children.ElementAt(rand.Next(0, NumberOfChildren));

        using (new AssertionScope()) {
            entity.FindChild(childToFind.Id).Should().Be(childToFind);
        }
    }

    [Test]
    [Repeat(20)]
    [Category("Unit Tests")]
    public static void FindChildName_FindsDeepChild() {
        const int NumberOfChildren = 10;
        var rand = new Random();
        var depth = rand.Next(5, NumberOfChildren);
        var topLevel = rand.Next(0, NumberOfChildren);
        var entity = new Entity();
        var childToFind = new Entity();
        childToFind.Name = childToFind.Id.ToString();

        for (var i = 0; i < NumberOfChildren; i++) {
            var child = entity.AddChild<Entity>();
            child.Name = child.Id.ToString();
            for (var d = 0; d < NumberOfChildren; d++) {
                if (i == topLevel && d == depth) {
                    child.AddChild(childToFind);
                }
                else {
                    var depthChild = child.AddChild<Entity>();
                    depthChild.Name = depthChild.Id.ToString();
                }
            }
        }

        using (new AssertionScope()) {
            entity.FindChild(childToFind.Id).Should().Be(childToFind);
        }
    }

    [Test]
    [Repeat(20)]
    [Category("Unit Tests")]
    public static void FindChildName_FindsImmediateChild() {
        const int NumberOfChildren = 100;
        var rand = new Random();
        var entity = new Entity();

        for (var i = 0; i < NumberOfChildren; i++) {
            var child = entity.AddChild<Entity>();
            child.Name = child.Id.ToString();
        }

        var childToFind = entity.Children.ElementAt(rand.Next(0, NumberOfChildren));

        using (new AssertionScope()) {
            entity.FindChild(childToFind.Name).Should().Be(childToFind);
        }
    }
}