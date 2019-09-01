namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;

    public sealed class SceneAsset : AddableAsset<Scene> {

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

        public override string FileExtension {
            get {
                return FileHelper.SceneExtension;
            }
        }

        public override AssetType Type {
            get {
                return AssetType.Scene;
            }
        }

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder, string projectDirectoryPath) {
            var path = Path.Combine(projectDirectoryPath, this.GetContentPath());
            contentStringBuilder.AppendLine($"#begin {path}");
            contentStringBuilder.AppendLine(@"/importer:SceneImporter");
            contentStringBuilder.AppendLine(@"/processor:SceneProcessor");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public override string ToString() {
            return Path.GetFileNameWithoutExtension(this.Name);
        }

        internal override void ResetContentPath() {
            base.ResetContentPath();

            if (this.SavableValue is Scene scene) {
                scene.Name = this.Name;
            }
        }

        protected override void SaveChanges(Serializer serializer) {
            this.SavableValue.SaveToFile(this.GetPath(), serializer);
        }
    }
}