namespace Macabresoft.Macabre2D.Tests.Core.Rendering {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public static class SpriteAnimatorTests {

        [Test]
        [Category("Unit Tests")]
        public static void SpriteAnimator_LoopingTest() {
            var numberOfSteps = 3;
            var animation = CreateAnimation(numberOfSteps);
            var animator = CreateAnimator(1);
            animator.Initialize(new GameEntity());

            var gameTime = new GameTime {
                ElapsedGameTime = TimeSpan.FromMilliseconds(100d)
            };

            var frameTime = new FrameTime(gameTime, 1f);

            animator.Play(animation, true);
            Assert.IsNull(animator.Sprite);
            animator.Update(frameTime, default);
            Assert.AreEqual(animation.Steps.ElementAt(0).Sprite.Id, animator.Sprite.Id);

            for (var i = 1; i < numberOfSteps; i++) {
                gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
                frameTime = new FrameTime(gameTime, 1f);
                animator.Update(frameTime, default);
                Assert.AreEqual(animation.Steps.ElementAt(i).Sprite.Id, animator.Sprite.Id);
            }

            gameTime.ElapsedGameTime = TimeSpan.FromSeconds(1d);
            frameTime = new FrameTime(gameTime, 1f);

            // Should loop here.
            animator.Update(frameTime, default);
            Assert.AreEqual(animation.Steps.ElementAt(0).Sprite.Id, animator.Sprite.Id);
        }

        private static SpriteAnimation CreateAnimation(int numberOfSteps) {
            var steps = new List<SpriteAnimationStep>();
            for (var i = 0; i < numberOfSteps; i++) {
                steps.Add(new SpriteAnimationStep {
                    Frames = 1,
                    Sprite = new Sprite(Guid.NewGuid())
                });
            }

            return new SpriteAnimation(steps);
        }

        private static SpriteAnimationComponent CreateAnimator(byte frameRate) {
            var animator = new SpriteAnimationComponent() {
                FrameRate = frameRate
            };

            return animator;
        }
    }
}