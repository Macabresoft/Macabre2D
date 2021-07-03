namespace Macabresoft.Macabre2D.UI.Common.MonoGame.Entities {
    using Avalonia.Input;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using MouseButton = Macabresoft.Macabre2D.Framework.MouseButton;

    /// <summary>
    /// A gizmo/component that allows the user to scale entities in the editor.
    /// </summary>
    internal sealed class ScaleGizmo : BaseAxisGizmo {
        private readonly IUndoService _undoService;
        private Texture2D _squareSprite;
        private Vector2 _unmovedScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleGizmo" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="entitySelectionService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        public ScaleGizmo(
            IEditorService editorService,
            ISceneService sceneService,
            IEntitySelectionService entitySelectionService,
            IUndoService undoService) : base(editorService, sceneService, entitySelectionService) {
            this._undoService = undoService;
        }

        /// <inheritdoc />
        public override GizmoKind GizmoKind => GizmoKind.Scale;

        /// <inheritdoc />
        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);
            if (this.Scene.Game.GraphicsDevice is GraphicsDevice graphicsDevice) {
                this._squareSprite = PrimitiveDrawer.CreateQuadSprite(graphicsDevice, new Point(GizmoPointSize));
            }
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                var settings = this.Scene.Game.Project.Settings;
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                var shadowOffset = lineThickness * settings.InversePixelsPerUnit;
                var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

                var viewRatio = settings.GetPixelAgnosticRatio(viewBoundingArea.Height, this.Scene.Game.ViewportSize.Y);
                var scale = new Vector2(viewRatio);
                var offset = viewRatio * GizmoPointSize * settings.InversePixelsPerUnit * 0.5f; // The extra 0.5f is to center it
                var pixelsPerUnit = this.Scene.Game.Project.Settings.PixelsPerUnit;
                spriteBatch.Draw(
                    pixelsPerUnit,
                    this._squareSprite,
                    this.XAxisPosition - new Vector2(offset) + shadowOffsetVector,
                    scale,
                    0f,
                    this.EditorService.DropShadowColor);

                spriteBatch.Draw(
                    pixelsPerUnit,
                    this._squareSprite,
                    this.YAxisPosition - new Vector2(offset) + shadowOffsetVector,
                    scale,
                    0f,
                    this.EditorService.DropShadowColor);

                base.Render(frameTime, viewBoundingArea);

                spriteBatch.Draw(
                    pixelsPerUnit,
                    this._squareSprite,
                    this.XAxisPosition - new Vector2(offset),
                    scale,
                    0f,
                    this.EditorService.XAxisColor);

                spriteBatch.Draw(
                    pixelsPerUnit,
                    this._squareSprite,
                    this.YAxisPosition - new Vector2(offset),
                    scale,
                    0f,
                    this.EditorService.YAxisColor);
            }
        }

        /// <inheritdoc />
        public override bool Update(FrameTime frameTime, InputState inputState) {
            var result = base.Update(frameTime, inputState);
            var mousePosition = this.Camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);

            if (inputState.IsButtonNewlyPressed(MouseButton.Left)) {
                var axis = this.GetAxisUnderMouse(mousePosition);
                if (axis == GizmoAxis.X || axis == GizmoAxis.Y) {
                    result = true;
                    this.StartDrag(axis);
                }
            }
            else if (this.CurrentAxis != GizmoAxis.None) {
                var entity = this.EntitySelectionService.SelectedEntity;
                if (inputState.IsButtonHeld(MouseButton.Left)) {
                    var lineLength = this.GetAxisLength();
                    var newPosition = mousePosition;
                    // TODO: snap to grid
                    // var snapToAxis = inputState.CurrentKeyboardState.IsKeyDown(Keys.LeftControl) || inputState.CurrentKeyboardState.IsKeyDown(Keys.RightControl);

                    if (this.CurrentAxis == GizmoAxis.X) {
                        newPosition = this.MoveAlongAxis(this.NeutralAxisPosition, this.XAxisPosition, mousePosition);
                        this.XAxisPosition = newPosition;
                    }
                    else if (this.CurrentAxis == GizmoAxis.Y) {
                        newPosition = this.MoveAlongAxis(this.NeutralAxisPosition, this.YAxisPosition, mousePosition);
                        this.YAxisPosition = newPosition;
                    }

                    var newLineLength = Vector2.Distance(newPosition, this.NeutralAxisPosition);
                    var multiplier = this.GetScaleSign(newPosition, lineLength) * newLineLength / lineLength;
                    var newScale = this._unmovedScale;

                    if (inputState.CurrentKeyboardState.IsKeyDown(Keys.LeftShift)) {
                        newScale *= multiplier;
                    }
                    else {
                        newScale = this.CurrentAxis == GizmoAxis.X ? new Vector2(newScale.X * multiplier, newScale.Y) : new Vector2(newScale.X, newScale.Y * multiplier);
                    }

                    UpdateScale(entity, newScale);
                    result = true;
                }
                else {
                    this.CurrentAxis = GizmoAxis.None;
                    this.SetCursor(StandardCursorType.None);

                    var scale = entity.Transform.Scale;
                    var unmovedScale = this._unmovedScale;

                    var originalHasChanges = this.SceneService.HasChanges;
                    this._undoService.Do(
                        () => {
                            UpdateScale(entity, scale);
                            this.SceneService.HasChanges = true;
                        },
                        () => {
                            UpdateScale(entity, unmovedScale);
                            this.SceneService.HasChanges = originalHasChanges;
                        }, UndoScope.Scene);
                }
            }
            else {
                var axis = this.GetAxisUnderMouse(mousePosition);
                this.SetCursor(axis == GizmoAxis.None || axis == GizmoAxis.Neutral ? StandardCursorType.None : StandardCursorType.Hand);
            }

            return result;
        }

        /// <inheritdoc />
        protected override bool ShouldBeEnabled() {
            return this.EntitySelectionService.SelectedEntity != null && base.ShouldBeEnabled();
        }

        private float GetScaleSign(Vector2 dragPosition, float lineLength) {
            var dragStartPoint = this.NeutralAxisPosition + (this.CurrentAxis == GizmoAxis.X ? new Vector2(lineLength, 0f) : new Vector2(0f, lineLength));
            var dragDistanceFromComponent = Vector2.Distance(dragPosition, this.NeutralAxisPosition);
            var totalDragDistance = Vector2.Distance(dragPosition, dragStartPoint);
            return totalDragDistance > lineLength && dragDistanceFromComponent < totalDragDistance ? -1f : 1f;
        }

        private void StartDrag(GizmoAxis axis) {
            this._unmovedScale = this.EntitySelectionService.SelectedEntity.Transform.Scale;
            this.CurrentAxis = axis;

            if (this.CurrentAxis == GizmoAxis.X) {
                this.SetCursor(StandardCursorType.SizeWestEast);
            }
            else if (this.CurrentAxis == GizmoAxis.Y) {
                this.SetCursor(StandardCursorType.SizeNorthSouth);
            }
        }

        private static void UpdateScale(ITransformable entity, Vector2 newScale) {
            entity.SetWorldScale(newScale);
        }
    }
}