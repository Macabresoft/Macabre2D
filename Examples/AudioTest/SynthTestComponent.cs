namespace Macabre2D.Examples.AudioTest {

    using CosmicSynth.Framework;
    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public sealed class SynthTestComponent : BaseComponent, IUpdateableComponent {
        private const int SampleRate = 44100 / 2;
        private const int SamplesPerBuffer = 500;
        private readonly byte[] _audioBuffer = new byte[SamplesPerBuffer * 2];
        private readonly float[] _workiingBuffer = new float[SamplesPerBuffer];
        private DynamicSoundEffectInstance _instance;
        private Queue<IOscillator> _oscillators = new Queue<IOscillator>();
        private IOscillator _selectedOscillator;
        private float _time = 0f;

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

        public void Update(GameTime gameTime) {
            var frequency = 0f;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Right)) {
                this._oscillators.Enqueue(this._selectedOscillator);
                this._selectedOscillator = this._oscillators.Dequeue();
                Thread.Sleep(200);
            }

            if (keyboardState.IsKeyDown(Keys.D1)) {
                frequency = Note.C.ToFrequency(Pitch.Normal);
            }
            else if (keyboardState.IsKeyDown(Keys.D2)) {
                frequency = Note.F.ToFrequency(Pitch.Normal);
            }
            else if (keyboardState.IsKeyDown(Keys.D3)) {
                frequency = Note.G.ToFrequency(Pitch.Normal);
            }
            else {
                this._time = 0f;
            }

            if (frequency > 0) {
                while (this._instance.PendingBufferCount < 2) {
                    Console.WriteLine(frequency);
                    this.SubmitBuffer(frequency);
                }
            }
        }

        protected override void Initialize() {
        }

        private void ConvertBuffer() {
            for (var sampleIndex = 0; sampleIndex < SamplesPerBuffer; sampleIndex++) {
                var workingSample = MathHelper.Clamp(this._workiingBuffer[sampleIndex], -1f, 1f);
                var shortSample = (short)Math.Round(workingSample >= 0f ? workingSample * short.MaxValue : workingSample * short.MinValue * -1);
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
            for (var i = 0; i < SamplesPerBuffer; i++) {
                this._workiingBuffer[i] = this._selectedOscillator.GetSignal(this._time, frequency, 1f);

                this._time += 1.0f / SampleRate;
            }
        }

        private void SubmitBuffer(float frequency) {
            this.FillWorkingBuffer(frequency);
            this.ConvertBuffer();
            this._instance.SubmitBuffer(this._audioBuffer);
        }
    }
}