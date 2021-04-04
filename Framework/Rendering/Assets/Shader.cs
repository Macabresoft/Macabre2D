namespace Macabresoft.Macabre2D.Framework {
    using System.Text;
    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Processors;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A shader that wraps around <see cref="Effect" />.
    /// </summary>
    public sealed class Shader : Asset<Effect> {
        
        /// <inheritdoc />
        public override bool IncludeFileExtensionInContentPath => false;
        
        /// <inheritdoc />
        public override string GetContentBuildCommands(string contentPath, string fileExtension) {
            var contentStringBuilder = new StringBuilder();
            contentStringBuilder.AppendLine($"#begin {contentPath}");
            contentStringBuilder.AppendLine($@"/importer:{nameof(EffectImporter)}");
            contentStringBuilder.AppendLine($@"/processor:{nameof(EffectProcessor)}");
            contentStringBuilder.AppendLine(@"/processorParam:DebugMode = Auto");
            contentStringBuilder.AppendLine($@"/build:{contentPath}{fileExtension}");
            contentStringBuilder.AppendLine($"#end {contentPath}");
            return contentStringBuilder.ToString();
        }
    }
}