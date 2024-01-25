namespace Macabresoft.Macabre2D.Tests.Framework.Physics;

using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

public static class RectangleColliderTests {
    [Test]
    [Category("Unit Tests")]
    [TestCase(0f, 0f, 1f, 0f, 0f, 0.5f, 0.5f, true, TestName = "Rectangle to Circle Collision - Circle Contains Square")]
    [TestCase(0f, 0f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Rectangle to Circle Collision - Square Contains Circle")]
    [TestCase(0.5f, 0f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Rectangle to Circle Collision - Right Collision")]
    [TestCase(-0.5f, 0f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Rectangle to Circle Collision - Left Collision")]
    [TestCase(3f, 3f, 1f, 0f, 0f, 1f, 1f, false, TestName = "Rectangle to Circle Collision - No Collision")]
    [TestCase(0f, -130f, 50f, 0f, -200f, 500f, 50f, true, TestName = "Rectangle to Circle Collision - Physics Test Failure")]
    public static void RectangleCollider_CollidesWithCircleTest(
        float x1,
        float y1,
        float r1,
        float x2,
        float y2,
        float w,
        float h,
        bool collisionOccured) {
        var circleBody = new DynamicPhysicsBody();
        var rectangleBody = new DynamicPhysicsBody();
        var scene = Substitute.For<IScene>();
        circleBody.Initialize(scene, new Entity());
        rectangleBody.Initialize(scene, new Entity());

        circleBody.SetWorldPosition(new Vector2(x1, y1));
        circleBody.Collider = new CircleCollider(r1);

        rectangleBody.SetWorldPosition(new Vector2(x2, y2));
        rectangleBody.Collider = new RectangleCollider(new Vector2(-0.5f * w, -0.5f * h), new Vector2(0.5f * w, 0.5f * h));

        Assert.That(collisionOccured, Is.EqualTo(circleBody.Collider.CollidesWith(rectangleBody.Collider, out var collision1)));
        Assert.That(collisionOccured, Is.EqualTo(rectangleBody.Collider.CollidesWith(circleBody.Collider, out var collision2)));

        if (collisionOccured) {
            Assert.That(collision1.MinimumTranslationVector.Length(), Is.EqualTo(collision2.MinimumTranslationVector.Length()).Within(0.0001f));
            Assert.That(collision1.FirstCollider, Is.EqualTo(collision2.SecondCollider));
            Assert.That(collision1.SecondCollider, Is.EqualTo(collision2.FirstCollider));
            Assert.That(collision1.FirstContainsSecond, Is.EqualTo(collision2.SecondContainsFirst));
            Assert.That(collision1.SecondContainsFirst, Is.EqualTo(collision2.FirstContainsSecond));

            var originalPosition = circleBody.WorldPosition;
            circleBody.SetWorldPosition(originalPosition + collision1.MinimumTranslationVector);
            Assert.That(circleBody.Collider.CollidesWith(rectangleBody.Collider, out collision1), Is.False);
            circleBody.SetWorldPosition(originalPosition);

            rectangleBody.SetWorldPosition(rectangleBody.WorldPosition + collision2.MinimumTranslationVector);
            Assert.That(rectangleBody.Collider.CollidesWith(circleBody.Collider, out collision2), Is.False);
        }
        else {
            Assert.That(collision1, Is.Null);
            Assert.That(collision2, Is.Null);
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(0f, 0f, 1f, 1f, 0f, 0f, 0.5f, 0.5f, true, TestName = "Rectangle to Rectangle Collision - First Rectangle Contains Second Rectangle")]
    [TestCase(0f, 0f, 0.5f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Rectangle to Rectangle Collision - Second Rectangle Contains First Rectangle")]
    [TestCase(0.5f, 0f, 0.5f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Rectangle to Rectangle Collision - Right Collision")]
    [TestCase(-0.5f, 0f, 0.5f, 0.5f, 0f, 0f, 1f, 1f, true, TestName = "Rectangle to Rectangle Collision - Left Collision")]
    [TestCase(3f, 3f, 1f, 1f, 0f, 0f, 1f, 1f, false, TestName = "Rectangle to Rectangle Collision - No Collision")]
    [TestCase(0f, -130f, 100f, 100f, 0f, -200f, 500f, 100f, true, TestName = "Rectangle to Rectangle Collision - Physics Test Failure")]
    public static void RectangleCollider_RectangleCollidesWithRectangleTest(
        float x1,
        float y1,
        float w1,
        float h1,
        float x2,
        float y2,
        float w2,
        float h2,
        bool collisionOccured) {
        var rectangleBody1 = new DynamicPhysicsBody();
        var rectangleBody2 = new DynamicPhysicsBody();
        var scene = Substitute.For<IScene>();
        rectangleBody1.Initialize(scene, new Entity());
        rectangleBody2.Initialize(scene, new Entity());

        rectangleBody1.SetWorldPosition(new Vector2(x1, y1));
        rectangleBody1.Collider = new RectangleCollider(new Vector2(-0.5f * w1, -0.5f * h1), new Vector2(0.5f * w1, 0.5f * h1));

        rectangleBody2.SetWorldPosition(new Vector2(x2, y2));
        rectangleBody2.Collider = new RectangleCollider(new Vector2(-0.5f * w2, -0.5f * h2), new Vector2(0.5f * w2, 0.5f * h2));

        Assert.That(collisionOccured, Is.EqualTo(rectangleBody1.Collider.CollidesWith(rectangleBody2.Collider, out var collision1)));
        Assert.That(collisionOccured, Is.EqualTo(rectangleBody2.Collider.CollidesWith(rectangleBody1.Collider, out var collision2)));

        if (collisionOccured) {
            Assert.That(collision1.MinimumTranslationVector.Length(), Is.EqualTo(collision2.MinimumTranslationVector.Length()).Within(0.0001f));
            Assert.That(collision1.FirstCollider, Is.EqualTo(collision2.SecondCollider));
            Assert.That(collision1.SecondCollider, Is.EqualTo(collision2.FirstCollider));
            Assert.That(collision1.FirstContainsSecond, Is.EqualTo(collision2.SecondContainsFirst));
            Assert.That(collision1.SecondContainsFirst, Is.EqualTo(collision2.FirstContainsSecond));

            var originalPosition = rectangleBody1.WorldPosition;
            rectangleBody1.SetWorldPosition(originalPosition + collision1.MinimumTranslationVector);
            Assert.That(rectangleBody1.Collider.CollidesWith(rectangleBody2.Collider, out collision1), Is.False);
            rectangleBody1.SetWorldPosition(originalPosition);

            rectangleBody2.SetWorldPosition(rectangleBody2.WorldPosition + collision2.MinimumTranslationVector);
            Assert.That(rectangleBody2.Collider.CollidesWith(rectangleBody1.Collider, out collision2), Is.False);
        }
        else {
            Assert.That(collision1, Is.Null);
            Assert.That(collision2, Is.Null);
        }
    }
}