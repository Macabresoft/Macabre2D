namespace Macabresoft.MonoGame.Tests.Core.Physics {

    using Macabresoft.MonoGame.Core;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;

    [TestFixture]
    public static class CircleColliderTests {

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 0f, 0f, 0.5f, true, TestName = "Circle to Circle Collision - First Contains Second")]
        [TestCase(0.5f, 0f, 1f, 0f, 0f, 1f, true, TestName = "Circle to Circle Collision - Right Collision")]
        [TestCase(-0.5f, 0f, 1f, 0f, 0f, 1f, true, TestName = "Circle to Circle Collision - Left Collision")]
        [TestCase(3f, 3f, 1f, 0f, 0f, 1f, false, TestName = "Circle to Circle Collision - No Collision")]
        public static void CircleCollider_CollidesWithCircleTest(float x1, float y1, float r1, float x2, float y2, float r2, bool collisionOccured) {
            using (var circleBody1 = new DynamicBodyComponent())
            using (var circleBody2 = new DynamicBodyComponent()) {
                circleBody1.SetWorldPosition(new Vector2(x1, y1));
                circleBody1.Collider = new CircleCollider(r1);
                circleBody1.Initialize(null);

                circleBody2.SetWorldPosition(new Vector2(x2, y2));
                circleBody2.Collider = new CircleCollider(r2);
                circleBody2.Initialize(null);

                Assert.AreEqual(collisionOccured, circleBody1.Collider.CollidesWith(circleBody2.Collider, out var collision1));
                Assert.AreEqual(collisionOccured, circleBody2.Collider.CollidesWith(circleBody1.Collider, out var collision2));

                if (collisionOccured) {
                    Assert.AreEqual(collision1.MinimumTranslationVector.Length(), collision2.MinimumTranslationVector.Length(), 0.0001f);
                    Assert.AreEqual(collision1.FirstCollider, collision2.SecondCollider);
                    Assert.AreEqual(collision1.SecondCollider, collision2.FirstCollider);
                    Assert.AreEqual(collision1.FirstContainsSecond, collision2.SecondContainsFirst);
                    Assert.AreEqual(collision1.SecondContainsFirst, collision2.FirstContainsSecond);

                    var originalPosition = circleBody1.WorldTransform.Position;
                    circleBody1.SetWorldPosition(originalPosition + collision1.MinimumTranslationVector);
                    Assert.False(circleBody1.Collider.CollidesWith(circleBody2.Collider, out collision1));
                    circleBody1.SetWorldPosition(originalPosition);

                    circleBody2.SetWorldPosition(circleBody2.WorldTransform.Position + collision2.MinimumTranslationVector);
                    Assert.False(circleBody2.Collider.CollidesWith(circleBody1.Collider, out collision2));
                }
                else {
                    Assert.Null(collision1);
                    Assert.Null(collision2);
                }
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 0f, 0f, 0.5f, 0.5f, true, TestName = "Circle to Quad Collision - Circle Contains Square")]
        [TestCase(0f, 0f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Circle to Quad Collision - Square Contains Circle")]
        [TestCase(0.5f, 0f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Circle to Quad Collision - Right Collision")]
        [TestCase(-0.5f, 0f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Circle to Quad Collision - Left Collision")]
        [TestCase(3f, 3f, 1f, 0f, 0f, 1f, 1f, false, TestName = "Circle to Quad Collision - No Collision")]
        [TestCase(0f, -130f, 50f, 0f, -200f, 500f, 50f, true, TestName = "Circle to Quad Collision - Physics Test Failure")]
        public static void CircleCollider_CollidesWithQuadTest(float x1, float y1, float r1, float x2, float y2, float w, float h, bool collisionOccured) {
            using (var circleBody = new DynamicBodyComponent())
            using (var quadBody = new DynamicBodyComponent()) {
                circleBody.SetWorldPosition(new Vector2(x1, y1));
                circleBody.Collider = new CircleCollider(r1);
                circleBody.Initialize(null);

                quadBody.SetWorldPosition(new Vector2(x2, y2));
                quadBody.Collider = PolygonCollider.CreateRectangle(w, h);
                quadBody.Initialize(null);

                Assert.AreEqual(collisionOccured, circleBody.Collider.CollidesWith(quadBody.Collider, out var collision1));
                Assert.AreEqual(collisionOccured, quadBody.Collider.CollidesWith(circleBody.Collider, out var collision2));

                if (collisionOccured) {
                    Assert.AreEqual(collision1.MinimumTranslationVector.Length(), collision2.MinimumTranslationVector.Length(), 0.0001f);
                    Assert.AreEqual(collision1.FirstCollider, collision2.SecondCollider);
                    Assert.AreEqual(collision1.SecondCollider, collision2.FirstCollider);
                    Assert.AreEqual(collision1.FirstContainsSecond, collision2.SecondContainsFirst);
                    Assert.AreEqual(collision1.SecondContainsFirst, collision2.FirstContainsSecond);

                    var originalPosition = circleBody.WorldTransform.Position;
                    circleBody.SetWorldPosition(originalPosition + collision1.MinimumTranslationVector);
                    Assert.False(circleBody.Collider.CollidesWith(quadBody.Collider, out collision1));
                    circleBody.SetWorldPosition(originalPosition);

                    quadBody.SetWorldPosition(quadBody.WorldTransform.Position + collision2.MinimumTranslationVector);
                    Assert.False(quadBody.Collider.CollidesWith(circleBody.Collider, out collision2));
                }
                else {
                    Assert.Null(collision1);
                    Assert.Null(collision2);
                }
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 0f, 0f, 1f, false, TestName = "Circle Contains Circle - Same Circle")]
        [TestCase(0f, 0f, 1f, 0f, 0f, 0.5f, true, TestName = "Circle Contains Circle - Inside at Center")]
        [TestCase(0f, 0f, 1f, 0.25f, -0.25f, 0.25f, true, TestName = "Circle Contains Circle - Inside Offset")]
        [TestCase(0f, 0f, 1f, 0.25f, -0.25f, 1f, false, TestName = "Circle Contains Circle - Inside But Does Not Contain")]
        [TestCase(0f, 0f, 1f, 3f, 3f, 1f, false, TestName = "Circle Contains Circle - Outside")]
        [TestCase(0f, 0f, 1f, 2f, 0f, 1f, false, TestName = "Circle Contains Circle - Edges Touch")]
        public static void CircleCollider_ContainsCircleTest(float x1, float y1, float r1, float x2, float y2, float r2, bool shouldContain) {
            using (var circleBody1 = new DynamicBodyComponent())
            using (var circleBody2 = new DynamicBodyComponent()) {
                circleBody1.SetWorldPosition(new Vector2(x1, y1));
                circleBody1.Collider = new CircleCollider(r1);
                circleBody1.Initialize(null);

                circleBody2.SetWorldPosition(new Vector2(x2, y2));
                circleBody2.Collider = new CircleCollider(r2);
                circleBody2.Initialize(null);

                Assert.AreEqual(shouldContain, circleBody1.Collider.Contains(circleBody2.Collider));
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 0f, 0f, true, TestName = "Circle Contains Point - Point is Center")]
        [TestCase(0f, 0f, 1f, 0f, 1f, true, TestName = "Circle Contains Point - Point is on Edge")]
        [TestCase(0f, 0f, 1f, 0.25f, 0.25f, true, TestName = "Circle Contains Point - Point is Inside")]
        [TestCase(0f, 0f, 1f, 1f, 1f, false, TestName = "Circle Contains Point - Point is Outside")]
        [TestCase(0f, 0f, 1f, 100f, 100f, false, TestName = "Circle Contains Point - Point is Way Outside")]
        public static void CircleCollider_ContainsPointTest(float x1, float y1, float r1, float x2, float y2, bool shouldContain) {
            using (var circleBody = new DynamicBodyComponent()) {
                circleBody.SetWorldPosition(new Vector2(x1, y1));
                circleBody.Collider = new CircleCollider(r1);
                var circle = circleBody.Collider as CircleCollider;
                circleBody.Initialize(null);

                Assert.AreEqual(shouldContain, circle.Contains(new Vector2(x2, y2)));
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 0f, 0f, 1f, 1f, true, TestName = "Circle Contains Quad - Inside at Center")]
        [TestCase(0f, 0f, 1f, 0.25f, -0.25f, 0.25f, 0.5f, true, TestName = "Circle Contains Quad - Inside Offset")]
        [TestCase(0f, 0f, 1f, 0.25f, -0.25f, 2f, 1f, false, TestName = "Circle Contains Quad - Inside But Does Not Contain")]
        [TestCase(0f, 0f, 1f, 3f, 3f, 1f, 1f, false, TestName = "Circle Contains Quad - Outside")]
        [TestCase(0f, 0f, 1f, 2f, 0f, 2f, 2f, false, TestName = "Circle Contains Quad - Edges Touch")]
        public static void CircleCollider_ContainsQuadTest(float x1, float y1, float r1, float x2, float y2, float w, float h, bool shouldContain) {
            using (var circleBody = new DynamicBodyComponent())
            using (var quadBody = new DynamicBodyComponent()) {
                circleBody.SetWorldPosition(new Vector2(x1, y1));
                circleBody.Collider = new CircleCollider(r1);
                circleBody.Initialize(null);

                quadBody.SetWorldPosition(new Vector2(x2, y2));
                quadBody.Collider = PolygonCollider.CreateRectangle(w, h);
                quadBody.Initialize(null);

                Assert.AreEqual(shouldContain, circleBody.Collider.Contains(quadBody.Collider));
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(0f, 0f, 1f, 0f, -2f, 0f, 1f, 5f, true, 0f, -1f, 0f, -1f, TestName = "Raycast on Circle - Ray Hits From Bottom")]
        [TestCase(0f, 0f, 1f, 0f, 2f, 0f, -1f, 5f, true, 0f, 1f, 0f, 1f, TestName = "Raycast on Circle - Ray Hits From Top")]
        [TestCase(0f, 0f, 1f, -2f, 0f, 1f, 0f, 5f, true, -1f, 0f, -1f, 0f, TestName = "Raycast on Circle - Ray Hits From Left")]
        [TestCase(0f, 0f, 1f, 2f, 0f, -1f, 0f, 5f, true, 1f, 0f, 1f, 0f, TestName = "Raycast on Circle - Ray Hits From Right")]
        [TestCase(0f, 0f, 1f, 2f, 2f, -1f, 0f, 5f, false, TestName = "Raycast on Circle - Ray Misses")]
        [TestCase(0f, 0f, 1f, -2f, 1f, 1f, 0f, 5f, true, 0f, 1f, 0f, 1f, TestName = "Raycast on Circle - Ray Hits Top Most Point")]
        public static void CircleCollider_IsHitByTest(
            float cx, float cy, float r,
            float rx, float ry, float directionX, float directionY, float distance,
            bool shouldHit, float ix = 0f, float iy = 0f, float nx = 0f, float ny = 0f) {
            using (var circleBody = new DynamicBodyComponent()) {
                circleBody.SetWorldPosition(new Vector2(cx, cy));
                circleBody.Collider = new CircleCollider(r);
                circleBody.Initialize(null);

                var ray = new LineSegment(new Vector2(rx, ry), new Vector2(directionX, directionY), distance);
                Assert.AreEqual(shouldHit, circleBody.Collider.IsHitBy(ray, out var hit));

                if (shouldHit) {
                    var normal = new Vector2(nx, ny);
                    var intersection = new Vector2(ix, iy);

                    Assert.AreEqual(normal.X, hit.Normal.X, 0.0001f);
                    Assert.AreEqual(normal.Y, hit.Normal.Y, 0.0001f);
                    Assert.AreEqual(intersection.X, hit.ContactPoint.X, 0.0001f);
                    Assert.AreEqual(intersection.Y, hit.ContactPoint.Y, 0.0001f);
                    Assert.AreEqual(circleBody.Collider, hit.Collider);
                }
            }
        }
    }
}