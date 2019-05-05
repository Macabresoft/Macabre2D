namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Serialization;
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

        public IScene Scene { get; private set; }

        public override AssetType Type {
            get {
                return AssetType.Scene;
            }
        }

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder, string contentPath) {
            var path = Path.Combine(contentPath, this.GetContentPath());
            contentStringBuilder.AppendLine($"#begin {path}");
            contentStringBuilder.AppendLine(@"/importer:SceneImporter");
            contentStringBuilder.AppendLine(@"/processor:SceneProcessor");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public IScene Load() {
            if (this.Scene == null) {
                this.Scene = new Serializer().Deserialize<Scene>(this.GetPath());
            }

            return this.Scene;
        }

        public override string ToString() {
            return Path.GetFileNameWithoutExtension(this.Name);
        }

        internal override void ResetContentPath() {
            base.ResetContentPath();

            if (this.Scene is Scene scene) {
                scene.Name = this.Name;
            }
        }
    }
}