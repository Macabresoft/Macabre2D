namespace Macabresoft.Macabre2D.Editor.Framework.ViewModels {

    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Editor.Framework.MonoGame;
    using Microsoft.Xna.Framework;
    using ReactiveUI;
    using System;
    using System.Windows.Input;

    /// <summary>
    /// A view model that holds a <see cref="ISceneEditor" /> and handles interactions with it from
    /// the view.
    /// </summary>
    public sealed class SceneEditorViewModel : MonoGameViewModel {

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditorViewModel" /> class.
        /// </summary>
        /// <param name="sceneEditor">The scene editor.</param>
        /// <exception cref="ArgumentNullException">sceneEditor</exception>
        public SceneEditorViewModel(ISceneEditor sceneEditor) : base(sceneEditor) {
            this.CenterCameraCommand = ReactiveCommand.Create(this.CenterCamera);
        }

        /// <summary>
        /// Gets a command that centers the <see cref="ISceneEditor" /> camera.
        /// </summary>
        /// <value>The center camera command.</value>
        public ICommand CenterCameraCommand { get; }

        /// <summary>
        /// Gets the scene editor.
        /// </summary>
        /// <value>The scene editor.</value>
        public ISceneEditor SceneEditor {
            get {
                return this.Game as ISceneEditor;
            }
        }

        private void CenterCamera() {
            this.SceneEditor.Camera.Entity.LocalPosition = Vector2.Zero;
        }
    }
}