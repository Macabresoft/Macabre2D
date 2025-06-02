namespace Macabresoft.Macabre2D.Tests.Framework.Hierarchy;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public sealed class EntityReferenceTests {
    [Test]
    [Category("Unit Tests")]
    public static void Initialize_ShouldSetEntity_WhenEntityIdIsAlreadySet() {
        var scene = new Scene();
        var child = scene.AddChild<Entity>();
        var entityReference = new EntityReference<IEntity>();
        entityReference.EntityId = child.Id;
        entityReference.Initialize(scene);

        using (new AssertionScope()) {
            entityReference.Entity.Should().Be(child);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void SetEntityId_ShouldNotSetEntity_WhenNotInitialized() {
        var scene = new Scene();
        var child = scene.AddChild<Entity>();
        var entityReference = new EntityReference<IEntity>();
        entityReference.EntityId = child.Id;

        using (new AssertionScope()) {
            entityReference.Entity.Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void SetEntityId_ShouldSetEntity_WhenAlreadyInitialized() {
        var scene = new Scene();
        var child = scene.AddChild<Entity>();
        var entityReference = new EntityReference<IEntity>();
        entityReference.Initialize(scene);
        entityReference.EntityId = child.Id;

        using (new AssertionScope()) {
            entityReference.Entity.Should().Be(child);
        }
    }
}