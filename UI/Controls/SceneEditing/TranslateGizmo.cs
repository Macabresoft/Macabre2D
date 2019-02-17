namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Rendering;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class TranslateGizmo : BaseGizmo {
        private readonly SpriteRenderer _xAxisSpriteRenderer = new SpriteRenderer();
        private readonly SpriteRenderer _yAxisSpriteRenderer = new SpriteRenderer();

        public TranslateGizmo(EditorGame editorGame) : base(editorGame) {
        }

        public override void Initialize() {
            base.Initialize();

            var arrowSprite = PrimitiveDrawer.CreateArrowSprite(this.EditorGame.GraphicsDevice, 32);
            this._xAxisSpriteRenderer.Sprite = arrowSprite;
            this._yAxisSpriteRenderer.Sprite = arrowSprite;
            this._xAxisSpriteRenderer.OffsetType = OffsetType.Center;
            this._yAxisSpriteRenderer.OffsetType = OffsetType.Center;
            this._xAxisSpriteRenderer.Initialize(this.EditorGame.CurrentScene);
            this._yAxisSpriteRenderer.Initialize(this.EditorGame.CurrentScene);
        }

        public override void Update(GameTime gameTime, MouseState mouseState, BaseComponent selectedComponent) {
            // TODO: allow dragging of the axis.
            return;
        }

        protected override void DrawGizmo(GameTime gameTime, Transform worldTransform, float viewHeight, float viewRatio) {
            var scale = viewRatio * 0.5f;
            this._xAxisSpriteRenderer.Color = this.XAxisColor;
            this._xAxisSpriteRenderer.LocalPosition = this.XAxisLineDrawer.EndPoint;
            this._xAxisSpriteRenderer.LocalScale = new Vector2(scale);
            this._xAxisSpriteRenderer.LocalRotation.Angle = worldTransform.Rotation.Angle - MathHelper.ToRadians(90f);
            this._xAxisSpriteRenderer.Draw(gameTime, viewHeight);

            this._yAxisSpriteRenderer.Color = this.YAxisColor;
            this._yAxisSpriteRenderer.LocalPosition = this.YAxisLineDrawer.EndPoint;
            this._yAxisSpriteRenderer.LocalScale = new Vector2(scale);
            this._yAxisSpriteRenderer.LocalRotation.Angle = worldTransform.Rotation.Angle;
            this._yAxisSpriteRenderer.Draw(gameTime, viewHeight);
        }
    }
}