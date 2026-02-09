namespace Macabre2D.Tests.Framework.Systems;

using System;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class FixedUpdateSystemTests {
    [TestCase(1f, 1, 1)]
    [TestCase(1.5f, 1, 0)]
    [TestCase(2f, 5, 2)]
    [TestCase(1f, 100, 100)]
    public static void Update_ShouldWork(float timeStep, int secondsPassed, int callsExpected) {
        var scene = new Scene();
        var entity = scene.AddChild<FixedUpdateTestEntity>();
        var system = scene.AddSystem<FixedUpdateSystem>();
        system.TimeStep = timeStep;
        scene.Initialize(GameHelpers.CreateGameSubstitute(), Substitute.For<IAssetManager>());

        system.Update(new FrameTime(new GameTime(TimeSpan.Zero, new TimeSpan(0, 0, 0, secondsPassed)), 1D), new InputState());

        using (new AssertionScope()) {
            entity.UpdateCount.Should().Be(callsExpected);
        }
    }

    private class FixedUpdateTestEntity : Entity, IFixedUpdateableEntity {

        public event EventHandler ShouldUpdateChanged;
        public event EventHandler UpdateOrderChanged;
        public bool ShouldUpdate => this.IsEnabled;
        public int UpdateCount { get; private set; }

        public void FixedUpdate(float timeStep) {
            this.UpdateCount++;
        }
    }
}