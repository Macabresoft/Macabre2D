namespace Macabresoft.Macabre2D.UI.Common.MonoGame.Entities {
    using System;
    using System.Linq;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// A component which selects entities and components based on their bounding areas.
    /// </summary>
    internal class SelectorGizmo : Entity, IGizmo {
        private readonly ISceneService _sceneService;
        private readonly IEntityService _entityService;
        private ICamera _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorGizmo" /> class.
        /// </summary>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="entityService">The selection service.</param>
        public SelectorGizmo(ISceneService sceneService, IEntityService entityService) : base() {
            this._sceneService = sceneService;
            this._entityService = entityService;
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

                if (this._entityService.Selected != selected) {
                    if (selected == null) {
                        Dispatcher.UIThread.Post(() => this._entityService.Selected = null);
                    }
                    else {
                        Dispatcher.UIThread.Post(() => this._entityService.Selected = selected);
                    }
                }

                result = true;
            }

            return result;
        }
    }
}