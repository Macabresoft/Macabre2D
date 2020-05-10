namespace Macabre2D.UI.CosmicSynthLibrary.Controls.SongEditing {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using System;

    public sealed class PianoKeyComponent : SimpleBodyComponent, IClickablePianoRollComponent, IUpdateableComponent {
        private const ushort SamplesPerBuffer = 1000;
        private static readonly Point BlackKeySpriteLocation = new Point(0, 0);
        private static readonly Point PianoKeySpriteSize = new Point(32, 16);
        private static readonly Point WhiteKeySpriteLocation = new Point(0, 16);
        private readonly IPianoRoll _pianoRoll;
        private float _currentTimePassed = 0f;
        private float _inverseSampleRate;
        private ushort _sampleRate;
        private DynamicSoundEffectInstance _soundEffectInstance;
        private SpriteRenderComponent _spriteRenderer;

        public PianoKeyComponent(Note note, Pitch pitch, IPianoRoll pianoRoll) : base() {
            this._pianoRoll = pianoRoll;
            this.LocalPosition = new Vector2(-2f, this._pianoRoll.GetRowPosition(note, pitch) + 0.5f);
            this.Frequency = new Frequency(note, pitch);
            this.PropertyChanged += this.Self_PropertyChanged;
        }

        public Frequency Frequency { get; }

        public bool IsClickable {
            get {
                return this.IsEnabled;
            }
        }

        public int Priority {
            get {
                return 0;
            }
        }

        public void EndClick() {
            this._soundEffectInstance.Stop();
        }

        public bool TryClick(Vector2 mouseWorldPosition) {
            var result = false;
            if (this.Collider.Contains(mouseWorldPosition)) {
                this._currentTimePassed = 0f;

                if (this._soundEffectInstance == null || this._pianoRoll.Song.SampleRate != this._sampleRate) {
                    this._sampleRate = this._pianoRoll.Song.SampleRate;
                    this._inverseSampleRate = 1f / this._pianoRoll.Song.SampleRate;
                    this._soundEffectInstance = new DynamicSoundEffectInstance(this._sampleRate, AudioChannels.Mono);
                }

                this._soundEffectInstance.Play();
                result = true;
            }

            return result;
        }

        public bool TryHoldClick(Vector2 mouseWorldPosition) {
            var result = false;
            if (this.Collider.Contains(mouseWorldPosition)) {
                result = true;
            }
            else {
                this.EndClick();
            }

            return result;
        }

        public void Update(FrameTime frameTime) {
            if (this._soundEffectInstance != null && this._soundEffectInstance.State == SoundState.Playing && this._soundEffectInstance.PendingBufferCount < 2) {
                var samples = new byte[SongPlayer.BytesPerSample * SamplesPerBuffer];

                for (var i = 0; i < SamplesPerBuffer; i++) {
                    var sample = this._pianoRoll.Track.Instrument.Oscillator.GetSignal(this._currentTimePassed, this.Frequency.Value, 1f);

                    foreach (var effect in this._pianoRoll.Track.Instrument.Effects) {
                        sample = effect.ApplyEffect(sample, this._currentTimePassed);
                    }

                    var shortSample = (short)(sample >= 0.0f ? sample * short.MaxValue : sample * -short.MinValue);
                    var index = i * SongPlayer.BytesPerSample;

                    if (!BitConverter.IsLittleEndian) {
                        samples[index] = (byte)(shortSample >> 8);
                        samples[index + 1] = (byte)shortSample;
                    }
                    else {
                        samples[index] = (byte)shortSample;
                        samples[index + 1] = (byte)(shortSample >> 8);
                    }

                    this._currentTimePassed += this._inverseSampleRate;
                }

                this._soundEffectInstance.SubmitBuffer(samples);
            }
        }

        protected override void Initialize() {
            base.Initialize();

            this._spriteRenderer = this.AddChild<SpriteRenderComponent>();
            this._spriteRenderer.Sprite = new Sprite(AssetManager.Instance.GetId(PianoRoll.SpriteSheetPath), this.Frequency.Note.IsNatural() ? WhiteKeySpriteLocation : BlackKeySpriteLocation, PianoKeySpriteSize);
            this._spriteRenderer.RenderSettings.OffsetType = PixelOffsetType.BottomLeft;

            this.Collider = new RectangleCollider(2f, 1f) {
                Offset = 0.5f * this._spriteRenderer.RenderSettings.Size * GameSettings.Instance.InversePixelsPerUnit
            };
        }

        private void Self_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.IsEnabled)) {
                this.RaisePropertyChanged(nameof(this.IsClickable));
            }
        }
    }
}