namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public abstract class BaseGizmo {
        private LineDrawer _xAxisLineDrawer;
        private LineDrawer _yAxisLineDrawer;

        public BaseGizmo(EditorGame editorGame) {
            this.EditorGame = editorGame;
        }

        public Color XAxisColor { get; set; } = Color.Red;

        public Color YAxisColor { get; set; } = Color.Green;

        protected EditorGame EditorGame { get; }

        public virtual void Draw(GameTime gameTime, float viewHeight, BaseComponent selectedComponent) {
            if (selectedComponent != null) {
                var worldTransform = selectedComponent.WorldTransform;
                var length = this.GetDynamicLineLength(viewHeight);
                this._xAxisLineDrawer.StartPoint = worldTransform.Position;
                this._xAxisLineDrawer.EndPoint = worldTransform.Position + Vector2.UnitX.RotateRadians(worldTransform.Rotation.Angle) * length;
                this._yAxisLineDrawer.StartPoint = worldTransform.Position;
                this._yAxisLineDrawer.EndPoint = worldTransform.Position - Vector2.UnitY.RotateRadians(-worldTransform.Rotation.Angle) * length;

                this._xAxisLineDrawer.Color = this.XAxisColor;
                this._yAxisLineDrawer.Color = this.YAxisColor;

                this._xAxisLineDrawer.Draw(gameTime, viewHeight);
                this._yAxisLineDrawer.Draw(gameTime, viewHeight);
            }
        }

        public virtual void Initialize() {
            this._xAxisLineDrawer = new LineDrawer() {
                Color = XAxisColor,
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this._yAxisLineDrawer = new LineDrawer() {
                Color = YAxisColor,
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this._xAxisLineDrawer.Initialize(this.EditorGame.CurrentScene);
            this._yAxisLineDrawer.Initialize(this.EditorGame.CurrentScene);
        }

        public abstract void Update(GameTime gameTime, MouseState mouseState, BaseComponent selectedComponent);

        private float GetDynamicLineLength(float viewHeight) {
            var result = 2f;
            if (this.EditorGame?.GameSettings is GameSettings gameSettings) {
                result *= (viewHeight / gameSettings.PixelsPerUnit);
            }

            return result;
        }
    }
}