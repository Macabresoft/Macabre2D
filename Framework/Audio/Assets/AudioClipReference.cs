﻿namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework.Audio;

/// <summary>
/// A reference to an <see cref="AudioClip" />.
/// </summary>
public class AudioClipReference : AssetReference<AudioClip, SoundEffect> {

    /// <summary>
    /// Gets the audio instance.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns>A new instance.</returns>
    public IAudioClipInstance GetInstance(AudioSettings settings) => this.Asset?.GetInstance(settings) ?? AudioClipInstance.Empty;

    /// <summary>
    /// Initializes the audio clip and gets an instance.
    /// </summary>
    /// <param name="assets">The asset manager.</param>
    /// <param name="game">The game.</param>
    /// <returns>An audio clip instance.</returns>
    public IAudioClipInstance InitializeAndGetInstance(IAssetManager assets, IGame game) {
        this.Initialize(assets, game);
        return this.GetInstance(game.AudioSettings);
    }

    /// <summary>
    /// Initializes the audio clip and gets an instance.
    /// </summary>
    /// <param name="assets">The asset manager.</param>
    /// <param name="game">The game.</param>
    /// <param name="volume">The volume.</param>
    /// <param name="pan">The panning.</param>
    /// <param name="pitch">The pitch.</param>
    /// <param name="shouldLoop">A value indicating whether or not the instance should loop.</param>
    /// <returns>An audio clip instance.</returns>
    public IAudioClipInstance InitializeAndGetInstance(
        IAssetManager assets,
        IGame game,
        float volume,
        float pan,
        float pitch,
        bool shouldLoop) {
        this.Initialize(assets, game);
        return this.Asset?.GetInstance(game.AudioSettings, volume, pan, pitch, shouldLoop) ?? AudioClipInstance.Empty;
    }
}