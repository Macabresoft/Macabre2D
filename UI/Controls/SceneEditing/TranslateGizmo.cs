namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Rendering;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class TranslateGizmo : BaseGizmo {
        private readonly SpriteRenderer _xAxisArrowRenderer = new SpriteRenderer();
        private readonly SpriteRenderer _xAxisTriangleRenderer = new SpriteRenderer();
        private readonly SpriteRenderer _yAxisArrowRenderer = new SpriteRenderer();
        private readonly SpriteRenderer _yAxisTriangleRenderer = new SpriteRenderer();

        public TranslateGizmo(EditorGame editorGame) : base(editorGame) {
        }

        public override void Initialize() {
            base.Initialize();

            var arrowSprite = PrimitiveDrawer.CreateArrowSprite(this.EditorGame.GraphicsDevice, 64);
            this._xAxisArrowRenderer.Sprite = arrowSprite;
            this._yAxisArrowRenderer.Sprite = arrowSprite;
            this._xAxisArrowRenderer.OffsetType = OffsetType.Center;
            this._yAxisArrowRenderer.OffsetType = OffsetType.Center;
            this._xAxisArrowRenderer.Initialize(this.EditorGame.CurrentScene);
            this._yAxisArrowRenderer.Initialize(this.EditorGame.CurrentScene);

            var triangleSprite = PrimitiveDrawer.CreateRightTriangleSprite(this.EditorGame.GraphicsDevice, new Point(64));
            this._xAxisTriangleRenderer.Sprite = triangleSprite;
            this._yAxisTriangleRenderer.Sprite = triangleSprite;
            this._xAxisTriangleRenderer.OffsetType = OffsetType.Center;
            this._yAxisTriangleRenderer.OffsetType = OffsetType.Center;
            this._xAxisTriangleRenderer.Initialize(this.EditorGame.CurrentScene);
            this._yAxisTriangleRenderer.Initialize(this.EditorGame.CurrentScene);
        }

        public override void Update(GameTime gameTime, MouseState mouseState, BaseComponent selectedComponent) {
            // TODO: allow dragging of the axis.
            return;
        }

        protected override void DrawGizmo(GameTime gameTime, Transform worldTransform, float viewHeight, float viewRatio) {
            var scale = viewRatio * 0.25f;
            this._xAxisArrowRenderer.Color = this.XAxisColor;
            this._xAxisArrowRenderer.LocalPosition = this.XAxisLineDrawer.EndPoint;
            this._xAxisArrowRenderer.LocalScale = new Vector2(scale);
            this._xAxisArrowRenderer.LocalRotation.Angle = worldTransform.Rotation.Angle - MathHelper.ToRadians(90f);
            this._xAxisArrowRenderer.Draw(gameTime, viewHeight);

            this._yAxisArrowRenderer.Color = this.YAxisColor;
            this._yAxisArrowRenderer.LocalPosition = this.YAxisLineDrawer.EndPoint;
            this._yAxisArrowRenderer.LocalScale = new Vector2(scale);
            this._yAxisArrowRenderer.LocalRotation.Angle = worldTransform.Rotation.Angle;
            this._yAxisArrowRenderer.Draw(gameTime, viewHeight);

            this._xAxisTriangleRenderer.Color = this.XAxisColor;
            this._xAxisTriangleRenderer.LocalPosition = this.XAxisLineDrawer.StartPoint;
            this._xAxisTriangleRenderer.LocalScale = new Vector2(scale);
            this._xAxisTriangleRenderer.LocalRotation.Angle = worldTransform.Rotation.Angle + MathHelper.ToRadians(180f);
            this._xAxisTriangleRenderer.Draw(gameTime, viewHeight);

            this._yAxisTriangleRenderer.Color = this.YAxisColor;
            this._yAxisTriangleRenderer.LocalPosition = this.YAxisLineDrawer.StartPoint;
            this._yAxisTriangleRenderer.LocalScale = new Vector2(scale);
            this._yAxisTriangleRenderer.LocalRotation.Angle = worldTransform.Rotation.Angle;
            this._yAxisTriangleRenderer.Draw(gameTime, viewHeight);
        }
    }
}