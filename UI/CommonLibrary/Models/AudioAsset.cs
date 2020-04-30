namespace Macabre2D.UI.CommonLibrary.Models {

    using Macabre2D.Framework;
    using MahApps.Metro.IconPacks;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    public sealed class AudioAsset : MetadataAsset, ISyncAsset<AudioClip>, IIdentifiableAsset {

        public AudioAsset(string name) : base(name) {
        }

        public AudioAsset() : this(string.Empty) {
        }

        [DataMember]
        public AudioClip AudioClip { get; private set; } = new AudioClip();

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.FileMusic;
            }
        }

        public override bool IsContent {
            get {
                return true;
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

        public IEnumerable<AudioClip> GetAssetsToSync() {
            return new[] { this.AudioClip };
        }

        public IEnumerable<Guid> GetOwnedAssetIds() {
            return new[] { this.AudioClip.Id };
        }

        public override void Refresh(AssetManager assetManager) {
            this.AudioClip.Id = this.Id;
            base.Refresh(assetManager);
            assetManager.SetMapping(this.Id, this.GetContentPathWithoutExtension());
        }

        private string GetImporterName() {
            string result;
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