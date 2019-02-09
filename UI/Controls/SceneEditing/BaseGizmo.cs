namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public abstract class BaseGizmo {

        public BaseGizmo(EditorGame editorGame) {
            this.EditorGame = editorGame;
        }

        public Color XAxisColor { get; set; } = Color.Red;

        public Color YAxisColor { get; set; } = Color.Green;

        protected EditorGame EditorGame { get; }

        protected LineDrawer XAxisLineDrawer { get; private set; }

        protected LineDrawer YAxisLineDrawer { get; private set; }

        public void Draw(GameTime gameTime, float viewHeight, BaseComponent selectedComponent) {
            if (selectedComponent != null) {
                var worldTransform = selectedComponent.WorldTransform;
                var ratio = this.GetViewHeightRatio(viewHeight);
                var length = ratio * 2f;
                this.XAxisLineDrawer.StartPoint = worldTransform.Position;
                this.XAxisLineDrawer.EndPoint = worldTransform.Position + Vector2.UnitX.RotateRadians(worldTransform.Rotation.Angle) * length;
                this.YAxisLineDrawer.StartPoint = worldTransform.Position;
                this.YAxisLineDrawer.EndPoint = worldTransform.Position - Vector2.UnitY.RotateRadians(-worldTransform.Rotation.Angle) * length;

                this.XAxisLineDrawer.Color = this.XAxisColor;
                this.YAxisLineDrawer.Color = this.YAxisColor;

                this.XAxisLineDrawer.Draw(gameTime, viewHeight);
                this.YAxisLineDrawer.Draw(gameTime, viewHeight);

                this.DrawGizmo(gameTime, worldTransform, viewHeight, ratio);
            }
        }

        public virtual void Initialize() {
            this.XAxisLineDrawer = new LineDrawer() {
                Color = XAxisColor,
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this.YAxisLineDrawer = new LineDrawer() {
                Color = YAxisColor,
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this.XAxisLineDrawer.Initialize(this.EditorGame.CurrentScene);
            this.YAxisLineDrawer.Initialize(this.EditorGame.CurrentScene);
        }

        public abstract void Update(GameTime gameTime, MouseState mouseState, BaseComponent selectedComponent);

        protected abstract void DrawGizmo(GameTime gameTime, Transform worldTransform, float viewHeight, float viewRatio);

        private float GetViewHeightRatio(float viewHeight) {
            var result = 1f;
            if (this.EditorGame?.GameSettings is GameSettings gameSettings) {
                result *= (viewHeight / gameSettings.PixelsPerUnit);
            }

            return result;
        }
    }
}