namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

/// <summary>
/// Interface for an instance of a <see cref="AudioClip" />.
/// </summary>
public interface IAudioClipInstance : IDisposable {
    /// <summary>
    /// Gets the audio category.
    /// </summary>
    VolumeCategory Category { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="AudioPlayer" /> should loop.
    /// </summary>
    bool ShouldLoop { get; }

    /// <summary>
    /// Gets the state.
    /// </summary>
    SoundState State { get; }

    /// <summary>
    /// Gets or sets the pan.
    /// </summary>
    float Pan { get; set; }

    /// <summary>
    /// Gets or sets the pitch.
    /// </summary>
    float Pitch { get; set; }

    /// <summary>
    /// Gets or sets the volume.
    /// </summary>
    float Volume { get; set; }

    /// <summary>
    /// Pauses this instance.
    /// </summary>
    void Pause();

    /// <summary>
    /// Play this instance.
    /// </summary>
    void Play();

    /// <summary>
    /// Stop this instance.
    /// </summary>
    void Stop();

    /// <summary>
    /// Stop this instance.
    /// </summary>
    /// <param name="isImmediate">If set to <c>true</c> is immediate.</param>
    void Stop(bool isImmediate);
}

/// <summary>
/// An instance of a <see cref="AudioClip" /> that can handle operations for a <see cref="SoundEffectInstance" />.
/// </summary>
public sealed class AudioClipInstance : IAudioClipInstance {
    private readonly SoundEffectInstance _leftChannel;
    private readonly SoundEffectInstance _rightChannel;
    private readonly AudioSettings _settings;
    private float _categoryVolume;
    private float _pan;
    private float _volume;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundEffectInstance" /> class.
    /// </summary>
    /// <param name="soundEffect">The sound effect.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="category">The category.</param>
    /// <param name="volume">The volume.</param>
    /// <param name="pan">The pan.</param>
    /// <param name="pitch">The pitch.</param>
    /// <param name="shouldLoop">A value indicating whether this should loop.</param>
    public AudioClipInstance(
        SoundEffect soundEffect,
        AudioSettings settings,
        VolumeCategory category,
        float volume,
        float pan,
        float pitch,
        bool shouldLoop) {
        this._leftChannel = soundEffect.CreateInstance();
        this._rightChannel = soundEffect.CreateInstance();
        this._leftChannel.Pan = -1f;
        this._rightChannel.Pan = 1f;
        this._settings = settings;
        this.Category = category;

        this._volume = volume;
        this._categoryVolume = this.GetVolume();
        this.Pan = pan;
        this.Pitch = pitch;
        this.ShouldLoop = shouldLoop;

        this._settings.VolumeChanged += this.Settings_VolumeChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundEffectInstance" /> class.
    /// </summary>
    /// <param name="soundEffect">The sound effect.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="category">The category.</param>
    public AudioClipInstance(SoundEffect soundEffect, AudioSettings settings, VolumeCategory category) : this(soundEffect, settings, category, 1f, 0f, 0f, false) {
    }

    /// <inheritdoc />
    public VolumeCategory Category { get; }

    /// <summary>
    /// Gets an empty instance of <see cref="IAudioClipInstance" />.
    /// </summary>
    public static IAudioClipInstance Empty => new EmptyAudioClipInstance();

    /// <inheritdoc />
    public SoundState State => this._leftChannel.State;

    /// <inheritdoc />
    public float Pan {
        get => this._pan;
        set {
            this._pan = MathHelper.Clamp(value, -1f, 1f);
            this.ResetVolume();
        }
    }

    /// <inheritdoc />
    public float Pitch {
        get => this._leftChannel.Pitch;
        set {
            this._leftChannel.Pitch = MathHelper.Clamp(value, -1f, 1f);
            this._rightChannel.Pitch = this._leftChannel.Pitch;
        }
    }

    /// <inheritdoc />
    public bool ShouldLoop {
        get => this._leftChannel.IsLooped;
        private init {
            this._leftChannel.IsLooped = value;
            this._rightChannel.IsLooped = value;
        }
    }

    /// <inheritdoc />
    public float Volume {
        get => this._volume;
        set {
            this._volume = MathHelper.Clamp(value, 0f, 1f);
            this._categoryVolume = this.GetVolume();
            this.ResetVolume();
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        this._settings.VolumeChanged -= this.Settings_VolumeChanged;
    }

    /// <inheritdoc />
    public void Pause() {
        if (this._leftChannel.State == SoundState.Playing) {
            this._leftChannel.Pause();
        }

        if (this._rightChannel.State == SoundState.Playing) {
            this._rightChannel.Pause();
        }
    }

    /// <inheritdoc />
    public void Play() {
        this._leftChannel.IsLooped = this.ShouldLoop;
        this._rightChannel.IsLooped = this.ShouldLoop;
        this._leftChannel.Volume = this.GetVolume();
        this._leftChannel.Play();
        this._rightChannel.Play();
    }

    /// <inheritdoc />
    public void Stop() {
        this.Stop(true);
    }

    /// <inheritdoc />
    public void Stop(bool isImmediate) {
        this._leftChannel.Stop(isImmediate);
        this._rightChannel.Stop(isImmediate);
    }

    private float GetVolume() => this._settings.GetVolume(this.Category, this._volume);

    private void ResetVolume() {
        var rightChannelMultiplier = (this._pan + 1f) / 2f;
        this._rightChannel.Volume = this._categoryVolume * rightChannelMultiplier;

        var leftChannelMultipler = 1f - rightChannelMultiplier;
        this._leftChannel.Volume = this._categoryVolume * leftChannelMultipler;
    }

    private void Settings_VolumeChanged(object? sender, VolumeCategory e) {
        if (this.State == SoundState.Playing && (e == VolumeCategory.Overall || e == this.Category)) {
            this._categoryVolume = this.GetVolume();
            this.ResetVolume();
        }
    }

    private sealed class EmptyAudioClipInstance : IAudioClipInstance {
        public VolumeCategory Category => VolumeCategory.Overall;
        public bool ShouldLoop => false;
        public SoundState State => SoundState.Stopped;
        public float Pan { get; set; }
        public float Pitch { get; set; }
        public float Volume { get; set; }

        public void Dispose() {
        }

        public void Pause() {
        }

        public void Play() {
        }

        public void Stop() {
        }

        public void Stop(bool isImmediate) {
        }
    }
}