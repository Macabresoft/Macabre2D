namespace Macabre2D.Examples.AudioTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class SynthTestComponent : BaseComponent, IUpdateableComponentAsync {
        private const int SampleRate = 44100;
        private const int SamplesPerBuffer = 1000;

        private readonly byte[] _audioBuffer = new byte[SamplesPerBuffer * 2];
        private readonly double[] _workiingBuffer = new double[SamplesPerBuffer];
        private DynamicSoundEffectInstance _instance;

        private Queue<IOscillator> _oscillators = new Queue<IOscillator>();
        private IOscillator _selectedOscillator;

        public override void LoadContent() {
            var pulseWaveOscillator = new PulseWaveOscillator() {
                DutyCycle = 0.1f
            };

            this._selectedOscillator = new SineWaveOscillator();
            this._oscillators.Enqueue(new TriangleWaveOscillator());
            this._oscillators.Enqueue(new SawToothOscillator());
            this._oscillators.Enqueue(new SquareWaveOscillator());
            this._oscillators.Enqueue(pulseWaveOscillator);
            this._oscillators.Enqueue(new WhiteNoiseOscillator());

            this._instance = new DynamicSoundEffectInstance(SampleRate, AudioChannels.Mono);
            this._instance.Play();
            base.LoadContent();
        }

        public Task UpdateAsync(GameTime gameTime) {
            var frequency = 0f;
            var keyboardState = Keyboard.GetState();

            return Task.Run(() => {
                if (keyboardState.IsKeyDown(Keys.Right)) {
                    this._oscillators.Enqueue(this._selectedOscillator);
                    this._selectedOscillator = this._oscillators.Dequeue();
                    Thread.Sleep(200);
                }

                if (keyboardState.IsKeyDown(Keys.D1)) {
                    frequency = MusicalScale.C.ToFrequency(MusicalPitch.Middle);
                }
                else if (keyboardState.IsKeyDown(Keys.D2)) {
                    frequency = MusicalScale.G.ToFrequency(MusicalPitch.Middle);
                }
                else if (keyboardState.IsKeyDown(Keys.D3)) {
                    frequency = MusicalScale.F.ToFrequency(MusicalPitch.Middle);
                }

                if (frequency > 0) {
                    while (this._instance.PendingBufferCount < 2) {
                        this.SubmitBuffer(frequency);
                    }
                }
            });
        }

        protected override void Initialize() {
        }

        private void ConvertBuffer() {
            for (int sampleIndex = 0; sampleIndex < SamplesPerBuffer; sampleIndex++) {
                var workingSample = Math.Max(0D, Math.Min(this._workiingBuffer[sampleIndex], 0.9D));
                var shortSample = (short)(workingSample >= 0f ? workingSample * short.MaxValue : workingSample * -short.MinValue);
                var index = sampleIndex * 2;

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

        private void FillWorkingBuffer(float frequency) {
            var time = 0f;
            for (var i = 0; i < SamplesPerBuffer; i++) {
                this._workiingBuffer[i] = this._selectedOscillator.GetSignal(time, frequency, 0.8f);

                time += 1.0f / SampleRate;
            }
        }

        private void SubmitBuffer(float frequency) {
            this.FillWorkingBuffer(frequency);
            this.ConvertBuffer();
            this._instance.SubmitBuffer(this._audioBuffer);
        }
    }
}