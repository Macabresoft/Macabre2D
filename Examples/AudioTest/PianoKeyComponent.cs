using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Macabre2D.Examples.AudioTest {

    public sealed class PianoKeyComponent : SimpleBodyComponent, IClickablePianoComponent, IUpdateableComponent {
        private const ushort SamplesPerBuffer = 1000;
        private const float VisualShrinkAmount = 0.9f;
        private readonly Color _color;
        private readonly SpriteRenderComponent _spriteRenderer;
        private float _currentTimePassed = 0f;
        private Instrument _instrument;
        private float _inverseSampleRate;
        private ushort _sampleRate;
        private Song _song;
        private DynamicSoundEffectInstance _soundEffectInstance;

        public PianoKeyComponent(Note note, Pitch pitch) : base() {
            this.LocalPosition = new Vector2(-2f, SceneHelper.GetRowPosition(note, pitch) + 0.5f);
            this.LocalScale = new Vector2(1f, VisualShrinkAmount);
            this.Frequency = new Frequency(note, pitch);

            if (note.IsNatural()) {
                this._color = Color.AntiqueWhite;
            }
            else {
                this._color = Color.DarkSlateGray;
            }

            this._spriteRenderer = this.AddChild<SpriteRenderComponent>();
            this._spriteRenderer.Sprite = PrimitiveDrawer.CreateQuadSprite(MacabreGame.Instance.GraphicsDevice, new Point(1, 1), this._color);
            this._spriteRenderer.LocalScale = new Vector2(GameSettings.Instance.PixelsPerUnit * 2f, GameSettings.Instance.PixelsPerUnit * VisualShrinkAmount);
            this._spriteRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;

            this.Collider = new RectangleCollider(2f, 1f) {
                Offset = this._spriteRenderer.RenderSettings.Offset
            };

            this.IsEnabledChanged += this.Self_IsEnabledChanged;
        }

        public event EventHandler ClickabilityChanged;

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

        public bool TryClick(Vector2 mouseWorldPosition, Song currentSong, Instrument currentInstrument) {
            var result = false;
            if (this.Collider.Contains(mouseWorldPosition)) {
                this._currentTimePassed = 0f;
                this._song = currentSong;
                this._instrument = currentInstrument;

                if (this._soundEffectInstance == null || this._song.SampleRate != this._sampleRate) {
                    this._sampleRate = this._song.SampleRate;
                    this._inverseSampleRate = 1f / this._song.SampleRate;
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
            if (this._instrument != null && this._soundEffectInstance != null && this._soundEffectInstance.State == SoundState.Playing && this._soundEffectInstance.PendingBufferCount < 2) {
                var samples = new byte[SongPlayer.BytesPerSample * SamplesPerBuffer];

                for (var i = 0; i < SamplesPerBuffer; i++) {
                    var sample = this._instrument.Oscillator.GetSignal(this._currentTimePassed, this.Frequency.Value, 1f);

                    foreach (var effect in this._instrument.Effects) {
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

        private void Self_IsEnabledChanged(object sender, EventArgs e) {
            this.ClickabilityChanged.SafeInvoke(this);
        }
    }
}