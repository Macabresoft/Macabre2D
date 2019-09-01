namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;

    public sealed class AudioAsset : MetadataAsset {

        public AudioAsset(string name) : base(name) {
        }

        public AudioAsset() : this(string.Empty) {
        }

        [DataMember]
        public AudioClip AudioClip { get; private set; } = new AudioClip();

        public override AssetType Type {
            get {
                return AssetType.Audio;
            }
        }

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder, string projectDirectoryPath) {
            var path = Path.Combine(projectDirectoryPath, this.GetContentPath());
            contentStringBuilder.AppendLine($"#begin {path}");
            contentStringBuilder.AppendLine($@"/importer:{this.GetImporterName()}");
            contentStringBuilder.AppendLine(@"/processor:SoundEffectProcessor");
            contentStringBuilder.AppendLine(@"/processorParam:Quality=Best");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public override void Delete() {
            this.RemoveIdentifiableContentFromScenes(this.AudioClip.Id);
            base.Delete();
        }

        private string GetImporterName() {
            var result = string.Empty;
            if (this.Name.ToUpper().EndsWith(".WAV")) {
                result = "WavImporter";
            }
            else if (this.Name.ToUpper().EndsWith(".MP3")) {
                result = "Mp3Importer";
            }
            else if (this.Name.ToUpper().EndsWith(".WMA")) {
                result = "WmaImporter";
            }
            else {
                throw new NotSupportedException("You have an audio asset with an unsupported file extension. Please use .mp3, .wav, or .wma.");
            }

            return result;
        }
    }
}