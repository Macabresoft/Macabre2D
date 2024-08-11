namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

/// <summary>
/// A collection of <see cref="AudioClip" /> references that can be played at random.
/// </summary>
public class AudioClipReferenceQueue : AssetReferenceCollection<AudioClip, SoundEffect> {
    private readonly Queue<Guid> _contentIdQueue = new();
    private readonly Dictionary<Guid, IAudioClipInstance> _contentIdToAudioClipInstance = new();

    /// <inheritdoc />
    public override void Clear() {
        base.Clear();
        this.ClearContent();
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.ClearContent();
    }

    /// <inheritdoc />
    public override void Initialize(IAssetManager assetManager, IGame game) {
        base.Initialize(assetManager, game);

        this._contentIdQueue.Clear();
        this._contentIdToAudioClipInstance.Clear();

        foreach (var asset in this.Assets) {
            this._contentIdToAudioClipInstance[asset.ContentId] = asset.GetInstance(game.AudioSettings);
            this._contentIdQueue.Enqueue(asset.ContentId);
        }
    }


    /// <summary>
    /// Plays the next item in the queue.
    /// </summary>
    public void PlayNext() {
        if (this._contentIdQueue.Count > 0) {
            var contentId = this._contentIdQueue.Dequeue();

            if (this._contentIdToAudioClipInstance.TryGetValue(contentId, out var instance)) {
                this._contentIdQueue.Enqueue(contentId);
                instance.Play();
            }
            else {
                this.PlayNext();
            }
        }
    }

    private void ClearContent() {
        foreach (var instance in this._contentIdToAudioClipInstance.Values) {
            instance.Dispose();
        }

        this._contentIdQueue.Clear();
        this._contentIdToAudioClipInstance.Clear();
    }
}