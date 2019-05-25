namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Physics;
    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class ScaleGizmo : BaseGizmo {

        private readonly Body _xAxisBody = new Body() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly SpriteRenderer _xAxisSquareRenderer = new SpriteRenderer();

        private readonly Body _yAxisBody = new Body() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly SpriteRenderer _yAxisSquareRenderer = new SpriteRenderer();

        public ScaleGizmo(IUndoService undoService) : base(undoService) {
        }

        public override void Initialize(IGame game) {
            base.Initialize(game);
            var squareSprite = PrimitiveDrawer.CreateQuadSprite(this.Game.GraphicsDevice, new Point(64));
            this._xAxisSquareRenderer.Sprite = squareSprite;
            this._yAxisSquareRenderer.Sprite = squareSprite;
            this._xAxisSquareRenderer.OffsetType = OffsetType.Center;
            this._yAxisSquareRenderer.OffsetType = OffsetType.Center;
            this._xAxisSquareRenderer.AddChild(this._xAxisBody);
            this._yAxisSquareRenderer.AddChild(this._yAxisBody);
            this._xAxisSquareRenderer.Initialize(this.Game.CurrentScene);
            this._yAxisSquareRenderer.Initialize(this.Game.CurrentScene);
        }

        public override bool Update(GameTime gameTime, MouseState mouseState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
            var hadInteractions = false;
            this.EndDrag(selectedComponent);
            return hadInteractions;
        }

        protected override void DrawGizmo(GameTime gameTime, Transform worldTransform, float viewHeight, float viewRatio) {
            var scale = viewRatio * 0.25f;
            this._xAxisSquareRenderer.Color = this.XAxisColor;
            this._xAxisSquareRenderer.LocalPosition = this.XAxisLineDrawer.EndPoint;
            this._xAxisSquareRenderer.LocalScale = new Vector2(scale);
            this._xAxisSquareRenderer.LocalRotation.Angle = worldTransform.Rotation.Angle - MathHelper.ToRadians(90f);
            this._xAxisSquareRenderer.Draw(gameTime, viewHeight);

            this._yAxisSquareRenderer.Color = this.YAxisColor;
            this._yAxisSquareRenderer.LocalPosition = this.YAxisLineDrawer.EndPoint;
            this._yAxisSquareRenderer.LocalScale = new Vector2(scale);
            this._yAxisSquareRenderer.LocalRotation.Angle = worldTransform.Rotation.Angle;
            this._yAxisSquareRenderer.Draw(gameTime, viewHeight);
        }

        private void EndDrag(ComponentWrapper selectedComponent) {
            this.XAxisColor = new Color(this.XAxisColor, 1f);
            this.YAxisColor = new Color(this.YAxisColor, 1f);
        }
    }
}