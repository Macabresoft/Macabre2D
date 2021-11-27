namespace Macabresoft.Macabre2D.Tests.Framework.Rendering {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
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

            var animation = new SpriteAnimation(steps);
            animation.SpriteSheet = new SpriteSheet() {
                Columns = 2,
                Rows = 2
            };
            
            if (animation.SpriteSheet.SpriteAnimations is SpriteAnimationCollection collection) {
                collection.Add(animation);
            }
            
            return animation;
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
            const byte NumberOfSteps = 3;
            var animation = CreateAnimation(NumberOfSteps);
            var animator = CreateAnimator(1);
            var scene = Substitute.For<IScene>();
            var assets = Substitute.For<IAssetManager>();
            scene.Assets.Returns(assets);

            animator.Initialize(scene, new Entity());

            var gameTime = new GameTime {
                ElapsedGameTime = TimeSpan.FromMilliseconds(100d)
            };

            var frameTime = new FrameTime(gameTime, 1f);

            animator.Play(animation, true);
            animator.Update(frameTime, default);
            Assert.AreEqual(animation.Steps.ElementAt(0).SpriteIndex, animator.SpriteIndex);

            for (byte i = 1; i < NumberOfSteps; i++) {
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