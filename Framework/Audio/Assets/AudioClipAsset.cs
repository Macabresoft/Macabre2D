namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

/// <summary>
/// A single audio clip.
/// </summary>
public sealed class AudioClipAsset : Asset<SoundEffect> {
    /// <summary>
    /// The valid file extensions for a <see cref="SoundEffectInstance" />.
    /// </summary>
    public static readonly string[] ValidFileExtensions = { ".wav", ".mp3", ".wma" };

    /// <inheritdoc />
    public override bool IncludeFileExtensionInContentPath => false;

    /// <inheritdoc />
    public override string GetContentBuildCommands(string contentPath, string fileExtension) {
        var contentStringBuilder = new StringBuilder();
        contentStringBuilder.AppendLine($"#begin {contentPath}");
        contentStringBuilder.AppendLine($@"/importer:{GetImporterName(fileExtension)}");
        contentStringBuilder.AppendLine($@"/processor:{nameof(SoundEffectProcessor)}");
        contentStringBuilder.AppendLine(@"/processorParam:Quality=Best");
        contentStringBuilder.AppendLine($@"/build:{contentPath}{fileExtension}");
        contentStringBuilder.AppendLine($"#end {contentPath}");
        return contentStringBuilder.ToString();
    }

    /// <summary>
    /// Gets an audio clip instance for the loaded sound effect..
    /// </summary>
    /// <param name="volume">The volume.</param>
    /// <param name="pan">The panning.</param>
    /// <param name="pitch">The pitch.</param>
    /// <param name="shouldLoop">A value indicating whether or not the instance should loop.</param>
    /// <returns>An audio clip instance.</returns>
    public IAudioClipInstance GetInstance(float volume, float pan, float pitch, bool shouldLoop) {
        var instance = AudioClipInstance.Empty;
        if (this.Content is { } soundEffect) {
            instance = new AudioClipInstance(soundEffect.CreateInstance());
            instance.Volume = volume;
            instance.Pan = pan;
            instance.Pitch = pitch;
            instance.ShouldLoop = shouldLoop;
        }

        return instance;
    }

    private static string GetImporterName(string fileExtension) {
        // TODO: can we support OGG? I'd like to do that just to be cool.
        return fileExtension.ToUpper() switch {
            ".WAV" => "WavImporter",
            ".MP3" => "Mp3Importer",
            ".WMA" => "WmaImporter",
            _ => throw new NotSupportedException("You have an audio asset with an unsupported file extension. Please use .mp3, .wav, or .wma.")
        };
    }
}