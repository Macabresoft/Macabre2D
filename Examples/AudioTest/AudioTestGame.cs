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
            audioPlayer.AudioClip.AssetId = lasterId;
            audioPlayer.AddChild(new VolumeController());

            var synth = new SynthTestComponent();
            scene.AddComponent(synth);

            scene.AddComponent(this.CreateSynthesizer());

            scene.SaveToFile(@"TestGame - CurrentLevel.json");
            this.CurrentScene = Serializer.Instance.Deserialize<Scene>(@"TestGame - CurrentLevel.json");
            this._isLoaded = true;
        }

        private SynthesizerComponent CreateSynthesizer() {
            var song = new Song {
                BeatsPerMinute = 140D
            };

            var track1 = song.AddTrack();
            track1.AddNote(0, 4, MusicalNote.C);
            track1.AddNote(4, 4, MusicalNote.F);
            track1.AddNote(8, 4, MusicalNote.G);
            track1.AddNote(12, 4, MusicalNote.C);

            var track2 = song.AddTrack();
            track2.Instrument.Oscillator = new SawToothOscillator();
            track2.Instrument.Pitch = MusicalPitch.High;
            track2.AddNote(0, 1, MusicalNote.C);
            track2.AddNote(2, 1, MusicalNote.C);
            track2.AddNote(4, 1, MusicalNote.F);
            track2.AddNote(6, 1, MusicalNote.F);

            track2.AddNote(8, 1, MusicalNote.G);
            track2.AddNote(9, 1, MusicalNote.B);
            track2.AddNote(10, 1, MusicalNote.D);
            track2.AddNote(11, 1, MusicalNote.B);

            track2.AddNote(12, 1, MusicalNote.C);
            track2.AddNote(14, 1, MusicalNote.C);

            var track3 = song.AddTrack();
            track3.Instrument.Oscillator = new SquareWaveOscillator();
            track3.Instrument.Pitch = MusicalPitch.Lower;

            track3.AddNote(0, 1, MusicalNote.C);
            track3.AddNote(1, 1, MusicalNote.G);
            track3.AddNote(2, 1, MusicalNote.C);
            track3.AddNote(3, 1, MusicalNote.G);

            track3.AddNote(4, 1, MusicalNote.F);
            track3.AddNote(5, 1, MusicalNote.C);
            track3.AddNote(6, 1, MusicalNote.F);
            track3.AddNote(7, 1, MusicalNote.C);

            track3.AddNote(8, 1, MusicalNote.G);
            track3.AddNote(9, 1, MusicalNote.D);
            track3.AddNote(10, 1, MusicalNote.G);
            track3.AddNote(11, 1, MusicalNote.D);

            track3.AddNote(12, 1, MusicalNote.C);
            track3.AddNote(13, 1, MusicalNote.G);
            track3.AddNote(14, 1, MusicalNote.C);
            track3.AddNote(15, 1, MusicalNote.G);

            var synth = new SynthesizerComponent();
            synth.Song = song;
            synth.Volume = 1f;
            return synth;
        }
    }
}