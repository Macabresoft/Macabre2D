namespace Macabresoft.Macabre2D.Framework {
    using System.Text;

    /// <summary>
    /// An asset which contains a <see cref="IGameScene" />.
    /// </summary>
    public class SceneAsset : Asset<GameScene> {
        /// <summary>
        /// The file extension for a serialized <see cref="GameScene" />.
        /// </summary>
        public const string FileExtension = ".m2dscene";

        /// <inheritdoc />
        public override string GetContentBuildCommands(string contentPath, string fileExtension) {
            var contentStringBuilder = new StringBuilder();
            contentStringBuilder.AppendLine($"#begin {contentPath}");
            contentStringBuilder.AppendLine($@"/importer:{nameof(SceneImporter)}");
            contentStringBuilder.AppendLine($@"/processor:{nameof(SceneProcessor)}");
            contentStringBuilder.AppendLine($@"/build:{contentPath}");
            contentStringBuilder.AppendLine($"#end {contentPath}");
            return contentStringBuilder.ToString();
        }
    }
}