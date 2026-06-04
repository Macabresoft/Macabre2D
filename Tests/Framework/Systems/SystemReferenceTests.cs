namespace Macabre2D.Tests.Framework.Systems;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabre2D.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class SceneSystemReferenceTests {
    [Test]
    [Category("Unit Tests")]
    public static void Initialize_ShouldSetEntity_WhenEntityIdIsAlreadySet() {
        var game = Substitute.For<IGame>();
        var scene = new Scene();
        var system = scene.AddSystem<SimplePhysicsSystem>();
        var reference = new SceneSystemReference<ISceneSystem>();
        reference.SystemId = system.Id;
        reference.Initialize(game, scene);

        using (new AssertionScope()) {
            reference.System.Should().Be(system);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void SetEntityId_ShouldNotSetEntity_WhenNotInitialized() {
        var scene = new Scene();
        var system = scene.AddSystem<SimplePhysicsSystem>();
        var reference = new SceneSystemReference<ISceneSystem>();
        reference.SystemId = system.Id;

        using (new AssertionScope()) {
            reference.System.Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void SetEntityId_ShouldSetEntity_WhenAlreadyInitialized() {
        var game = Substitute.For<IGame>();
        var scene = new Scene();
        var system = scene.AddSystem<SimplePhysicsSystem>();
        var reference = new SceneSystemReference<ISceneSystem>();
        reference.Initialize(game, scene);
        reference.SystemId = system.Id;

        using (new AssertionScope()) {
            reference.System.Should().Be(system);
        }
    }
}