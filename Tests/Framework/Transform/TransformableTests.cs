namespace Macabresoft.Macabre2D.Tests.Framework.Transform;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class TransformableTests {
    [Test]
    [Category("Unit Tests")]
    public static void PositionShouldChange_When_TransformIsRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.Both);

        using (new AssertionScope()) {
            child.Transform.Position.Should().Be(Vector2.Zero);
            parent.Transform.Position.Should().Be(Vector2.Zero);
            parent.LocalPosition = Vector2.One;
            child.Transform.Position.Should().Be(Vector2.One);
            parent.Transform.Position.Should().Be(Vector2.One);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void PositionShouldChange_When_XIsRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.X);

        using (new AssertionScope()) {
            child.Transform.Position.Should().Be(Vector2.Zero);
            parent.Transform.Position.Should().Be(Vector2.Zero);
            parent.LocalPosition = Vector2.One;
            child.Transform.Position.Should().Be(new Vector2(1f, 0f));
            parent.Transform.Position.Should().Be(Vector2.One);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void PositionShouldChange_When_YIsRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.Y);

        using (new AssertionScope()) {
            child.Transform.Position.Should().Be(Vector2.Zero);
            parent.Transform.Position.Should().Be(Vector2.Zero);
            parent.LocalPosition = Vector2.One;
            child.Transform.Position.Should().Be(new Vector2(0f, 1f));
            parent.Transform.Position.Should().Be(Vector2.One);
        }
    }


    [Test]
    [Category("Unit Tests")]
    public static void PositionShouldNotChange_When_TransformNotRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.None);

        using (new AssertionScope()) {
            child.Transform.Position.Should().Be(Vector2.Zero);
            parent.Transform.Position.Should().Be(Vector2.Zero);
            parent.LocalPosition = Vector2.One;
            child.Transform.Position.Should().Be(Vector2.Zero);
            parent.Transform.Position.Should().Be(Vector2.One);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void ScaleShouldChange_When_TransformIsRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.Both);

        using (new AssertionScope()) {
            child.Transform.Scale.Should().Be(Vector2.One);
            parent.Transform.Scale.Should().Be(Vector2.One);
            parent.LocalScale = Vector2.Zero;
            child.Transform.Scale.Should().Be(Vector2.Zero);
            parent.Transform.Scale.Should().Be(Vector2.Zero);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void ScaleShouldChange_When_XIsRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.X);

        using (new AssertionScope()) {
            child.Transform.Scale.Should().Be(Vector2.One);
            parent.Transform.Scale.Should().Be(Vector2.One);
            parent.LocalScale = Vector2.Zero;
            child.Transform.Scale.Should().Be(new Vector2(0f, 1f));
            parent.Transform.Scale.Should().Be(Vector2.Zero);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void ScaleShouldChange_When_YIsRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.Y);

        using (new AssertionScope()) {
            child.Transform.Scale.Should().Be(Vector2.One);
            parent.Transform.Scale.Should().Be(Vector2.One);
            parent.LocalScale = Vector2.Zero;
            child.Transform.Scale.Should().Be(new Vector2(1f, 0f));
            parent.Transform.Scale.Should().Be(Vector2.Zero);
        }
    }


    [Test]
    [Category("Unit Tests")]
    public static void ScaleShouldNotChange_When_TransformNotRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.None);

        using (new AssertionScope()) {
            child.Transform.Scale.Should().Be(Vector2.One);
            parent.Transform.Scale.Should().Be(Vector2.One);
            parent.LocalScale = Vector2.Zero;
            child.Transform.Scale.Should().Be(Vector2.One);
            parent.Transform.Scale.Should().Be(Vector2.Zero);
        }
    }

    private static (ITransformable Parent, ITransformable Child) GetParentChild(TransformInheritance inheritance) {
        var scene = new Scene();
        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());
        var parent = scene.AddChild<Entity>();
        var child = parent.AddChild<Entity>();
        child.TransformInheritance = inheritance;
        return (parent, child);
    }
}