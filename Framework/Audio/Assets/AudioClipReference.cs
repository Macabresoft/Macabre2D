namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework.Audio;

/// <summary>
/// A reference to an <see cref="AudioClipAsset" />.
/// </summary>
public class AudioClipReference : AssetReference<AudioClipAsset, SoundEffect> {
    /// <summary>
    /// Initializes the audio clip and gets an instance.
    /// </summary>
    /// <param name="assets">The asset manager.</param>
    /// <returns>An audio clip instance.</returns>
    public IAudioClipInstance InitializeAndGetInstance(IAssetManager assets) {
        this.Initialize(assets);
        return this.Asset?.GetInstance() ?? AudioClipInstance.Empty;
    }

    /// <summary>
    /// Initializes the audio clip and gets an instance.
    /// </summary>
    /// <param name="assets">The asset manager.</param>
    /// <param name="volume">The volume.</param>
    /// <param name="pan">The panning.</param>
    /// <param name="pitch">The pitch.</param>
    /// <param name="shouldLoop">A value indicating whether or not the instance should loop.</param>
    /// <returns>An audio clip instance.</returns>
    public IAudioClipInstance InitializeAndGetInstance(IAssetManager assets, float volume, float pan, float pitch, bool shouldLoop) {
        this.Initialize(assets);
        return this.Asset?.GetInstance(volume, pan, pitch, shouldLoop) ?? AudioClipInstance.Empty;
    }
}