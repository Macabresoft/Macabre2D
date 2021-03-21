namespace Macabresoft.Macabre2D.Framework {
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A shader that wraps around <see cref="Effect" />.
    /// </summary>
    public sealed class Shader : Asset<Effect> {
        /// <inheritdoc />
        public override string GetContentBuildCommands(string contentPath, string fileExtension) {
            var contentStringBuilder = new StringBuilder();
            contentStringBuilder.AppendLine($"#begin {contentPath}");
            contentStringBuilder.AppendLine(@"/importer:EffectImporter");
            contentStringBuilder.AppendLine(@"/processor:EffectProcessor");
            contentStringBuilder.AppendLine(@"/processorParam:DebugMode = Auto");
            contentStringBuilder.AppendLine($@"/build:{contentPath}");
            return contentStringBuilder.ToString();
        }
    }
}