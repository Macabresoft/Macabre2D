namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using MahApps.Metro.IconPacks;
    using System.IO;
    using System.Text;

    public sealed class ShaderAsset : MetadataAsset {

        public ShaderAsset(string name) : base(name) {
        }

        public ShaderAsset() : this(string.Empty) {
        }

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.ImageFilter;
            }
        }

        public Shader Shader { get; private set; } = new Shader();

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder, string projectDirectoryPath) {
            var path = Path.Combine(projectDirectoryPath, this.GetContentPath());
            contentStringBuilder.AppendLine($"#begin {path}");
            contentStringBuilder.AppendLine(@"/importer:EffectImporter");
            contentStringBuilder.AppendLine(@"/processor:EffectProcessor");
            contentStringBuilder.AppendLine(@"/processorParam:DebugMode = Auto");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public override void Delete() {
            this.RemoveIdentifiableContentFromScenes(this.Shader.Id);
            base.Delete();
        }
    }
}