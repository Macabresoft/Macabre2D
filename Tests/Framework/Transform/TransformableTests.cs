namespace Macabresoft.Macabre2D.Tests.Framework.Transform;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
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
            child.WorldPosition.Should().Be(Vector2.Zero);
            parent.WorldPosition.Should().Be(Vector2.Zero);
            parent.LocalPosition = Vector2.One;
            child.WorldPosition.Should().Be(Vector2.One);
            parent.WorldPosition.Should().Be(Vector2.One);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void PositionShouldChange_When_XIsRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.X);

        using (new AssertionScope()) {
            child.WorldPosition.Should().Be(Vector2.Zero);
            parent.WorldPosition.Should().Be(Vector2.Zero);
            parent.LocalPosition = Vector2.One;
            child.WorldPosition.Should().Be(new Vector2(1f, 0f));
            parent.WorldPosition.Should().Be(Vector2.One);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void PositionShouldChange_When_YIsRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.Y);

        using (new AssertionScope()) {
            child.WorldPosition.Should().Be(Vector2.Zero);
            parent.WorldPosition.Should().Be(Vector2.Zero);
            parent.LocalPosition = Vector2.One;
            child.WorldPosition.Should().Be(new Vector2(0f, 1f));
            parent.WorldPosition.Should().Be(Vector2.One);
        }
    }


    [Test]
    [Category("Unit Tests")]
    public static void PositionShouldNotChange_When_TransformNotRelativeToParent() {
        var (parent, child) = GetParentChild(TransformInheritance.None);

        using (new AssertionScope()) {
            child.WorldPosition.Should().Be(Vector2.Zero);
            parent.WorldPosition.Should().Be(Vector2.Zero);
            parent.LocalPosition = Vector2.One;
            child.WorldPosition.Should().Be(Vector2.Zero);
            parent.WorldPosition.Should().Be(Vector2.One);
        }
    }

    private static (ITransformable Parent, ITransformable Child) GetParentChild(TransformInheritance inheritance) {
        var scene = new Scene();
        scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());
        var parent = scene.AddChild<Entity>();
        var child = parent.AddChild<Entity>();
        child.TransformInheritance = inheritance;
        return (parent, child);
    }
}