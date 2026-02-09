namespace Macabre2D.Tests.Framework.UI.Docking;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabre2D.Framework;
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

    [Test]
    [Category("Unit Tests")]
    [TestCase(1f, 1f)]
    [TestCase(-1f, -1f)]
    [TestCase(1f, 0)]
    [TestCase(0f, 1f)]
    [TestCase(-1f, 0)]
    [TestCase(0f, -1f)]
    [TestCase(-1f, 1f)]
    [TestCase(1f, -1f)]
    [TestCase(100f, 2500f)]
    public static void Margin_Should_ExpandBoundingArea(float marginX, float marginY) {
        var child = new DockablePanel {
            Width = 5f,
            Height = 5f
        };

        var wrapper = CreateWrapper(child);
        wrapper.Margin = new Vector2(marginX, marginY);

        using (new AssertionScope()) {
            wrapper.Margin.Should().NotBe(Vector2.Zero);
            wrapper.BoundingArea.Maximum.Should().Be(child.BoundingArea.Maximum + wrapper.Margin);
            wrapper.BoundingArea.Minimum.Should().Be(child.BoundingArea.Minimum - wrapper.Margin);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void Move_Should_ExpandBoundingArea_WhenChildDoesNotInheritTransform() {
        var childA = new DockablePanel {
            Width = 5f,
            Height = 5f,
            TransformInheritance = TransformInheritance.None
        };

        var childB = new DockablePanel {
            Width = 5f,
            Height = 5f,
            TransformInheritance = TransformInheritance.Both
        };

        var wrapper = CreateWrapper(childA, childB);

        var originalMaximum = wrapper.BoundingArea.Maximum;
        var originalMinimum = wrapper.BoundingArea.Minimum;

        wrapper.Move(new Vector2(-10f));

        using (new AssertionScope()) {
            wrapper.BoundingArea.Maximum.Should().Be(originalMaximum);
            wrapper.BoundingArea.Minimum.X.Should().BeLessThan(originalMinimum.X);
            wrapper.BoundingArea.Minimum.Y.Should().BeLessThan(originalMinimum.Y);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void Move_Should_OnlyResetBoundingAreaOnce() {
        var children = new IEntity[10];
        var childChangeCalls = 0;
        for (var i = 0; i < children.Length; i++) {
            var child = new DockablePanel { Width = i + 1f, Height = i + 1f };
            child.BoundingAreaChanged += (_, _) => childChangeCalls++;
            children[i] = child;
        }

        var wrapper = CreateWrapper(children);
        var wrapperChangeCalls = 0;
        childChangeCalls = 0; // Reset it, because initialize will increase it.
        wrapper.BoundingAreaChanged += (_, _) => wrapperChangeCalls++;
        wrapper.Move(Vector2.One);

        using (new AssertionScope()) {
            childChangeCalls.Should().Be(children.Length);
            wrapperChangeCalls.Should().Be(1);
        }
    }

    private static DockableWrapper CreateWrapper(params IEntity[] children) {
        var scene = Substitute.For<IScene>();
        var project = Substitute.For<IGameProject>();
        var game = Substitute.For<IGame>();
        scene.Game.Returns(game);
        project.PixelsPerUnit = 1;
        project.UnitsPerPixel.Returns(1f);
        scene.Project.Returns(project);
        game.Project.Returns(project);
        var wrapper = new DockableWrapper();

        foreach (var child in children) {
            wrapper.AddChild(child);
        }

        wrapper.Initialize(scene, scene);
        return wrapper;
    }
}