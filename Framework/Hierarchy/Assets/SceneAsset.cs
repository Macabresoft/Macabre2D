namespace Macabresoft.Macabre2D.Framework {
    
    /// <summary>
    /// An asset which contains a <see cref="IGameScene"/>.
    /// </summary>
    public class SceneAsset : ContentAsset<GameScene> {
        
        /// <summary>
        /// The file extension for a serialized <see cref="GameScene"/>.
        /// </summary>
        public const string FileExtension = ".m2dscene";
    }
}