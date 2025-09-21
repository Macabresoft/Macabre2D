namespace Macabresoft.Macabre2D.Tests.Framework.Systems;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public class SystemReferenceTests {
    [Test]
    [Category("Unit Tests")]
    public static void Initialize_ShouldSetEntity_WhenEntityIdIsAlreadySet() {
        var scene = new Scene();
        var system = scene.AddSystem<SimplePhysicsSystem>();
        var reference = new SystemReference<IGameSystem>();
        reference.SystemId = system.Id;
        reference.Initialize(scene);

        using (new AssertionScope()) {
            reference.System.Should().Be(system);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void SetEntityId_ShouldNotSetEntity_WhenNotInitialized() {
        var scene = new Scene();
        var system = scene.AddSystem<SimplePhysicsSystem>();
        var reference = new SystemReference<IGameSystem>();
        reference.SystemId = system.Id;

        using (new AssertionScope()) {
            reference.System.Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void SetEntityId_ShouldSetEntity_WhenAlreadyInitialized() {
        var scene = new Scene();
        var system = scene.AddSystem<SimplePhysicsSystem>();
        var reference = new SystemReference<IGameSystem>();
        reference.Initialize(scene);
        reference.SystemId = system.Id;

        using (new AssertionScope()) {
            reference.System.Should().Be(system);
        }
    }
}