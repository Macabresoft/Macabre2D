namespace Macabresoft.Macabre2D.Framework {
    using System.Text;

    /// <summary>
    /// An asset which contains a <see cref="IScene" />.
    /// </summary>
    public class SceneAsset : Asset<Scene> {
        /// <summary>
        /// The file extension for a serialized <see cref="Scene" />.
        /// </summary>
        public const string FileExtension = ".m2dscene";
        
        /// <inheritdoc />
        public override bool IncludeFileExtensionInContentPath => true;
    }
}