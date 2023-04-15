namespace Macabresoft.Macabre2D.Tests.Framework.Loops;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public class LoopReferenceTests {
    [Test]
    [Category("Unit Tests")]
    public static void Initialize_ShouldSetEntity_WhenEntityIdIsAlreadySet() {
        var scene = new Scene();
        var loop = scene.AddLoop<SimplePhysicsLoop>();
        var reference = new LoopReference<ILoop>();
        reference.LoopId = loop.Id;
        reference.Initialize(scene);

        using (new AssertionScope()) {
            reference.Loop.Should().Be(loop);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void SetEntityId_ShouldNotSetEntity_WhenNotInitialized() {
        var scene = new Scene();
        var loop = scene.AddLoop<SimplePhysicsLoop>();
        var reference = new LoopReference<ILoop>();
        reference.LoopId = loop.Id;

        using (new AssertionScope()) {
            reference.Loop.Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void SetEntityId_ShouldSetEntity_WhenAlreadyInitialized() {
        var scene = new Scene();
        var loop = scene.AddLoop<SimplePhysicsLoop>();
        var reference = new LoopReference<ILoop>();
        reference.Initialize(scene);
        reference.LoopId = loop.Id;

        using (new AssertionScope()) {
            reference.Loop.Should().Be(loop);
        }
    }
}