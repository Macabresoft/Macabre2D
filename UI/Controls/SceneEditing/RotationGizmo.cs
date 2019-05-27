namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Physics;
    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Windows.Input;

    public sealed class RotationGizmo : BaseGizmo {

        private readonly Body _circleBody = new Body() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly CircleDrawer _circleOutlineDrawer = new CircleDrawer();
        private readonly SpriteRenderer _circleRenderer = new SpriteRenderer();
        private ButtonState _previousButtonState = ButtonState.Released;
        private float _unmovedWorldRotationAngle;

        public RotationGizmo(IUndoService undoService) : base(undoService) {
            this.YAxisColor = Color.Transparent;
        }

        public override void Initialize(IGame game) {
            base.Initialize(game);
            var circleSprite = PrimitiveDrawer.CreateCircleSprite(game.GraphicsDevice, 64);
            this._circleRenderer.Sprite = circleSprite;
            this._circleRenderer.OffsetType = OffsetType.Center;
            this._circleRenderer.AddChild(this._circleBody);
            this._circleRenderer.Initialize(game.CurrentScene);

            this._circleOutlineDrawer.Color = this.XAxisColor;
            this._circleOutlineDrawer.LineThickness = 1f;
            this._circleOutlineDrawer.UseDynamicLineThickness = true;
            this._circleOutlineDrawer.Initialize(game.CurrentScene);
        }

        public override bool Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
            var hadInteractions = false;
            if (mouseState.LeftButton == ButtonState.Pressed) {
                if (this._previousButtonState == ButtonState.Pressed) {
                    hadInteractions = true;
                    this.UpdateAngle(selectedComponent, this.GetNewAngle(selectedComponent, mousePosition));
                }
                else if (this._circleBody.Collider.Contains(mousePosition)) {
                    this.StartDrag(selectedComponent.Component.WorldTransform.Rotation.Angle);
                    hadInteractions = true;
                }
            }
            else {
                this.EndDrag(selectedComponent);
            }

            return hadInteractions;
        }

        protected override void DrawGizmo(GameTime gameTime, Transform worldTransform, float viewHeight, float viewRatio, float lineLength) {
            this.ResetEndPoint(worldTransform, viewRatio, lineLength);

            var scale = viewRatio * 0.25f;
            this._circleRenderer.Color = this.XAxisColor;
            this._circleRenderer.LocalPosition = this.XAxisLineDrawer.EndPoint;
            this._circleRenderer.LocalScale = new Vector2(scale);
            this._circleRenderer.LocalRotation.Angle = worldTransform.Rotation.Angle;
            this._circleRenderer.Draw(gameTime, viewHeight);

            this._circleOutlineDrawer.Color = this.XAxisColor;
            this._circleOutlineDrawer.LocalPosition = worldTransform.Position;
            this._circleOutlineDrawer.LocalRotation.Angle = worldTransform.Rotation.Angle;
            this._circleOutlineDrawer.Radius = lineLength;
            this._circleOutlineDrawer.Complexity = 128;
            this._circleOutlineDrawer.Draw(gameTime, viewHeight);
        }

        private void EndDrag(ComponentWrapper selectedComponent) {
            if (this.CurrentAxis != GizmoAxis.None) {
                var originalAngle = this._unmovedWorldRotationAngle;
                var newAngle = selectedComponent.Component.WorldTransform.Rotation.Angle;
                var undoCommand = new UndoCommand(() => {
                    this.UpdateAngle(selectedComponent, newAngle);
                },
                () => {
                    this.UpdateAngle(selectedComponent, originalAngle);
                });

                this.UndoService.Do(undoCommand);
                this.CurrentAxis = GizmoAxis.None;
                System.Windows.Input.Mouse.OverrideCursor = null;
            }

            this._previousButtonState = ButtonState.Released;
            this.XAxisColor = new Color(this.XAxisColor, 1f);
        }

        private float GetNewAngle(ComponentWrapper selectedComponent, Vector2 mousePosition) {
            var worldPosition = selectedComponent.Component.WorldTransform.Position;
            var normalizedMouseVector = mousePosition - worldPosition;
            return (float)Math.Atan2(normalizedMouseVector.Y, normalizedMouseVector.X);
        }

        private void StartDrag(float currentRotationAngle) {
            this._previousButtonState = ButtonState.Pressed;
            this._unmovedWorldRotationAngle = currentRotationAngle;
            this.CurrentAxis = GizmoAxis.Neutral;
            System.Windows.Input.Mouse.OverrideCursor = Cursors.SizeAll;
            this.XAxisColor = new Color(this.XAxisColor, 0.5f);
        }

        private void UpdateAngle(ComponentWrapper component, float newRotationAngle) {
            component.Component.SetWorldRotation(newRotationAngle);
            component.UpdateProperty($"{nameof(BaseComponent.LocalRotation)}.{nameof(Rotation.Angle)}", component.Component.LocalRotation.Angle);
        }
    }
}