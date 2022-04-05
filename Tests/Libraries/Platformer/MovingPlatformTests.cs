namespace Macabresoft.Macabre2D.Tests.Libraries.Platformer;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Libraries.Platformer;
using Microsoft.Xna.Framework;
using NUnit.Framework;

[TestFixture]
public class MovingPlatformTests {
    [Category("Unit Tests")]
    [Test]
    [TestCase(1f, 1f)]
    [TestCase(0f, 300f)]
    [TestCase(986f, 0f)]
    [TestCase(3.41234123123f, 5.2020445f)]
    public static void Actor_ShouldMove_WhenPlatformMoves(float xMovement, float yMovement) {
        var actor = new PlatformerPlayer();
        var actorPosition = actor.Transform.Position;
        var toMove = new Vector2(xMovement, yMovement);
        var movingPlatform = new MovingPlatform();
        movingPlatform.Attach(actor);
        movingPlatform.Move(toMove);

        using (new AssertionScope()) {
            actor.Transform.Position.Should().Be(actorPosition + toMove);
        }
    }
}