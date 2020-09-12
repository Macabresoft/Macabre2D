namespace Macabresoft.MonoGame.Tests.Core.Physics {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;

    [TestFixture]
    public static class PolygonColliderTests {

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 1f, 0f, 0f, 0.5f, false, TestName = "Quad Contains Circle - Touches Edges")]
        [TestCase(0f, 0f, 1f, 1f, 0f, 0f, 0.25f, true, TestName = "Quad Contains Circle - Inside at Center")]
        [TestCase(0f, 0f, 1f, 1f, 0.25f, -0.25f, 0.1f, true, TestName = "Quad Contains Circle - Inside Offset")]
        [TestCase(0f, 0f, 1f, 1f, 0.25f, -0.25f, 2f, false, TestName = "Quad Contains Circle - Inside But Does Not Contain")]
        [TestCase(0f, 0f, 1f, 1f, 3f, -2f, 0.5f, false, TestName = "Quad Contains Circle - Outside")]
        [TestCase(0f, 0f, 1f, 1f, 1, 1f, 0.5f, false, TestName = "Quad Contains Circle - Edges Touch")]
        public static void PolygonCollider_ContainsCircle(float x1, float y1, float w1, float h1, float x2, float y2, float r2, bool shouldContain) {
            using (var quadBody = new DynamicPhysicsBody())
            using (var circleBody = new DynamicPhysicsBody()) {
                circleBody.Initialize(new GameEntity());
                quadBody.Initialize(new GameEntity());

                quadBody.Entity.SetWorldPosition(new Vector2(x1, y1));
                quadBody.Collider = PolygonCollider.CreateRectangle(w1, w1);

                circleBody.Entity.SetWorldPosition(new Vector2(x2, y2));
                circleBody.Collider = new CircleCollider(r2);

                Assert.AreEqual(shouldContain, quadBody.Collider.Contains(circleBody.Collider));
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 1f, 0f, 0f, true, TestName = "Quad Contains Point - Point is Center")]
        [TestCase(0f, 0f, 1f, 1f, 0f, 0.5f, true, TestName = "Quad Contains Point - Point is on Edge")]
        [TestCase(0f, 0f, 1f, 1f, 0.25f, 0.25f, true, TestName = "Quad Contains Point - Point is Inside")]
        [TestCase(0f, 0f, 1f, 1f, 1f, 1f, false, TestName = "Quad Contains Point - Point is Outside")]
        [TestCase(0f, 0f, 1f, 1f, 100f, 100f, false, TestName = "Quad Contains Point - Point is Way Outside")]
        public static void PolygonCollider_ContainsPoint(float x1, float y1, float w, float h, float x2, float y2, bool shouldContain) {
            using (var quadBody = new DynamicPhysicsBody()) {
                quadBody.Initialize(new GameEntity());
                quadBody.Entity.SetWorldPosition(new Vector2(x1, y1));
                quadBody.Collider = PolygonCollider.CreateRectangle(w, h);

                Assert.AreEqual(shouldContain, quadBody.Collider.Contains(new Vector2(x2, y2)));
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 1f, 0f, 0f, 1f, 1f, false, TestName = "Quad Contains Quad - Same Quad")]
        [TestCase(0f, 0f, 1f, 1f, 0f, 0f, 0.5f, 0.5f, true, TestName = "Quad Contains Quad - Inside at Center")]
        [TestCase(0f, 0f, 1f, 1f, 0.25f, -0.25f, 0.25f, 0.5f, true, TestName = "Quad Contains Quad - Inside Offset")]
        [TestCase(0f, 0f, 1f, 1f, 0.25f, -0.25f, 3f, 2f, false, TestName = "Quad Contains Quad - Inside But Does Not Contain")]
        [TestCase(0f, 0f, 1f, 1f, 3f, -2f, 1f, 1f, false, TestName = "Quad Contains Quad - Outside")]
        [TestCase(0f, 0f, 1f, 1f, 1, 1f, 1f, 1f, false, TestName = "Quad Contains Quad - Edges Touch")]
        public static void PolygonCollider_ContainsPolygon(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2, bool shouldContain) {
            using (var quadBody1 = new DynamicPhysicsBody())
            using (var quadBody2 = new DynamicPhysicsBody()) {
                quadBody1.Initialize(new GameEntity());
                quadBody2.Initialize(new GameEntity());

                quadBody1.Entity.SetWorldPosition(new Vector2(x1, y1));
                quadBody1.Collider = PolygonCollider.CreateRectangle(w1, w1);

                quadBody2.Entity.SetWorldPosition(new Vector2(x2, y2));
                quadBody2.Collider = PolygonCollider.CreateRectangle(w2, w2);

                Assert.AreEqual(shouldContain, quadBody1.Collider.Contains(quadBody2.Collider));
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 1f, 0f, 0f, 0.5f, 0.5f, true, TestName = "Quad to Quad Collision - First Quad Contains Second Quad")]
        [TestCase(0f, 0f, 0.5f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Quad to Quad Collision - Second Quad Contains First Quad")]
        [TestCase(0.5f, 0f, 0.5f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Quad to Quad Collision - Left Collision")]
        [TestCase(-0.5f, 0f, 0.5f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Quad to Quad Collision - Right Collision")]
        [TestCase(3f, 3f, 1f, 1f, 0f, 0f, 1f, 1f, false, TestName = "Quad to Quad Collision - No Collision")]
        [TestCase(0f, -130f, 100f, 100f, 0f, -200f, 500f, 50f, true, TestName = "Quad to Quad Collision - Physics Test Failure")] // I think this should be failing?
        public static void PolygonCollider_QuadCollidesWithQuadTest(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2, bool collisionOccured) {
            using (var quadBody1 = new DynamicPhysicsBody())
            using (var quadBody2 = new DynamicPhysicsBody()) {
                quadBody1.Initialize(new GameEntity());
                quadBody2.Initialize(new GameEntity());

                quadBody1.Entity.SetWorldPosition(new Vector2(x1, y1));
                quadBody1.Collider = PolygonCollider.CreateRectangle(w1, h1);

                quadBody2.Entity.SetWorldPosition(new Vector2(x2, y2));
                quadBody2.Collider = PolygonCollider.CreateRectangle(w2, h2);

                Assert.AreEqual(collisionOccured, quadBody1.Collider.CollidesWith(quadBody2.Collider, out var collision1));
                Assert.AreEqual(collisionOccured, quadBody2.Collider.CollidesWith(quadBody1.Collider, out var collision2));

                if (collisionOccured) {
                    Assert.AreEqual(collision1.MinimumTranslationVector.Length(), collision2.MinimumTranslationVector.Length(), 0.0001f);
                    Assert.AreEqual(collision1.FirstCollider, collision2.SecondCollider);
                    Assert.AreEqual(collision1.SecondCollider, collision2.FirstCollider);
                    Assert.AreEqual(collision1.FirstContainsSecond, collision2.SecondContainsFirst);
                    Assert.AreEqual(collision1.SecondContainsFirst, collision2.FirstContainsSecond);

                    var originalPosition = quadBody1.Entity.Transform.Position;
                    quadBody1.Entity.SetWorldPosition(originalPosition + collision1.MinimumTranslationVector);
                    Assert.False(quadBody1.Collider.CollidesWith(quadBody2.Collider, out collision1));
                    quadBody1.Entity.SetWorldPosition(originalPosition);

                    quadBody2.Entity.SetWorldPosition(quadBody2.Entity.Transform.Position + collision2.MinimumTranslationVector);
                    Assert.False(quadBody2.Collider.CollidesWith(quadBody1.Collider, out collision2));
                }
                else {
                    Assert.Null(collision1);
                    Assert.Null(collision2);
                }
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 2f, 2f, 0f, -2f, 0f, 1f, 5f, true, 0f, -1f, 0f, -1f, TestName = "Raycast on Quad - Ray Hits From Bottom")]
        [TestCase(0f, 0f, 2f, 2f, 0f, 2f, 0f, -1f, 5f, true, 0f, 1f, 0f, 1f, TestName = "Raycast on Quad - Ray Hits From Top")]
        [TestCase(0f, 0f, 2f, 2f, -2f, 0f, 1f, 0f, 5f, true, -1f, 0f, -1f, 0f, TestName = "Raycast on Quad - Ray Hits From Left")]
        [TestCase(0f, 0f, 2f, 2f, 2f, 0f, -1f, 0f, 5f, true, 1f, 0f, 1f, 0f, TestName = "Raycast on Quad - Ray Hits From Right")]
        [TestCase(0f, 0f, 2f, 2f, 2f, 2f, -1f, 0f, 5f, false, TestName = "Raycast on Quad - Ray Misses")]
        [TestCase(0f, 0f, 2f, 2f, -2f, 1f, 1f, 0f, 5f, true, -1f, 1f, -1f, 0f, TestName = "Raycast on Quad - Ray Hits Top Most Point")]
        public static void PolyGonCollider_QuadIsHitByTest(float qx, float qy, float qw, float qh, float rx, float ry, float directionX, float directionY, float distance, bool shouldHit, float ix = 0f, float iy = 0f, float nx = 0f, float ny = 0f) {
            using (var quadBody = new DynamicPhysicsBody()) {
                quadBody.Initialize(new GameEntity());
                quadBody.Entity.SetWorldPosition(new Vector2(qx, qy));
                quadBody.Collider = PolygonCollider.CreateRectangle(qw, qh);

                var ray = new LineSegment(new Vector2(rx, ry), new Vector2(directionX, directionY), distance);
                Assert.AreEqual(shouldHit, quadBody.Collider.IsHitBy(ray, out var hit));

                if (shouldHit) {
                    var normal = new Vector2(nx, ny);
                    var intersection = new Vector2(ix, iy);

                    Assert.AreEqual(normal, hit.Normal);
                    Assert.AreEqual(intersection, hit.ContactPoint);
                    Assert.AreEqual(quadBody.Collider, hit.Collider);
                }
            }
        }
    }
}