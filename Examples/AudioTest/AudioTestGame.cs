namespace Macabre2D.Examples.AudioTest {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Audio;
    using Macabre2D.Framework.Serialization;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class AudioTestGame : MacabreGame {

        protected override void LoadContent() {
            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            var scene = new Scene();

            var audioPlayer = new AudioPlayer();
            scene.AddChild(audioPlayer);
            audioPlayer.Volume = 0.5f;
            audioPlayer.AudioClip = new AudioClip();
            audioPlayer.AudioClip.ContentPath = "laser";
            audioPlayer.AddChild(new VolumeController());

            scene.SaveToFile(@"TestGame - CurrentLevel.json", new Serializer());
            this.CurrentScene = new Serializer().Deserialize<Scene>(@"TestGame - CurrentLevel.json");
            this._isLoaded = true;
        }
    }
}