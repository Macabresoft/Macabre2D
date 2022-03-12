namespace Macabresoft.Macabre2D.Tests.Framework.Transform;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class TransformableTests {
    private class TestTransformable : Entity {
        private bool _isTransformRelativeToParent;
        protected override bool IsTransformRelativeToParent => this._isTransformRelativeToParent;

        public void SetIsTransformRelativeToParent(bool value) {
            this._isTransformRelativeToParent = value;
        }
    }

    private static (ITransformable Parent, ITransformable Child) GetParentChild(bool isTransformRelativeToParent) {
        var scene = new Scene();
        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());
        var parent = scene.AddChild<Entity>();
        var child = parent.AddChild<TestTransformable>();
        child.SetIsTransformRelativeToParent(isTransformRelativeToParent);
        return (parent, child);
    }

    [Test]
    [Category("Unit Tests")]
    public static void TransformShouldChange_When_TransformIsRelativeToParent() {
        var (parent, child) = GetParentChild(true);

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
    public static void TransformShouldNotChange_When_TransformNotRelativeToParent() {
        var (parent, child) = GetParentChild(false);

        using (new AssertionScope()) {
            child.Transform.Position.Should().Be(Vector2.Zero);
            parent.Transform.Position.Should().Be(Vector2.Zero);
            parent.LocalPosition = Vector2.One;
            child.Transform.Position.Should().Be(Vector2.Zero);
            parent.Transform.Position.Should().Be(Vector2.One);
        }
    }
}