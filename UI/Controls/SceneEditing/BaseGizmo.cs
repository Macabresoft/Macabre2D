namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Extensions;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;

    public abstract class BaseGizmo {

        public BaseGizmo(IUndoService undoService) {
            this.UndoService = undoService;
        }

        protected enum GizmoAxis {
            X,
            Y,
            Neutral,
            None
        }

        public Color XAxisColor { get; set; } = Color.Red;

        public Color YAxisColor { get; set; } = Color.Green;

        protected GizmoAxis CurrentAxis { get; set; } = GizmoAxis.None;

        protected IGame Game { get; private set; }

        protected IUndoService UndoService { get; }

        protected LineDrawer XAxisLineDrawer { get; private set; }

        protected LineDrawer YAxisLineDrawer { get; private set; }

        public void Draw(GameTime gameTime, float viewHeight, BaseComponent selectedComponent) {
            if (selectedComponent != null) {
                var worldTransform = selectedComponent.WorldTransform;
                var ratio = GameSettings.Instance.GetPixelAgnosticRatio(viewHeight, this.Game.GraphicsDevice.Viewport.Height);
                var lineLength = this.GetDefaultLineLength(viewHeight);

                this.XAxisLineDrawer.Color = this.XAxisColor;
                this.YAxisLineDrawer.Color = this.YAxisColor;

                this.XAxisLineDrawer.Draw(gameTime, viewHeight);
                this.YAxisLineDrawer.Draw(gameTime, viewHeight);

                this.DrawGizmo(gameTime, worldTransform, viewHeight, ratio, lineLength);
            }
        }

        public virtual void Initialize(IGame game) {
            this.Game = game;

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

            this.XAxisLineDrawer.Initialize(this.Game.CurrentScene);
            this.YAxisLineDrawer.Initialize(this.Game.CurrentScene);
        }

        public abstract bool Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent);

        protected abstract void DrawGizmo(GameTime gameTime, Transform worldTransform, float viewHeight, float viewRatio, float lineLength);

        protected float GetDefaultLineLength(float viewHeight) {
            return viewHeight / 5f;
        }

        protected Vector2 MoveAlongAxis(Vector2 start, Vector2 end, Vector2 moveToPosition) {
            var slope = end.X != start.X ? (end.Y - start.Y) / (end.X - start.X) : 1f;
            var yIntercept = end.Y - slope * end.X;
            Vector2 newPosition;
            if (Math.Abs(slope) < 0.5f) {
                if (slope == 0f) {
                    newPosition = new Vector2(moveToPosition.X, end.Y);
                }
                else {
                    var newX = (moveToPosition.Y - yIntercept) / slope;
                    newPosition = new Vector2(newX, moveToPosition.Y);
                }
            }
            else {
                if (Math.Abs(slope) == 1f) {
                    newPosition = new Vector2(end.X, moveToPosition.Y);
                }
                else {
                    var newY = (slope * moveToPosition.X) + yIntercept;
                    newPosition = new Vector2(moveToPosition.X, newY);
                }
            }

            return newPosition;
        }

        protected void ResetEndPoint(Transform worldTransform, float ratio, float lineLength) {
            this.XAxisLineDrawer.StartPoint = worldTransform.Position;
            this.XAxisLineDrawer.EndPoint = worldTransform.Position + Vector2.UnitX.RotateRadians(worldTransform.Rotation.Angle) * lineLength;
            this.YAxisLineDrawer.StartPoint = worldTransform.Position;
            this.YAxisLineDrawer.EndPoint = worldTransform.Position - Vector2.UnitY.RotateRadians(-worldTransform.Rotation.Angle) * lineLength;
        }
    }
}