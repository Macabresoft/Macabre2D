namespace Macabresoft.Macabre2D.Tests.Framework {

    using Macabresoft.Macabre2D.Framework;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public static class IdentifiableContentComponentTests {

        [Test]
        [Category("Unit Test")]
        public static void IdentifiableContentComponent_AudioPlayerTest() {
            var audioPlayer = new AudioPlayerComponent();
            var audioClip = new AudioClip();
            audioPlayer.AudioClip = audioClip;

            Assert.True(audioPlayer.HasAsset(audioClip.Id));
            audioPlayer.RemoveAsset(audioClip.Id);
            Assert.False(audioPlayer.HasAsset(audioClip.Id));
        }

        [Test]
        [Category("Unit Test")]
        public static void IdentifiableContentComponent_SpriteRendererTest() {
            var spriteRenderer = new SpriteRenderComponent();
            var sprite = new Sprite(Guid.NewGuid());
            spriteRenderer.Sprite = sprite;

            Assert.True(spriteRenderer.HasAsset(sprite.Id));
            spriteRenderer.RemoveAsset(sprite.Id);
            Assert.False(spriteRenderer.HasAsset(sprite.Id));
        }

        [Test]
        [Category("Unit Test")]
        public static void IdentifiableContentComponent_TextRendererTest() {
            var textRenderer = new TextRenderComponent();
            var font = new Font(Guid.NewGuid());
            textRenderer.Font = font;

            Assert.True(textRenderer.HasAsset(font.Id));
            textRenderer.RemoveAsset(font.Id);
            Assert.False(textRenderer.HasAsset(font.Id));
        }
    }
}