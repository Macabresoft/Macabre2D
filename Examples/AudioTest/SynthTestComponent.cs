namespace Macabre2D.Examples.AudioTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Input;
    using System;

    public sealed class SynthTestComponent : BaseComponent, IUpdateableComponent {
        private const int ChannelsCount = 2;
        private const int SampleRate = 44100;
        private const int SamplesPerBuffer = 3000;

        private readonly byte[] _audioBuffer = new byte[ChannelsCount * SamplesPerBuffer * 2];
        private readonly float[,] _workiingBuffer = new float[ChannelsCount, SamplesPerBuffer];
        private DynamicSoundEffectInstance _instance;

        public override void LoadContent() {
            this._instance = new DynamicSoundEffectInstance(SampleRate, AudioChannels.Stereo);
            this._instance.Play();
            base.LoadContent();
        }

        public void Update(GameTime gameTime) {
            var frequency = 0;
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.D1)) {
                frequency = 261630;
            }
            else if (keyboardState.IsKeyDown(Keys.D2)) {
                frequency = 349228;
            }
            else if (keyboardState.IsKeyDown(Keys.D3)) {
                frequency = 391995;
            }

            if (frequency > 0) {
                while (this._instance.PendingBufferCount < 3) {
                    this.SubmitBuffer(frequency);
                }
            }
        }

        protected override void Initialize() {
        }

        private void ConvertBuffer() {
            for (int sampleIndex = 0; sampleIndex < SamplesPerBuffer; sampleIndex++) {
                for (int channel = 0; channel < ChannelsCount; channel++) {
                    var workingSample = MathHelper.Clamp(this._workiingBuffer[channel, sampleIndex], 0f, 1f);
                    var shortSample = (short)(workingSample >= 0f ? workingSample * short.MaxValue : workingSample * -short.MinValue);
                    var index = sampleIndex * ChannelsCount * 2 + channel * 2;

                    if (!BitConverter.IsLittleEndian) {
                        this._audioBuffer[index] = (byte)(shortSample >> 8);
                        this._audioBuffer[index + 1] = (byte)(shortSample);
                    }
                    else {
                        this._audioBuffer[index] = (byte)(shortSample);
                        this._audioBuffer[index + 1] = (byte)(shortSample >> 8);
                    }
                }
            }
        }

        private float CreateSineWave(double time, double frequency) {
            return (float)Math.Sin(time * 2 * Math.PI * frequency);
        }

        private void FillWorkingBuffer(int frequency) {
            var time = 0f;
            for (var i = 0; i < SamplesPerBuffer; i++) {
                this._workiingBuffer[0, i] = this.CreateSineWave(time, frequency);
                this._workiingBuffer[1, i] = this.CreateSineWave(time, frequency);

                time += 1.0f / SampleRate;
            }
        }

        private void SubmitBuffer(int frequency) {
            this.FillWorkingBuffer(frequency);
            this.ConvertBuffer();
            this._instance.SubmitBuffer(this._audioBuffer);
        }
    }
}