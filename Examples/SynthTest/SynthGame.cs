namespace Macabre2D.Examples.SynthTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [ExcludeFromCodeCoverage]
    public class SynthGame : MacabreGame {
        private const string PianoSpriteSheetName = "PianoRollSpriteSheet";
        private static readonly Point BlackPressedKeySpriteLocation = new Point(0, 32);
        private static readonly Point BlackUnpressedKeySpriteLocation = new Point(0, 0);
        private static readonly Point PianoKeySpriteSize = new Point(32, 16);
        private static readonly Point WhitePressedKeySpriteLocation = new Point(0, 48);
        private static readonly Point WhiteUnpressedKeySpriteLocation = new Point(0, 16);

        protected override void Initialize() {
            base.Initialize();

            this._graphics.IsFullScreen = false;
            this._graphics.PreferredBackBufferHeight = 1080;
            this._graphics.PreferredBackBufferWidth = 1920;
            this._graphics.ApplyChanges();
            this.IsMouseVisible = true;
        }

        protected override void LoadContent() {
            this.AssetManager.Initialize(this.Content);

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.CurrentScene = new Scene {
                BackgroundColor = Color.Teal
            };

            var camera = this.CurrentScene.AddChild<Camera>();
            camera.ViewHeight = 36f;
            camera.OffsetSettings.OffsetType = PixelOffsetType.BottomLeft;
            camera.LocalPosition = Vector2.Zero;

            if (this.Settings is GameSettings settings) {
                settings.PixelsPerUnit = 16;
            }

            var spriteSheetId = Guid.NewGuid();
            this.AssetManager.SetMapping(spriteSheetId, PianoSpriteSheetName);
            var whiteKeyUnpressed = new Sprite(spriteSheetId, WhiteUnpressedKeySpriteLocation, PianoKeySpriteSize);
            var whiteKeyPressed = new Sprite(spriteSheetId, WhitePressedKeySpriteLocation, PianoKeySpriteSize);
            var blackKeyUnpressed = new Sprite(spriteSheetId, BlackUnpressedKeySpriteLocation, PianoKeySpriteSize);
            var blackKeyPressed = new Sprite(spriteSheetId, BlackPressedKeySpriteLocation, PianoKeySpriteSize);
            var songPlayer = new LiveSongPlayer(new Song());
            this.CurrentScene.AddChild(new PianoComponent(songPlayer, whiteKeyUnpressed, whiteKeyPressed, blackKeyUnpressed, blackKeyPressed));
            this.CurrentScene.AddChild(new SongPlayerComponent(this.CreateSong()));
            this._isLoaded = true;
        }

        private Song CreateSong() {
            var song = new Song {
                BeatsPerMinute = 120
            };

            var firstTrack = song.Tracks.First();
            firstTrack.Instrument.Oscillator = new SawToothOscillator();
            firstTrack.Instrument.NoteEnvelope.Attack = 50;
            firstTrack.Instrument.NoteEnvelope.Release = 50;
            firstTrack.LeftChannelVolume = 0.25f;
            firstTrack.RightChannelVolume = 0.75f;

            var secondTrack = song.AddTrack();
            secondTrack.Instrument.Oscillator = new SineWaveOscillator();
            secondTrack.Instrument.NoteEnvelope.Attack = 100;
            secondTrack.Instrument.NoteEnvelope.Release = 500;
            secondTrack.LeftChannelVolume = 0.75f;
            secondTrack.RightChannelVolume = 0.25f;

            for (var i = 0; i < 4; i++) {
                firstTrack.AddNote((4f * i) + 0f, 1f, Note.C);
                firstTrack.AddNote((4f * i) + 1f, 1f, Note.F);
                firstTrack.AddNote((4f * i) + 2f, 1f, Note.G);
                firstTrack.AddNote((4f * i) + 3f, 1f, Note.C);

                secondTrack.AddSlideNote((4f * i) + 0f, 0.4f, new Frequency(Note.C, Pitch.Low), new Frequency(Note.C, Pitch.Normal), 1f);
                secondTrack.AddSlideNote((4f * i) + 0.5f, 0.4f, new Frequency(Note.C, Pitch.Low), new Frequency(Note.C, Pitch.Normal), 1f);
                secondTrack.AddSlideNote((4f * i) + 1f, 0.4f, new Frequency(Note.F, Pitch.Low), new Frequency(Note.F, Pitch.Normal), 1f);
                secondTrack.AddSlideNote((4f * i) + 1.5f, 0.4f, new Frequency(Note.F, Pitch.Low), new Frequency(Note.F, Pitch.Normal), 1f);
                secondTrack.AddSlideNote((4f * i) + 2f, 0.4f, new Frequency(Note.G, Pitch.Low), new Frequency(Note.G, Pitch.Normal), 1f);
                secondTrack.AddSlideNote((4f * i) + 2.5f, 0.4f, new Frequency(Note.G, Pitch.Low), new Frequency(Note.G, Pitch.Normal), 1f);
                secondTrack.AddSlideNote((4f * i) + 3f, 0.4f, new Frequency(Note.C, Pitch.High), new Frequency(Note.C, Pitch.Normal), 1f);
                secondTrack.AddSlideNote((4f * i) + 3.5f, 0.4f, new Frequency(Note.C, Pitch.High), new Frequency(Note.C, Pitch.Normal), 1f);
            }

            return song;
        }
    }
}