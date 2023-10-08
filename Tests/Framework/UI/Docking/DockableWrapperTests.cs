namespace Macabresoft.Macabre2D.Tests.Framework.UI.Docking;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class DockableWrapperTests {
    [Test]
    [Category("Unit Tests")]
    public static void Initialize_Should_CombineBoundingAreas() {
        var boundingAreaA = new TestableBoundable {
            BoundingArea = new BoundingArea(Vector2.Zero, new Vector2(10f))
        };

        var boundingAreaB = new TestableBoundable {
            BoundingArea = new BoundingArea(new Vector2(-5f), new Vector2(1f))
        };

        var wrapper = CreateWrapper(boundingAreaA, boundingAreaB);

        using (new AssertionScope()) {
            wrapper.BoundingArea.Maximum.Should().Be(boundingAreaA.BoundingArea.Maximum);
            wrapper.BoundingArea.Minimum.Should().Be(boundingAreaB.BoundingArea.Minimum);
        }
    }

    private static DockableWrapper CreateWrapper(params IEntity[] children) {
        var scene = Substitute.For<IScene>();
        var wrapper = new DockableWrapper();

        foreach (var child in children) {
            wrapper.AddChild(child);
        }

        wrapper.Initialize(scene, scene);
        return wrapper;
    }
}