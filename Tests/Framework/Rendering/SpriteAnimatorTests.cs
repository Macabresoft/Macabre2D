namespace Macabresoft.Macabre2D.Tests.Framework.Rendering {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public static class SpriteAnimatorTests {
        private static SpriteAnimation CreateAnimation(byte numberOfSteps) {
            var steps = new List<SpriteAnimationStep>();
            for (byte i = 0; i < numberOfSteps; i++) {
                steps.Add(new SpriteAnimationStep {
                    Frames = 1,
                    SpriteIndex = i
                });
            }

            return new SpriteAnimation(steps);
        }

        private static SpriteAnimator CreateAnimator(byte frameRate) {
            var animator = new SpriteAnimator {
                FrameRate = frameRate
            };

            return animator;
        }

        [Test]
        [Category("Unit Tests")]
        public static void SpriteAnimator_LoopingTest() {
            byte numberOfSteps = 3;
            var animation = CreateAnimation(numberOfSteps);
            var animator = CreateAnimator(1);
            var scene = Substitute.For<IScene>();
            animator.Initialize(scene, new Entity());

            var gameTime = new GameTime {
                ElapsedGameTime = TimeSpan.FromMilliseconds(100d)
            };

            var frameTime = new FrameTime(gameTime, 1f);

            animator.Play(animation, true);
            animator.Update(frameTime, default);
            Assert.AreEqual(animation.Steps.ElementAt(0).SpriteIndex, animator.SpriteIndex);

            for (byte i = 1; i < numberOfSteps; i++) {
                gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
                frameTime = new FrameTime(gameTime, 1f);
                animator.Update(frameTime, default);
                Assert.AreEqual(animation.Steps.ElementAt(i).SpriteIndex, animator.SpriteIndex);
            }

            gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
            frameTime = new FrameTime(gameTime, 1f);

            // Should loop here.
            animator.Update(frameTime, default);
            Assert.AreEqual(animation.Steps.ElementAt(0).SpriteIndex, animator.SpriteIndex);
        }
    }
}