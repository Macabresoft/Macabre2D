namespace Macabre2D.UI.Models {

    using Macabre2D.Framework.Audio;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;

    public sealed class AudioAsset : MetadataAsset {

        public AudioAsset(string name) : base(name) {
            this.PropertyChanged += this.AudioAsset_PropertyChanged;
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

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder) {
            var path = this.GetContentPath();
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

        public override void Refresh() {
            this.AudioClip.ContentPath = Path.ChangeExtension(this.GetContentPath(), null);
            base.Refresh();
        }

        internal override void ResetContentPath(string newPath) {
            base.ResetContentPath(newPath);

            if (this.AudioClip != null) {
                this.AudioClip.ContentPath = newPath;
            }
        }

        private void AudioAsset_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Name)) {
                this.AudioClip.ContentPath = Path.ChangeExtension(this.GetContentPath(), null);
            }
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