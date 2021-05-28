namespace Macabresoft.Macabre2D.Tests.Framework.Physics {
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public static class RaycastTests {
        [Test]
        [Category("Unit Tests")]
        [TestCase]
        public static void Raycast_WeirdSpecialCaseTest() {
            // This was a specific edge case real world scenario (numbers copy/pasted from a debug
            // session) that was failing due to floating point precision.
            var ray = new LineSegment(new Vector2(3.125f, 0.616657f), new Vector2(0f, -1f), 0.666667f);

            var body = new SimplePhysicsBody();
            var scene = Substitute.For<IScene>();
            body.Initialize(scene, new Entity());
            body.LocalPosition = new Vector2(0f, -0.5f);
            body.Collider = new RectangleCollider(18f, 1f);
            var result = body.Collider.IsHitBy(ray, out var hit);
            Assert.IsTrue(result);
        }
    }
}