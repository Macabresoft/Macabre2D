namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;

    public sealed class SceneAsset : MetadataAsset {

        [DataMember]
        private Camera _camera = new Camera();

        public SceneAsset(string name) : base(name) {
        }

        public SceneAsset() : this(string.Empty) {
        }

        public Camera Camera {
            get {
                return this._camera;
            }
        }

        public override AssetType Type {
            get {
                return AssetType.Scene;
            }
        }

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder) {
            var path = this.GetContentPath();
            contentStringBuilder.AppendLine($"#begin {path}");
            contentStringBuilder.AppendLine(@"/importer:SceneImporter");
            contentStringBuilder.AppendLine(@"/processor:SceneProcessor");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public IScene Load() {
            return new Serializer().Deserialize<Scene>(this.GetPath());
        }

        public override string ToString() {
            return Path.GetFileNameWithoutExtension(this.Name);
        }
    }
}