namespace Macabresoft.Macabre2D.Tests.Framework.Physics;

using System;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class SimplePhysicsGameSystemTests {
    [Test]
    [Category("Unit Tests")]
    [TestCase(-2f, 0f, 1f, 0f, true, true, TestName = "Raycast to Circle Collider - Collision")]
    [TestCase(-2f, 0f, 1f, 0f, false, false, TestName = "Raycast to Circle Collider - Different Layers")]
    [TestCase(-2f, 2f, 1f, 0f, true, false, TestName = "Raycast to Circle Collider - No Collision")]
    [TestCase(-2f, 2f, 1f, 0f, false, false, TestName = "Raycast to Circle Collider - No Collision / Different Layers")]
    public static void RaycastCircleTest(
        float raycastX,
        float raycastY,
        float directionX,
        float directionY,
        bool layersCompatible,
        bool raycastHit) {
        var scene = new Scene();
        var project = Substitute.For<IGameProject>();
        var game = GameHelpers.CreateGameSubstitute();
        game.Project.Returns(project);

        var physicsSystem = scene.AddSystem<SimplePhysicsGameSystem>();
        var circleBody = scene.AddChild<SimplePhysicsBody>();
        circleBody.SetWorldPosition(Vector2.Zero);
        circleBody.Collider = new CircleCollider(1f);
        circleBody.Layers = (Layers)1;
        scene.Initialize(game, Substitute.For<IAssetManager>());

        // Need to do this update to insure colliders are inserter.
        scene.Update(new FrameTime(new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)), 1), new InputState());

        var raycastLayer = layersCompatible ? circleBody.Layers : (Layers)2;
        var result = physicsSystem.TryRaycast(new Vector2(raycastX, raycastY), new Vector2(directionX, directionY), 5f, raycastLayer, out var hit);
        Assert.That(raycastHit, Is.EqualTo(result));
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(0f, 0.6499903f, 0f, -1f, 0.666667f, true, TestName = "Raycast to Line Collider - Collision #1")]
    public static void RaycastLineTest(
        float raycastX,
        float raycastY,
        float directionX,
        float directionY,
        float distance,
        bool raycastHit) {
        var scene = new Scene();
        var project = Substitute.For<IGameProject>();
        var game = GameHelpers.CreateGameSubstitute();
        game.Project.Returns(project);

        var physicsSystem = scene.AddSystem<SimplePhysicsGameSystem>();

        var lineBody = scene.AddChild<SimplePhysicsBody>();
        lineBody.SetWorldPosition(Vector2.Zero);
        lineBody.Collider = new LineCollider(new Vector2(-1f, 0f), new Vector2(1f, 0f));
        lineBody.Layers = Layers.Default;
        lineBody.Initialize(scene, scene);

        scene.Initialize(game, Substitute.For<IAssetManager>());
        physicsSystem.TimeStep = 1f;
        scene.Update(new FrameTime(new GameTime(new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 1)), 1), new InputState());

        var result = physicsSystem.TryRaycast(new Vector2(raycastX, raycastY), new Vector2(directionX, directionY), distance, Layers.Default, out var hit);
        Assert.That(raycastHit, Is.EqualTo(result));
        result = physicsSystem.TryRaycast(new Vector2(raycastX, raycastY), new Vector2(directionX, directionY), distance, Layers.Default, out hit);
        Assert.That(raycastHit, Is.EqualTo(result));

        scene.Update(new FrameTime(new GameTime(new TimeSpan(0, 0, 2), new TimeSpan(0, 0, 1)), 1), new InputState());

        result = physicsSystem.TryRaycast(new Vector2(raycastX, raycastY), new Vector2(directionX, directionY), distance, Layers.Default, out hit);
        Assert.That(raycastHit, Is.EqualTo(result));
    }
}