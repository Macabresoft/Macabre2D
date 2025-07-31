namespace Macabresoft.Macabre2D.Tests.Framework.Rendering;

using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class QueueableSpriteAnimatorTests {
    [Test]
    [Category("Unit Tests")]
    public static void GetPercentageComplete_ShouldWork_WithManyStepsAndFramesEach() {
        CreateAnimation(10, 10, true, out var animator, out var gameTime, out var frameTime);

        gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
        frameTime = new FrameTime(gameTime, 1f);

        for (var i = 0; i < 25; i++) {
            animator.IncrementTime(frameTime);
        }

        using (new AssertionScope()) {
            var percentageComplete = animator.GetPercentageComplete();
            percentageComplete.Should().Be(0.25f);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void GetPercentageComplete_ShouldWork_WithOneStepOneFrame() {
        CreateAnimation(1, 1, true, out var animator, out _, out _);

        using (new AssertionScope()) {
            var percentageComplete = animator.GetPercentageComplete();
            percentageComplete.Should().Be(0f);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void GetPercentageComplete_ShouldWork_WithOneStepTwoFrames() {
        CreateAnimation(2, 1, true, out var animator, out var gameTime, out var frameTime);

        gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
        frameTime = new FrameTime(gameTime, 1f);
        animator.IncrementTime(frameTime);

        using (new AssertionScope()) {
            var percentageComplete = animator.GetPercentageComplete();
            percentageComplete.Should().Be(0.5f);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void GetPercentageComplete_ShouldWork_WithTwoStepsOneFrameEach() {
        CreateAnimation(2, 1, true, out var animator, out var gameTime, out var frameTime);

        gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
        frameTime = new FrameTime(gameTime, 1f);
        animator.IncrementTime(frameTime);

        using (new AssertionScope()) {
            var percentageComplete = animator.GetPercentageComplete();
            percentageComplete.Should().Be(0.5f);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void IncrementTime_ShouldNotTriggerOnAnimationFinished_WhenAnimationNotFinished() {
        CreateAnimation(1, 1, false, out var animator, out var gameTime, out var frameTime);
        var isFinished = false;
        SpriteAnimation finishedAnimation = null;
        animator.OnAnimationFinished += (_, spriteAnimation) =>
        {
            isFinished = true;
            finishedAnimation = spriteAnimation;
        };

        using (new AssertionScope()) {
            gameTime.ElapsedGameTime = TimeSpan.FromMilliseconds(1d);
            frameTime = new FrameTime(gameTime, 1f);
            animator.IncrementTime(frameTime);
            isFinished.Should().BeFalse();
            finishedAnimation.Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void IncrementTime_ShouldNotTriggerOnAnimationFinished_WhenLooping() {
        CreateAnimation(1, 1, true, out var animator, out var gameTime, out var frameTime);
        var isFinished = false;
        SpriteAnimation finishedAnimation = null;
        animator.OnAnimationFinished += (_, spriteAnimation) =>
        {
            isFinished = true;
            finishedAnimation = spriteAnimation;
        };

        using (new AssertionScope()) {
            gameTime.ElapsedGameTime = TimeSpan.FromSeconds(10d);
            frameTime = new FrameTime(gameTime, 1f);
            animator.IncrementTime(frameTime);
            isFinished.Should().BeFalse();
            finishedAnimation.Should().BeNull();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void IncrementTime_ShouldTriggerOnAnimationFinished_WhenAnimationFinished() {
        var animation = CreateAnimation(1, 1, false, out var animator, out var gameTime, out var frameTime);
        var isFinished = false;
        SpriteAnimation finishedAnimation = null;
        animator.OnAnimationFinished += (_, spriteAnimation) =>
        {
            isFinished = true;
            finishedAnimation = spriteAnimation;
        };

        using (new AssertionScope()) {
            gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
            frameTime = new FrameTime(gameTime, 1f);
            animator.IncrementTime(frameTime);
            isFinished.Should().BeTrue();
            finishedAnimation.Should().Be(animation);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void Play_ShouldTriggerOnAnimationFinished() {
        var animation = CreateAnimation(1, 1, false, out var animator, out var gameTime, out _);
        var secondAnimation = CreateAnimation(5, 1, false, out _, out _, out _);
        var isFinished = false;
        SpriteAnimation finishedAnimation = null;
        animator.OnAnimationFinished += (_, spriteAnimation) =>
        {
            isFinished = true;
            finishedAnimation = spriteAnimation;
        };

        using (new AssertionScope()) {
            gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
            animator.Play(secondAnimation, false);
            isFinished.Should().BeTrue();
            finishedAnimation.Should().Be(animation);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void QueueableSpriteAnimator_IncrementTime_LoopingTest() {
        var animation = CreateAnimation(3, 1, true, out var animator, out var gameTime, out var frameTime);

        using (new AssertionScope()) {
            animator.SpriteIndex.Should().Be(animation.Steps.ElementAt(0).SpriteIndex);

            for (byte i = 0; i < animation.Steps.Count; i++) {
                animator.SpriteIndex.Should().Be(animation.Steps.ElementAt(i).SpriteIndex);
                gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
                frameTime = new FrameTime(gameTime, 1f);
                animator.IncrementTime(frameTime);
            }

            // Should loop here.
            animator.SpriteIndex.Should().Be(animation.Steps.ElementAt(0).SpriteIndex);
        }
    }

    [Test]
    [TestCase(5, 1)]
    [TestCase(10, 10)]
    [TestCase(25, 3)]
    [Category("Unit Tests")]
    public static void SetPercentageComplete_ShouldWork_WithManySteps(byte steps, byte framesPerStep) {
        CreateAnimation(steps, framesPerStep, false, out var animator, out _, out _);

        using (new AssertionScope()) {
            for (var step = 0; step <= steps; step++) {
                for (var frame = 0; frame <= framesPerStep; frame++) {
                    var percentageComplete = animator.GetPercentageComplete();
                    animator.SetPercentageComplete(percentageComplete);
                    var newPercentageComplete = animator.GetPercentageComplete();
                    newPercentageComplete.Should().BeApproximately(percentageComplete, Defaults.FloatComparisonTolerance);
                    animator.NextFrame();
                }
            }
        }
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(25)]
    [Category("Unit Tests")]
    public static void SetPercentageComplete_ShouldWork_WithOneStep(byte frames) {
        CreateAnimation(1, frames, false, out var animator, out var gameTime, out var frameTime);

        using (new AssertionScope()) {
            for (var i = 0; i <= frames; i++) {
                var percentageComplete = animator.GetPercentageComplete();
                animator.SetPercentageComplete(percentageComplete);
                animator.GetPercentageComplete().Should().BeApproximately(percentageComplete, Defaults.FloatComparisonTolerance);
                animator.NextFrame();
            }
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void Stop_ShouldTriggerOnAnimationFinished() {
        var animation = CreateAnimation(1, 1, true, out var animator, out var gameTime, out _);
        var isFinished = false;
        SpriteAnimation finishedAnimation = null;
        animator.OnAnimationFinished += (_, spriteAnimation) =>
        {
            isFinished = true;
            finishedAnimation = spriteAnimation;
        };

        using (new AssertionScope()) {
            gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
            animator.Stop();
            isFinished.Should().BeTrue();
            finishedAnimation.Should().Be(animation);
        }
    }

    private static SpriteAnimation CreateAnimation(
        byte numberOfSteps,
        byte framesPerStep,
        bool isLooping,
        out QueueableSpriteAnimator animator,
        out GameTime gameTime,
        out FrameTime frameTime) {
        var steps = new List<SpriteAnimationStep>();
        for (byte i = 0; i < numberOfSteps; i++) {
            steps.Add(new SpriteAnimationStep {
                Frames = framesPerStep,
                SpriteIndex = i
            });
        }

        var animation = new SpriteAnimation(steps);
        animation.SpriteSheet = new SpriteSheet {
            Columns = 2,
            Rows = 2
        };

        if (animation.SpriteSheet.GetMemberCollection<SpriteAnimation>() is { } collection) {
            collection.Add(animation);
        }

        animator = CreateAnimator(1);
        gameTime = new GameTime {
            ElapsedGameTime = TimeSpan.Zero
        };

        frameTime = new FrameTime(gameTime, 1f);

        animator.Play(animation, isLooping);
        return animation;
    }

    private static QueueableSpriteAnimator CreateAnimator(byte frameRate) {
        var animator = new QueueableSpriteAnimator();

        animator.FrameRateOverride.IsEnabled = frameRate > 0;
        animator.FrameRateOverride.Value = frameRate;


        var scene = Substitute.For<IScene>();
        var assets = Substitute.For<IAssetManager>();
        scene.Assets.Returns(assets);

        animator.Initialize(scene, new Entity());

        return animator;
    }
}