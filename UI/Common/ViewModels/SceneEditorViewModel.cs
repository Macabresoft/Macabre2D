namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using System;
    using System.Reactive;
    using System.Windows.Input;
    using Macabresoft.Macabre2D.Framework;
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
        private readonly IEntityService _entityService;

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
        /// <param name="entityService">The entity service.</param>
        /// <param name="sceneEditor">The scene editor.</param>
        /// <param name="sceneService">The scene service.</param>
        [InjectionConstructor]
        public SceneEditorViewModel(
            IEditorService editorService,
            IEntityService entityService,
            IAvaloniaGame sceneEditor,
            ISceneService sceneService) : base(sceneEditor) {
            this.EditorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            this._entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
            this.SceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));

            this.EditorService.CenterCameraRequested += this.EditorService_CenterCameraRequested;
            this.EditorService.FocusRequested += this.EntityService_FocusRequested;
        }

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
        /// Gets the scene service.
        /// </summary>
        public ISceneService SceneService { get; }

        private void EditorService_CenterCameraRequested(object sender, EventArgs e) {
            this.SceneEditor.Camera.LocalPosition = Vector2.Zero;
        }

        private void EntityService_FocusRequested(object sender, IEntity e) {
            if (e != null) {
                this.SceneEditor.Camera.LocalPosition = e.Transform.Position;
            }
        }
    }
}