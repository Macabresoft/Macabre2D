namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Audio;

/// <summary>
/// Plays a <see cref="AudioClip" />.
/// </summary>
public sealed class AudioPlayer : Entity {
    private IAudioClipInstance _instance = AudioClipInstance.Empty;
    private float _pan;
    private float _pitch;
    private float _volume;

    /// <summary>
    /// Gets the audio clip reference.
    /// </summary>
    [DataMember(Order = 0, Name = "Audio Clip")]
    public AudioClipReference AudioClipReference { get; } = new();

    /// <summary>
    /// Gets the state.
    /// </summary>
    public SoundState State => this._instance.State;

    /// <summary>
    /// Gets or sets the pan.
    /// </summary>
    public float Pan {
        get => this._pan;
        set {
            this._pan = value;
            this._instance.Pan = value;
        }
    }

    /// <summary>
    /// Gets or sets the pitch.
    /// </summary>
    [DataMember(Order = 4, Name = "Pitch")]
    public float Pitch {
        get => this._pitch;
        set {
            this._pitch = value;
            this._instance.Pitch = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="AudioPlayer" /> should loop.
    /// </summary>
    /// <value><c>true</c> if should loop; otherwise, <c>false</c>.</value>
    [DataMember(Order = 1, Name = "Should Loop")]
    public bool ShouldLoop { get; private set; }

    /// <summary>
    /// Gets or sets the volume.
    /// </summary>
    /// <value>The volume.</value>
    [DataMember(Order = 2, Name = "Volume")]
    public float Volume {
        get => this._volume;
        set {
            this._volume = value;
            this._instance.Volume = value;
        }
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.AudioClipReference.PropertyChanged -= this.AudioClipReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this._instance = this.AudioClipReference.InitializeAndGetInstance(this.Scene.Assets, this.Game, this.Volume, this.Pan, this.Pitch, this.ShouldLoop);
        this.AudioClipReference.PropertyChanged += this.AudioClipReference_PropertyChanged;

        if (this.ShouldLoop && this.IsEnabled && !BaseGame.IsDesignMode) {
            this.Play();
        }
    }

    /// <summary>
    /// Pauses this instance.
    /// </summary>
    public void Pause() {
        this._instance.Pause();
    }

    /// <summary>
    /// Play this instance.
    /// </summary>
    public void Play() {
        this._instance.Play();
    }

    /// <summary>
    /// Stop this instance.
    /// </summary>
    public void Stop() {
        this.Stop(true);
    }

    /// <summary>
    /// Stop this instance.
    /// </summary>
    /// <param name="isImmediate">If set to <c>true</c> is immediate.</param>
    public void Stop(bool isImmediate) {
        this._instance.Stop(isImmediate);
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.AudioClipReference;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(IEntity.IsEnabled)) {
            if (!this.IsEnabled) {
                this.Stop();
            }
            else if (this.ShouldLoop) {
                this.Play();
            }
        }
    }

    private void AudioClipReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.AudioClipReference.Asset)) {
            if (this.AudioClipReference.Asset is { } audioClip) {
                this._instance.Dispose();
                this._instance = audioClip.GetInstance(this.Game.AudioSettings, this.Volume, this.Pan, this.Pitch, this.ShouldLoop);
            }
        }
    }
}