namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Entities {
    using System;
    using System.Linq;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// A component which selects entities and components based on their bounding areas.
    /// </summary>
    internal class SelectorGizmo : Entity, IGizmo {
        private readonly ISceneService _sceneService;
        private readonly ISelectionService _selectionService;
        private ICamera _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorGizmo" /> class.
        /// </summary>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="selectionService">The selection service.</param>
        public SelectorGizmo(ISceneService sceneService, ISelectionService selectionService) : base() {
            this._sceneService = sceneService;
            this._selectionService = selectionService;
        }

        /// <inheritdoc />
        public GizmoKind GizmoKind => GizmoKind.Selector;

        /// <inheritdoc />
        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);

            if (!this.TryGetParentEntity(out this._camera)) {
                throw new NotSupportedException("Could not find a camera ancestor.");
            }
        }

        /// <inheritdoc />
        public bool Update(FrameTime frameTime, InputState inputState) {
            var result = false;
            if (this._camera != null &&
                !Framework.Scene.IsNullOrEmpty(this._sceneService.CurrentScene) &&
                inputState.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                inputState.PreviousMouseState.LeftButton == ButtonState.Released) {
                var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);
                var selected = this._sceneService.CurrentScene.RenderableEntities.FirstOrDefault(x => x.BoundingArea.Contains(mousePosition));

                if (this._selectionService.SelectedEntity != selected) {
                    if (selected == null) {
                        Dispatcher.UIThread.Post(() => this._selectionService.SelectedEntity = null);
                    }
                    else {
                        Dispatcher.UIThread.Post(() => this._selectionService.SelectedEntity = selected);
                    }
                }

                result = true;
            }

            return result;
        }
    }
}