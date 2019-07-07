namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Windows.Input;

    public sealed class RotationGizmo : BaseAxisGizmo {

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
            this._circleRenderer.Offset.Type = PixelOffsetType.Center;
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
                else if (this._circleBody.Collider.Contains(mousePosition) && selectedComponent.Component is IRotatable rotatable) {
                    this.StartDrag(rotatable.Rotation.Angle);
                    hadInteractions = true;
                }
            }
            else {
                this.EndDrag(selectedComponent);
            }

            return hadInteractions;
        }

        protected override void DrawGizmo(GameTime gameTime, BaseComponent selectedComponent, BoundingArea viewBoundingArea, float viewRatio, float lineLength) {
            this.ResetEndPoint(selectedComponent, lineLength);

            if (selectedComponent is IRotatable rotatable) {
                var scale = viewRatio * 0.25f;
                this._circleRenderer.Color = this.XAxisColor;
                this._circleRenderer.LocalPosition = this.XAxisLineDrawer.EndPoint;
                this._circleRenderer.LocalScale = new Vector2(scale);
                this._circleRenderer.Rotation.Angle = rotatable.Rotation.Angle;
                this._circleRenderer.Draw(gameTime, viewBoundingArea);

                this._circleOutlineDrawer.Color = this.XAxisColor;
                this._circleOutlineDrawer.LocalPosition = selectedComponent.WorldTransform.Position;
                this._circleOutlineDrawer.Radius = lineLength;
                this._circleOutlineDrawer.Complexity = 128;
                this._circleOutlineDrawer.Draw(gameTime, viewBoundingArea);
            }
        }

        private void EndDrag(ComponentWrapper selectedComponent) {
            if (this.CurrentAxis != GizmoAxis.None && selectedComponent.Component is IRotatable rotatable) {
                var originalAngle = this._unmovedWorldRotationAngle;
                var newAngle = rotatable.Rotation.Angle;
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
            if (component.Component is IRotatable rotatable) {
                //component.Component.SetWorldRotation(newRotationAngle); Might not need this??
                component.UpdateProperty($"{nameof(IRotatable.Rotation)}.{nameof(Rotation.Angle)}", rotatable.Rotation.Angle);
            }
        }
    }
}