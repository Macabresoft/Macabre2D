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
        }

        public override void Update(GameTime gameTime, MouseState mouseState, BaseComponent selectedComponent) {
            // TODO: allow dragging of the axis.
            return;
        }

        protected override void DrawGizmo(GameTime gameTime, Transform worldTransform, float viewHeight, float viewRatio) {
            this._xAxisSpriteRenderer.Color = this.XAxisColor;
            this._xAxisSpriteRenderer.LocalPosition = this.XAxisLineDrawer.EndPoint;
            this._xAxisSpriteRenderer.LocalScale = new Vector2(viewRatio);
            this._xAxisSpriteRenderer.Draw(gameTime, viewHeight);

            this._yAxisSpriteRenderer.Color = this.YAxisColor;
            this._yAxisSpriteRenderer.LocalPosition = this.YAxisLineDrawer.EndPoint;
            this._yAxisSpriteRenderer.LocalScale = new Vector2(viewRatio);
            this._yAxisSpriteRenderer.Draw(gameTime, viewHeight);
        }
    }
}