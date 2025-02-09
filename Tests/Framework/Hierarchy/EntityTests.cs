namespace Macabresoft.Macabre2D.Tests.Framework.Hierarchy;

using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class EntityTests {

    [Test]
    public static void Deinitialize_DeinitializesChildren() {
        var parent = new Entity();
        var deinitializeCount = 0;
        var child = Substitute.For<IEntity>();
        child.When(x => x.Deinitialize()).Do(x => deinitializeCount++);

        parent.AddChild(child);

        for (var i = 1; i <= 10; i++) {
            parent.Deinitialize();
            Assert.That(deinitializeCount == i);
        }
    }

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
        test.Scene.Update(FrameTime.Zero, new InputState());

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

    [Test]
    [Category("Unit Tests")]
    public static void GetDescendentsWithContent_ShouldReturnAllChildren_WhenChildrenHaveContent() {
        const int NumberOfChildren = 10;
        const int MaxDepth = 5;

        var parent = new Entity();
        var contentCount = 0;
        var contentId = Guid.NewGuid();

        for (var i = 0; i < NumberOfChildren; i++) {
            var child = parent.AddChild<Entity>();

            child.Name = child.Id.ToString();
            for (var d = 0; d < MaxDepth; d++) {
                if (i + d % 3 == 0) {
                    var spriteRenderer = parent.AddChild<SpriteRenderer>();
                    spriteRenderer.SpriteReference.ContentId = Guid.NewGuid();
                }
                else if (i + d % 2 == 0) {
                    contentCount++;
                    var spriteRenderer = parent.AddChild<SpriteRenderer>();
                    spriteRenderer.SpriteReference.ContentId = contentId;
                }
                else {
                    parent.AddChild<Entity>();
                }
            }
        }

        using (new AssertionScope()) {
            contentCount.Should().BePositive();
            parent.GetDescendentsWithContent(contentId).Count().Should().Be(contentCount);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void GetDescendentsWithContent_ShouldReturnChild_WhenContentUnused() {
        var parent = new Entity();
        var child = parent.AddChild<SpriteRenderer>();
        child.SpriteReference.ContentId = Guid.NewGuid();

        using (new AssertionScope()) {
            parent.GetDescendentsWithContent(Guid.NewGuid()).Count().Should().Be(0);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void GetDescendentsWithContent_ShouldReturnChild_WhenDirectChildHasContent() {
        var parent = new Entity();
        var child = parent.AddChild<SpriteRenderer>();
        child.SpriteReference.ContentId = Guid.NewGuid();

        var descendents = parent.GetDescendentsWithContent(child.SpriteReference.ContentId).ToList();

        using (new AssertionScope()) {
            descendents.Count().Should().Be(1);
            descendents.FirstOrDefault().Should().Be(child);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void GetDescendentsWithContent_ShouldReturnEmpty_WhenNoChildren() {
        var entity = new SpriteRenderer();
        entity.SpriteReference.ContentId = Guid.NewGuid();

        using (new AssertionScope()) {
            entity.GetDescendentsWithContent(entity.SpriteReference.ContentId).Count().Should().Be(0);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void ReorderChild_FirstChildPlusOne_ShouldWork() {
        const int NumberOfChildren = 5;
        var entity = new Entity();

        for (var i = 0; i < NumberOfChildren; i++) {
            entity.AddChild<Entity>();
        }

        var reordered = entity.Children.ElementAt(0);
        entity.ReorderChild(reordered, 1);

        using (new AssertionScope()) {
            entity.Children.Count.Should().Be(NumberOfChildren);
            entity.Children.ElementAt(1).Should().Be(reordered);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void ReorderChild_LastChildMinusOne_ShouldWork() {
        const int NumberOfChildren = 5;
        const int OriginalIndex = NumberOfChildren - 1;
        const int NewIndex = NumberOfChildren - 2;
        var entity = new Entity();

        for (var i = 0; i < NumberOfChildren; i++) {
            entity.AddChild<Entity>();
        }

        var reordered = entity.Children.ElementAt(OriginalIndex);
        entity.ReorderChild(reordered, NewIndex);

        using (new AssertionScope()) {
            entity.Children.Count.Should().Be(NumberOfChildren);
            entity.Children.ElementAt(NewIndex).Should().Be(reordered);
        }
    }
}