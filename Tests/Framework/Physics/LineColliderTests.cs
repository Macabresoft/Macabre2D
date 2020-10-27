namespace Macabresoft.Macabre2D.Tests.Framework.Physics {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;

    [TestFixture]
    public static class LineColliderTests {

        [Test]
        [Category("Unit Tests")]
        [TestCase(-1f, 0f, 1f, 0f, 0f, -1f, 0f, 1f, true, 0f, 0f, 0f, -1f, TestName = "Raycast on Line - Ray Hits From Below")]
        [TestCase(-1f, 0f, 1f, 0f, 0f, 1f, 0f, -1f, true, 0f, 0f, 0f, 1f, TestName = "Raycast on Line - Ray Hits From Above")]
        public static void LineCollider_IsHitByTest(
            float x1, float y1, float x2, float y2,
            float rx1, float ry1, float rx2, float ry2,
            bool shouldHit, float ix = 0f, float iy = 0f, float nx = 0f, float ny = 0f) {
            using var lineBody = new DynamicPhysicsBody();
            lineBody.Initialize(new GameEntity());
            lineBody.Collider = new LineCollider(new Vector2(x1, y1), new Vector2(x2, y2));

            var ray = new LineSegment(new Vector2(rx1, ry1), new Vector2(rx2, ry2));
            Assert.AreEqual(shouldHit, lineBody.Collider.IsHitBy(ray, out var hit));

            if (shouldHit) {
                var normal = new Vector2(nx, ny);
                var intersection = new Vector2(ix, iy);

                Assert.AreEqual(normal, hit.Normal);
                Assert.AreEqual(intersection, hit.ContactPoint);
                Assert.AreEqual(lineBody.Collider, hit.Collider);
            }
        }
    }
}