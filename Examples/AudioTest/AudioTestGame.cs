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
            var song = new Song();
            song.SampleRate = 16000;
            song.BeatsPerMinute = 220D;

            var fourthNote = (ushort)(song.SamplesPerBeat / 4);

            var track1 = song.AddTrack();
            track1.LeftChannelVolume = 0.75f;
            track1.RightChannelVolume = 0.25f;
            track1.Instrument.NoteEnvelope.Attack = (ushort)(song.SamplesPerBeat * 2);
            track1.Instrument.NoteEnvelope.Release = song.SamplesPerBeat;
            track1.AddNote(0, 4, MusicalNote.C);
            track1.AddNote(4, 4, MusicalNote.F);
            track1.AddNote(8, 4, MusicalNote.G);
            track1.AddNote(12, 4, MusicalNote.C);

            var track2 = song.AddTrack();
            track2.Instrument.Oscillator = new SawToothOscillator();
            track2.Instrument.Pitch = MusicalPitch.High;
            track2.Instrument.NoteEnvelope.Attack = fourthNote;
            track2.Instrument.NoteEnvelope.Decay = fourthNote;
            track2.Instrument.NoteEnvelope.Release = fourthNote;
            track2.Instrument.NoteEnvelope.SustainAmplitude = 0.25f;
            track2.Instrument.NoteEnvelope.PeakAmplitude = 1f;
            track2.LeftChannelVolume = 0.25f;
            track2.RightChannelVolume = 0.75f;

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
            track3.Instrument.NoteEnvelope.Attack = fourthNote;
            track3.Instrument.NoteEnvelope.Decay = fourthNote;
            track3.Instrument.NoteEnvelope.Release = fourthNote;

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

            var track4 = song.AddTrack();
            track4.Instrument.Oscillator = new WhiteNoiseOscillator();
            track4.Instrument.NoteEnvelope.Attack = (ushort)(fourthNote / 2);
            track4.Instrument.NoteEnvelope.Decay = (ushort)(fourthNote / 2);
            track4.Instrument.NoteEnvelope.SustainAmplitude = 0f;
            track4.AddNote(1, 1, MusicalNote.G);
            track4.AddNote(3, 1, MusicalNote.G);
            track4.AddNote(5, 1, MusicalNote.C);
            track4.AddNote(7, 1, MusicalNote.C);
            track4.AddNote(9, 1, MusicalNote.D);
            track4.AddNote(11, 1, MusicalNote.D);
            track4.AddNote(13, 1, MusicalNote.G);
            track4.AddNote(15, 1, MusicalNote.G);

            var synth = new SynthesizerComponent {
                Song = song,
                Volume = 1f
            };

            return synth;
        }
    }
}