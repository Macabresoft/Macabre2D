namespace Macabre2D.UI.CommonLibrary.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.Resources.Properties;
    using MahApps.Metro.IconPacks;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public sealed class ShaderAsset : AddableAsset<Shader>, IReloadableAsset, IIdentifiableAsset {

        public ShaderAsset(string name) : base(name) {
        }

        public ShaderAsset() : this(string.Empty) {
        }

        public override string FileExtension {
            get {
                return FileHelper.ShaderExtension;
            }
        }

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.ImageFilter;
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
            contentStringBuilder.AppendLine(@"/importer:EffectImporter");
            contentStringBuilder.AppendLine(@"/processor:EffectProcessor");
            contentStringBuilder.AppendLine(@"/processorParam:DebugMode = Auto");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public override void Delete() {
            this.RemoveIdentifiableContentFromScenes(this.SavableValue.Id);
            base.Delete();
        }

        public IEnumerable<Guid> GetOwnedAssetIds() {
            return new[] { this.Id };
        }

        public override void Refresh(AssetManager assetManager) {
            base.Refresh(assetManager);
        }

        public void Reload() {
            this.SavableValue?.Load();
        }

        protected override Shader CreateAsset() {
            var path = this.GetPath();
            if (!File.Exists(path)) {
                File.WriteAllText(this.GetPath(), Resources.DefaultShader);
            }

            return new Shader(this.Id);
        }

        protected override Shader GetInitialSavableValue() {
            return this.CreateAsset();
        }

        protected override void SaveChanges() {
            this.SavableValue.AssetId = this.Id;
        }
    }
}