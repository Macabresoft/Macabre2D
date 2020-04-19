namespace Macabre2D.Examples.AudioTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AudioTestGame : MacabreGame {

        protected override void LoadContent() {
            this.AssetManager.Initialize(this.Content);
            var lasterId = Guid.NewGuid();
            this.AssetManager.SetMapping(lasterId, "laser");
            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            var scene = new Scene();

            var audioPlayer = new AudioPlayer();
            scene.AddComponent(audioPlayer);
            audioPlayer.Volume = 0.5f;
            audioPlayer.AudioClip = new AudioClip();
            audioPlayer.AudioClip.Id = lasterId;
            audioPlayer.AddChild(new VolumeController());

            scene.SaveToFile(@"TestGame - CurrentLevel.json");
            this.CurrentScene = Serializer.Instance.Deserialize<Scene>(@"TestGame - CurrentLevel.json");
            this.CurrentScene.AddComponent<PianoComponent>();
            this._isLoaded = true;
        }
    }
}