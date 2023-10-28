namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// Settings for audio.
/// </summary>
[DataContract]
[Category(CommonCategories.Audio)]
public class AudioSettings {
    private float _effectVolume = 1f;
    private float _menuVolume = 1f;
    private float _musicVolume = 1f;
    private float _notificationVolume = 1f;
    private float _overallVolume = 1f;
    private float _voiceVolume = 1f;

    /// <summary>
    /// Raised when the volume is changed.
    /// </summary>
    public event EventHandler<VolumeCategory>? VolumeChanged;

    /// <summary>
    /// Gets or sets the volume for effects.
    /// </summary>
    [DataMember]
    public float EffectVolume {
        get => this._effectVolume;
        set {
            this._effectVolume = Math.Clamp(value, 0f, 1f);
            this.VolumeChanged.SafeInvoke(this, VolumeCategory.Effect);
        }
    }

    /// <summary>
    /// Gets or sets the volume for menus.
    /// </summary>
    [DataMember]
    public float MenuVolume {
        get => this._menuVolume;
        set {
            this._menuVolume = Math.Clamp(value, 0f, 1f);
            this.VolumeChanged.SafeInvoke(this, VolumeCategory.Menu);
        }
    }

    /// <summary>
    /// Gets or sets the volume for music.
    /// </summary>
    [DataMember]
    public float MusicVolume {
        get => this._musicVolume;
        set {
            this._musicVolume = Math.Clamp(value, 0f, 1f);
            this.VolumeChanged.SafeInvoke(this, VolumeCategory.Music);
        }
    }

    /// <summary>
    /// Gets or sets the volume for menus.
    /// </summary>
    [DataMember]
    public float NotificationVolume {
        get => this._notificationVolume;
        set {
            this._notificationVolume = Math.Clamp(value, 0f, 1f);
            this.VolumeChanged.SafeInvoke(this, VolumeCategory.Notification);
        }
    }

    /// <summary>
    /// Gets or sets the overall volume.
    /// </summary>
    [DataMember]
    public float OverallVolume {
        get => this._overallVolume;
        set {
            this._overallVolume = Math.Clamp(value, 0f, 1f);
            this.VolumeChanged.SafeInvoke(this, VolumeCategory.Default);
        }
    }

    /// <summary>
    /// Gets or sets the volume for voices.
    /// </summary>
    [DataMember]
    public float VoiceVolume {
        get => this._voiceVolume;
        set {
            this._voiceVolume = Math.Clamp(value, 0f, 1f);
            this.VolumeChanged.SafeInvoke(this, VolumeCategory.Voice);
        }
    }

    /// <summary>
    /// Changes the volume given an <see cref="VolumeCategory" /> and the amount to change..
    /// </summary>
    /// <param name="category">The category</param>
    /// <param name="amount">The amount to alter the volume.</param>
    public void ChangeCategoryVolume(VolumeCategory category, float amount) {
        var newVolume = this.GetCategoryVolume(category) + amount;
        this.SetCategoryVolume(category, newVolume);
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public AudioSettings Clone() {
        var settings = new AudioSettings();
        this.CopyTo(settings);
        return settings;
    }

    /// <summary>
    /// Copies audio settings to another instance.
    /// </summary>
    /// <param name="other">The other instance.</param>
    public void CopyTo(AudioSettings other) {
        other.EffectVolume = this.EffectVolume;
        other.MenuVolume = this.MenuVolume;
        other.MusicVolume = this.MusicVolume;
        other.NotificationVolume = this.NotificationVolume;
        other.OverallVolume = this.OverallVolume;
        other.VoiceVolume = this.VoiceVolume;
    }

    /// <summary>
    /// Gets the volume of an <see cref="VolumeCategory" />.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The volume.</returns>
    public float GetCategoryVolume(VolumeCategory category) {
        return category switch {
            VolumeCategory.Overall => this.OverallVolume,
            VolumeCategory.Effect => this.EffectVolume,
            VolumeCategory.Menu => this.MenuVolume,
            VolumeCategory.Music => this.MusicVolume,
            VolumeCategory.Notification => this.NotificationVolume,
            VolumeCategory.Voice => this.VoiceVolume,
            _ => 1f
        };
    }

    /// <summary>
    /// Gets the volume given an <see cref="VolumeCategory" /> and instance volume.
    /// </summary>
    /// <param name="category">The category</param>
    /// <param name="instanceVolume">The volume of the individual entity.</param>
    /// <returns>The volume.</returns>
    public float GetVolume(VolumeCategory category, float instanceVolume) {
        var volume = 0f;

        if (this.OverallVolume > 0f) {
            volume = instanceVolume * this.GetCategoryVolume(category) * this.OverallVolume;
        }

        return volume;
    }

    /// <summary>
    /// Sets the volume for a category. Used when editing sound settings.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="volume">The new volume.</param>
    public void SetCategoryVolume(VolumeCategory category, float volume) {
        switch (category) {
            case VolumeCategory.Overall:
                this.OverallVolume = volume;
                break;
            case VolumeCategory.Effect:
                this.EffectVolume = volume;
                break;
            case VolumeCategory.Menu:
                this.MenuVolume = volume;
                break;
            case VolumeCategory.Music:
                this.MusicVolume = volume;
                break;
            case VolumeCategory.Notification:
                this.NotificationVolume = volume;
                break;
            case VolumeCategory.Voice:
                this.VoiceVolume = volume;
                break;
        }
    }
}