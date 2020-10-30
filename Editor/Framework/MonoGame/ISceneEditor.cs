namespace Macabresoft.Macabre2D.Editor.Framework.MonoGame {

    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// An extension of <see cref="IAvaloniaGame" /> that makes editing a Macabre2D <see
    /// cref="IGameScene" /> easier.
    /// </summary>
    public interface ISceneEditor : IAvaloniaGame {

        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>The camera.</value>
        public ICameraComponent Camera { get; }

        /// <summary>
        /// Gets or sets the scene to edit.
        /// </summary>
        /// <value>The scene to edit.</value>
        public IGameScene SceneToEdit { get; set; }
    }
}