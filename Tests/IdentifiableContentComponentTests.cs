namespace Macabre2D.Tests {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Audio;
    using Macabre2D.Framework.Rendering;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public static class IdentifiableContentComponentTests {

        [Test]
        [Category("Unit Test")]
        public static void IdentifiableContentComponent_AudioPlayerTest() {
            var audioPlayer = new AudioPlayer();
            var audioClip = new AudioClip();
            audioPlayer.AudioClip = audioClip;

            Assert.True(audioPlayer.HasContent(audioClip.Id));
            audioPlayer.RemoveContent(audioClip.Id);
            Assert.False(audioPlayer.HasContent(audioClip.Id));
        }

        [Test]
        [Category("Unit Test")]
        public static void IdentifiableContentComponent_SpriteAnimationTest() {
            var spriteAnimation = new SpriteAnimation();
            var spriteAnimator = new SpriteAnimator(spriteAnimation);

            var spriteAnimationId = spriteAnimation.Id;

            var steps = new List<SpriteAnimationStep>();

            for (var i = 0; i < 10; i++) {
                var step = new SpriteAnimationStep();
                step.Sprite = new Sprite();
                steps.Add(step);
                spriteAnimation.AddStep(step);
            };

            var firstStep = steps.First();
            var firstSpriteId = firstStep.Sprite.Id;
            Assert.True(spriteAnimator.HasContent(firstSpriteId));

            spriteAnimator.RemoveContent(firstSpriteId);
            Assert.False(spriteAnimator.HasContent(firstSpriteId));
            Assert.Null(firstStep.Sprite);

            var lastStep = steps.Last();
            var lastSpriteId = lastStep.Sprite.Id;
            Assert.True(spriteAnimator.HasContent(lastSpriteId));

            spriteAnimator.RemoveContent(lastSpriteId);
            Assert.False(spriteAnimator.HasContent(lastSpriteId));
            Assert.Null(lastStep.Sprite);
            Assert.True(spriteAnimator.HasContent(spriteAnimationId));

            spriteAnimator.RemoveContent(spriteAnimationId);
            Assert.False(spriteAnimator.HasContent(spriteAnimationId));
        }

        [Test]
        [Category("Unit Test")]
        public static void IdentifiableContentComponent_SpriteRendererTest() {
            var spriteRenderer = new SpriteRenderer();
            var sprite = new Sprite();
            spriteRenderer.Sprite = sprite;

            Assert.True(spriteRenderer.HasContent(sprite.Id));
            spriteRenderer.RemoveContent(sprite.Id);
            Assert.False(spriteRenderer.HasContent(sprite.Id));
        }

        [Test]
        [Category("Unit Test")]
        public static void IdentifiableContentComponent_TextRendererTest() {
            var textRenderer = new TextRenderer();
            var font = new Font();
            textRenderer.Font = font;

            Assert.True(textRenderer.HasContent(font.Id));
            textRenderer.RemoveContent(font.Id);
            Assert.False(textRenderer.HasContent(font.Id));
        }
    }
}