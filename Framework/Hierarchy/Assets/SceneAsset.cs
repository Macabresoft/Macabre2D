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
        public override bool IncludeFileExtensionInContentPath => true;
    }
}