namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using System;
    using System.Linq;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// A component which selects entities and components based on their bounding areas.
    /// </summary>
    internal class SelectorComponent : GameComponent, IGizmo {
        private readonly IProjectService _projectService;
        private readonly ISelectionService _selectionService;
        private ICameraComponent _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorComponent" /> class.
        /// </summary>
        /// <param name="projectService">The project service.</param>
        /// <param name="selectionService">The selection service.</param>
        public SelectorComponent(IProjectService projectService, ISelectionService selectionService) : base() {
            this._projectService = projectService;
            this._selectionService = selectionService;
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            if (!this.Entity.TryGetComponent(out this._camera)) {
                throw new ArgumentNullException(nameof(this._camera));
            }
        }

        /// <inheritdoc />
        public GizmoKind GizmoKind => GizmoKind.Selector;
        
        /// <inheritdoc />
        public bool Update(FrameTime frameTime, InputState inputState) {
            var result = false;
            if (this._camera != null && 
                !GameScene.IsNullOrEmpty(this._projectService.CurrentScene) &&
                inputState.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                inputState.PreviousMouseState.LeftButton == ButtonState.Released) {
                
                var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);
                var selected = this._projectService.CurrentScene.RenderableComponents.FirstOrDefault(x => x.BoundingArea.Contains(mousePosition));

                if (this._selectionService.SelectedComponent != selected) {
                    if (selected == null) {
                        Dispatcher.UIThread.Post(() => this._selectionService.SelectedEntity = null);
                    }
                    else {
                        Dispatcher.UIThread.Post(() => this._selectionService.SelectedComponent = selected);
                    }
                }

                result = true;
            }

            return result;
        }
    }
}