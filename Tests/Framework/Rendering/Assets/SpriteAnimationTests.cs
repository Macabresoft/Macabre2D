namespace Macabre2D.Tests;

using System;
using System.Collections.Generic;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public static class SpriteAnimationTests {

    [Test]
    [Category("Unit Tests")]
    public static void SpriteAnimation_TotalNumberOfFrames_ShouldBeAccurate_WhenEmpty() {
        var spriteAnimation = new SpriteAnimation();
        spriteAnimation.TotalNumberOfFrames.Should().Be(0);
    }

    [Test]
    [Category("Unit Tests")]
    public static void SpriteAnimation_TotalNumberOfFrames_ShouldBeAccurate_WithInitialSteps() {
        var steps = new List<SpriteAnimationStep>();
        var totalFrames = 0;
        for (var i = 0; i < TimesToRun; i++) {
            var step = new SpriteAnimationStep();
            step.Frames = Random.Next(1, 100);
            steps.Add(step);
            totalFrames += step.Frames;
        }

        var spriteAnimation = new SpriteAnimation(steps);
        spriteAnimation.TotalNumberOfFrames.Should().Be(totalFrames);
    }

    [Test]
    [Category("Unit Tests")]
    public static void SpriteAnimation_TotalNumberOfFrames_ShouldBeAccurate_WithMultiple() {
        var spriteAnimation = new SpriteAnimation();
        var firstStep = spriteAnimation.AddStep();
        var secondStep = spriteAnimation.AddStep();

        using (new AssertionScope()) {
            for (var i = 0; i < TimesToRun; i++) {
                firstStep.Frames = Random.Next(1, 100);
                secondStep.Frames = Random.Next(1, 100);

                spriteAnimation.TotalNumberOfFrames.Should().Be(firstStep.Frames + secondStep.Frames);
            }
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void SpriteAnimation_TotalNumberOfFrames_ShouldBeAccurate_WithOneStep() {
        var spriteAnimation = new SpriteAnimation();
        var step = spriteAnimation.AddStep();

        using (new AssertionScope()) {
            for (var i = 0; i < TimesToRun; i++) {
                step.Frames = Random.Next(1, int.MaxValue);
                spriteAnimation.TotalNumberOfFrames.Should().Be(step.Frames);
            }
        }
    }

    private const int TimesToRun = 100;
    private static readonly Random Random = new();
}