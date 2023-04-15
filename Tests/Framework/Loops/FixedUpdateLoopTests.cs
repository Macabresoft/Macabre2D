namespace Macabresoft.Macabre2D.Tests.Framework.Loops; 

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class FixedUpdateLoopTests {
    [TestCase(1f, 1, 1)]
    [TestCase(1.5f, 1, 0)]
    [TestCase(2f, 5, 2)]
    [TestCase(1f, 100, 100)]
    public static void Update_ShouldWork(float timeStep, int secondsPassed, int callsExpected) {
        var scene = new Scene();
        var entity = scene.AddChild<FixedUpdateTestEntity>();
        var loop = scene.AddLoop<FixedUpdateLoop>();
        loop.TimeStep = timeStep;
        scene.Initialize(Substitute.For<IGame>(), Substitute.For<IAssetManager>());

        loop.Update(new FrameTime(new GameTime(TimeSpan.Zero, new TimeSpan(0, 0, 0, secondsPassed)), 1D), new InputState());

        using (new AssertionScope()) {
            entity.UpdateCount.Should().Be(callsExpected);
        }
    }

    private class FixedUpdateTestEntity : Entity, IFixedUpdateableEntity {
        public int UpdateCount { get; private set; }

        public void FixedUpdate(float timeStep) {
            this.UpdateCount++;
        }
    }
}