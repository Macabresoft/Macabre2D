namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Text;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content.Pipeline.Processors;

    /// <summary>
    /// A single audio clip.
    /// </summary>
    public sealed class AudioClip : Asset<SoundEffect> {
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
        /// Gets a sound effect instance.
        /// </summary>
        /// <param name="volume">The volume.</param>
        /// <param name="pan">The panning.</param>
        /// <param name="pitch">The pitch.</param>
        /// <returns>A sound effect instance.</returns>
        public SoundEffectInstance? GetSoundEffectInstance(float volume, float pan, float pitch) {
            SoundEffectInstance? instance;
            if (this.Content is SoundEffect soundEffect) {
                instance = soundEffect.CreateInstance();
                instance.Volume = volume;
                instance.Pan = pan;
                instance.Pitch = pitch;
            }
            else {
                instance = null;
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
}