namespace Macabre2D.Examples.AudioTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
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
            audioPlayer.AudioClip.AssetId = lasterId;
            audioPlayer.AddChild(new VolumeController());

            scene.SaveToFile(@"TestGame - CurrentLevel.json");
            this.CurrentScene = Serializer.Instance.Deserialize<Scene>(@"TestGame - CurrentLevel.json");
            this._isLoaded = true;
        }
    }
}