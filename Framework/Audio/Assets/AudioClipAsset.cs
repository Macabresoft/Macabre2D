namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;
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

    private float _defaultPan;
    private float _defaultPitch;
    private bool _defaultShouldLoop;
    private float _defaultVolume = 1f;

    /// <inheritdoc />
    public override bool IncludeFileExtensionInContentPath => false;


    /// <summary>
    /// Gets or sets the default pan.
    /// </summary>
    [Category("Defaults")]
    [DataMember(Order = 3)]
    public float DefaultPan {
        get => this._defaultPan;
        set => this.Set(ref this._defaultPan, MathHelper.Clamp(value, -1f, 1f));
    }

    /// <summary>
    /// Gets or sets the default pitch.
    /// </summary>
    [Category("Defaults")]
    [DataMember(Order = 2)]
    public float DefaultPitch {
        get => this._defaultPitch;
        set => this.Set(ref this._defaultPitch, MathHelper.Clamp(value, -1f, 1f));
    }

    /// <summary>
    /// Gets or sets the default volume.
    /// </summary>
    [Category("Defaults")]
    [DataMember(Order = 0)]
    public bool DefaultShouldLoop {
        get => this._defaultShouldLoop;
        set => this.Set(ref this._defaultShouldLoop, value);
    }

    /// <summary>
    /// Gets or sets the default volume.
    /// </summary>
    [Category("Defaults")]
    [DataMember(Order = 1)]
    public float DefaultVolume {
        get => this._defaultVolume;
        set => this.Set(ref this._defaultVolume, MathHelper.Clamp(value, -1f, 1f));
    }

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
    /// Gets an audio clip instance for the loaded sound effect.
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

    /// <summary>
    /// Gets an audio clip instance for the loaded sound effect.
    /// </summary>
    /// <returns>An audio clip instance.</returns>
    public IAudioClipInstance GetInstance() {
        return this.GetInstance(this.DefaultVolume, this.DefaultPan, this.DefaultPitch, this.DefaultShouldLoop);
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