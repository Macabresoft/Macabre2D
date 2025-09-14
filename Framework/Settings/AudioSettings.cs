namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Newtonsoft.Json;

/// <summary>
/// Settings for audio.
/// </summary>
[DataContract]
[Category(CommonCategories.Audio)]
public class AudioSettings : CopyableSettings<AudioSettings> {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<VolumeCategory, float> _categoryToVolume = new();

    /// <summary>
    /// Raised when the volume is changed.
    /// </summary>
    public event EventHandler<VolumeCategory>? VolumeChanged;

    /// <summary>
    /// Changes the volume given an <see cref="VolumeCategory" /> and the amount to change..
    /// </summary>
    /// <param name="category">The category</param>
    /// <param name="amount">The amount to alter the volume.</param>
    public void ChangeCategoryVolume(VolumeCategory category, float amount) {
        var newVolume = this.GetCategoryVolume(category) + amount;
        this.SetCategoryVolume(category, newVolume);
    }

    /// <inheritdoc />
    public override void CopyTo(AudioSettings other) {
        other._categoryToVolume.Clear();
        other._categoryToVolume.AddRange(this._categoryToVolume);
    }

    /// <summary>
    /// Gets the volume of an <see cref="VolumeCategory" />.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>The volume.</returns>
    public float GetCategoryVolume(VolumeCategory category) => this._categoryToVolume.GetValueOrDefault(category, 1f);

    /// <summary>
    /// Gets the volume given an <see cref="VolumeCategory" /> and instance volume.
    /// </summary>
    /// <param name="category">The category</param>
    /// <param name="instanceVolume">The volume of the individual entity.</param>
    /// <returns>The volume.</returns>
    public float GetVolume(VolumeCategory category, float instanceVolume) {
        var volume = 0f;

        var overallVolume = this.GetCategoryVolume(VolumeCategory.Overall);
        if (overallVolume > 0f) {
            volume = Math.Clamp(instanceVolume * this.GetCategoryVolume(category) * overallVolume, 0f, 1f);
        }

        return volume;
    }

    /// <summary>
    /// Sets the volume for a category. Used when editing sound settings.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="volume">The new volume.</param>
    public void SetCategoryVolume(VolumeCategory category, float volume) {
        this._categoryToVolume[category] = Math.Clamp(volume, 0f, 1f);
        this.VolumeChanged.SafeInvoke(this, category);
    }
}