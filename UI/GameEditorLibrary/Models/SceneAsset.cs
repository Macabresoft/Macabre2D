namespace Macabre2D.UI.GameEditorLibrary.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using MahApps.Metro.IconPacks;
    using Microsoft.Xna.Framework;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;

    public sealed class SceneAsset : AddableAsset<Scene> {

        [DataMember]
        private readonly Camera _camera = new Camera();

        public SceneAsset(string name) : base(name) {
        }

        public SceneAsset() : this(string.Empty) {
        }

        public Color BackgroundColor {
            get {
                return this.SavableValue.BackgroundColor;
            }

            set {
                if (this.SavableValue.BackgroundColor != value) {
                    this.SavableValue.BackgroundColor = value;
                    this.RaisePropertyChanged();
                }
            }
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

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.FileCloud;
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

        protected override void SaveChanges() {
            this.SavableValue.SaveToFile(this.GetPath());
        }
    }
}