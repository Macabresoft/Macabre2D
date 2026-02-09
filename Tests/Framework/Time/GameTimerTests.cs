namespace Macabre2D.Tests.Framework.Time;

using System;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NUnit.Framework;

[TestFixture]
public class GameTimerTests {
    [Category("Unit Tests")]
    [Test]
    [TestCase(1f, 1f, TimerState.Finished)]
    [TestCase(10f, 5f, TimerState.Running)]
    [TestCase(100f, 1000f, TimerState.Finished)]
    public static void Increment_ShouldHandleStatusChange(float timeLimit, float timeToPass, TimerState endState) {
        var gameTimer = new GameTimer { TimeLimit = timeLimit };
        gameTimer.Start(new FrameTime());
        gameTimer.Increment(new FrameTime(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(timeToPass)), 1));

        using (new AssertionScope()) {
            gameTimer.State.Should().Be(endState);
        }
    }

    [Category("Unit Tests")]
    [Test]
    [TestCase(1f, 1f, TimerState.Finished)]
    [TestCase(10f, 5f, TimerState.Running)]
    [TestCase(100f, 1000f, TimerState.Finished)]
    public static void Start_ShouldHandleStatusChange(float timeLimit, float timeToPass, TimerState endState) {
        var gameTimer = new GameTimer { TimeLimit = timeLimit };
        gameTimer.Start(new FrameTime(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(timeToPass)), 1));

        using (new AssertionScope()) {
            gameTimer.State.Should().Be(endState);
        }
    }
}