namespace Macabresoft.Macabre2D.UI.Common.MonoGame.Entities {
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A gizmo for rotating <see cref="IRotatable"/> entities in the scene editor.
    /// </summary>
    public class RotationGizmo : BaseAxisGizmo {
        private readonly IUndoService _undoService;
        private Texture2D _sprite;


        /// <summary>
        /// Initializes a new instance of the <see cref="RotationGizmo" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="entityService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        public RotationGizmo(IEditorService editorService, IEntityService entityService, ISceneService sceneService, IUndoService undoService) : base(editorService, sceneService, entityService) {
            this._undoService = undoService;
        }

        /// <inheritdoc />
        public override GizmoKind GizmoKind => GizmoKind.Rotation;

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this.PrimitiveDrawer is PrimitiveDrawer drawer) {
                var settings = this.Scene.Game.Project.Settings;
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                var shadowOffset = lineThickness * settings.InversePixelsPerUnit;
                var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

                var viewRatio = settings.GetPixelAgnosticRatio(viewBoundingArea.Height, this.Scene.Game.ViewportSize.Y);
                var scale = new Vector2(viewRatio);
                var offset = viewRatio * GizmoPointSize * settings.InversePixelsPerUnit * 0.5f; // The extra 0.5f is to center it
                var axisLength = this.GetAxisLength();
                
                drawer.DrawCircle(
                    spriteBatch, 
                    settings.PixelsPerUnit, 
                    axisLength, 
                    this.NeutralAxisPosition + shadowOffsetVector, 
                    32, 
                    this.EditorService.DropShadowColor, 
                    lineThickness);

                spriteBatch.Draw(
                    settings.PixelsPerUnit,
                    this._sprite,
                    this.XAxisPosition - new Vector2(offset) + shadowOffsetVector,
                    scale,
                    0f,
                    this.EditorService.DropShadowColor);
                
                drawer.DrawCircle(
                    spriteBatch, 
                    settings.PixelsPerUnit, 
                    axisLength, 
                    this.NeutralAxisPosition, 
                    64,
                    this.EditorService.XAxisColor,
                    lineThickness);
                
                spriteBatch.Draw(
                    settings.PixelsPerUnit,
                    this._sprite,
                    this.XAxisPosition - new Vector2(offset),
                    scale,
                    0f,
                    this.EditorService.XAxisColor);
            }
        }

        /// <inheritdoc />
        public override bool Update(FrameTime frameTime, InputState inputState) {
            var result = base.Update(frameTime, inputState);

            if (this.EntityService.Selected is IRotatable rotatable) {
                
            }
            
            return result;
        }

        /// <inheritdoc />
        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);
            
            if (this.Scene.Game.GraphicsDevice is GraphicsDevice graphicsDevice) {
                this._sprite = PrimitiveDrawer.CreateCircleSprite(graphicsDevice, GizmoPointSize);
            }
        }
    }
}