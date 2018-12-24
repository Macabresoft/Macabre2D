namespace Macabre2D.Examples.AudioTest {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Audio;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class AudioTestGame : MacabreGame {

        protected override void LoadContent() {
            base.LoadContent();
            var scene = new Scene();

            var audioPlayer = new AudioPlayer();
            scene.AddChild(audioPlayer);
            audioPlayer.Volume = 0.5f;
            audioPlayer.AudioClip = new AudioClip();
            audioPlayer.AudioClip.ContentPath = "laser";
            audioPlayer.AddChild(new VolumeController());

            scene.SaveAsJson(@"TestGame - CurrentLevel.json", new Serializer());
            this.CurrentScene = Scene.LoadFromJson(@"TestGame - CurrentLevel.json", new Serializer());
        }
    }
}