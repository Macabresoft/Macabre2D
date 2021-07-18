namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using System;
    using System.Reactive;
    using System.Windows.Input;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop;
    using Macabresoft.Macabre2D.UI.Common.MonoGame;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Microsoft.Xna.Framework;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model that holds a <see cref="ISceneEditor" /> and handles interactions with it from
    /// the view.
    /// </summary>
    public sealed class SceneEditorViewModel : MonoGameViewModel {
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditorViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public SceneEditorViewModel() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditorViewModel" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="sceneEditor">The scene editor.</param>
        /// <exception cref="ArgumentNullException">editorService</exception>
        [InjectionConstructor]
        public SceneEditorViewModel(IEditorService editorService, IAvaloniaGame sceneEditor) : base(sceneEditor) {
            this.EditorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            this.CenterCameraCommand = ReactiveCommand.Create(this.CenterCamera);
            this.SetSelectedGizmoCommand = ReactiveCommand.Create<GizmoKind, Unit>(this.SetSelectedGizmo);
        }

        /// <summary>
        /// Gets a command that centers the <see cref="ISceneEditor" /> camera.
        /// </summary>
        /// <value>The center camera command.</value>
        public ICommand CenterCameraCommand { get; }

        /// <summary>
        /// Gets the editor service.
        /// </summary>
        public IEditorService EditorService { get; }

        /// <summary>
        /// Gets the scene editor.
        /// </summary>
        /// <value>The scene editor.</value>
        public ISceneEditor SceneEditor => this.Game as ISceneEditor;

        /// <summary>
        /// Gets a command to set the selected gizmo.
        /// </summary>
        public ICommand SetSelectedGizmoCommand { get; }

        private void CenterCamera() {
            this.SceneEditor.Camera.LocalPosition = Vector2.Zero;
        }

        private Unit SetSelectedGizmo(GizmoKind kind) {
            this.EditorService.SelectedGizmo = kind;
            return Unit.Default;
        }
    }
}