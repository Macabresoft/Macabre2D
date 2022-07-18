namespace Macabresoft.Macabre2D.Tests.Framework;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class LineStripColliderTests {
    [Test]
    [Category("Unit Tests")]
    public static void TryGetLineSegmentContainingPoint_ShouldReturnTrue_ForAllVertices() {
        var lineStripCollider = new LineStripCollider(new[] {
            Vector2.Zero,
            Vector2.One,
            new(5f, 2f),
            new(3f, -5f)
        });

        var physicsBody = Substitute.For<IPhysicsBody>();
        physicsBody.GetWorldTransform(Arg.Any<Vector2>()).Returns(x => new Macabre2D.Framework.Transform((Vector2)x[0], Vector2.One));
        lineStripCollider.Initialize(physicsBody);

        using (new AssertionScope()) {
            lineStripCollider.LineSegments.Should().NotBeEmpty();

            foreach (var lineSegment in lineStripCollider.LineSegments) {
                lineStripCollider.TryGetLineSegmentContainingPoint(lineSegment.Start, out _).Should().BeTrue();
                lineStripCollider.TryGetLineSegmentContainingPoint(lineSegment.End, out _).Should().BeTrue();
            }
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void TryGetLineSegmentContainingPoint_ShouldReturnTrue_WhenPointIsOnLine() {
        var lineStripCollider = new LineStripCollider(new[] {
            Vector2.Zero,
            Vector2.One,
            new(5f, 2f),
            new(3f, -5f)
        });

        var physicsBody = Substitute.For<IPhysicsBody>();
        physicsBody.GetWorldTransform(Arg.Any<Vector2>()).Returns(x => new Macabre2D.Framework.Transform((Vector2)x[0], Vector2.One));
        lineStripCollider.Initialize(physicsBody);

        using (new AssertionScope()) {
            lineStripCollider.LineSegments.Should().NotBeEmpty();

            foreach (var lineSegment in lineStripCollider.LineSegments) {
                var testPoint = 0.5f * (lineSegment.Start + lineSegment.End);
                lineStripCollider.TryGetLineSegmentContainingPoint(testPoint, out var foundLineSegment).Should().BeTrue();
                foundLineSegment.Should().Be(lineSegment);
            }
        }
    }
}