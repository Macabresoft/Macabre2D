namespace Macabre2D.UI.Library.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.Models.FrameworkWrappers;
    using Macabre2D.UI.Library.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System.Windows.Input;

    public sealed class ScaleGizmo : BaseAxisGizmo {

        private readonly SimpleBodyComponent _xAxisBody = new SimpleBodyComponent() {
            Collider = new CircleCollider(64f, RadiusScalingType.X)
        };

        private readonly SpriteRenderComponent _xAxisSquareRenderer = new SpriteRenderComponent();

        private readonly SimpleBodyComponent _yAxisBody = new SimpleBodyComponent() {
            Collider = new CircleCollider(64f, RadiusScalingType.X)
        };

        private readonly SpriteRenderComponent _yAxisSquareRenderer = new SpriteRenderComponent();
        private float _defaultLineLength;
        private ButtonState _previousButtonState = ButtonState.Released;
        private Vector2 _unmovedWorldScale;

        public ScaleGizmo(IUndoService undoService) : base(undoService) {
        }

        public override string EditingPropertyName {
            get {
                return nameof(BaseComponent.LocalScale);
            }
        }

        public override void Initialize(SceneEditor game) {
            base.Initialize(game);
            var squareSprite = PrimitiveDrawer.CreateQuadSprite(this.Game.GraphicsDevice, new Point(64));
            this._xAxisSquareRenderer.Sprite = squareSprite;
            this._yAxisSquareRenderer.Sprite = squareSprite;
            this._xAxisSquareRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            this._yAxisSquareRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            this._xAxisSquareRenderer.AddChild(this._xAxisBody);
            this._yAxisSquareRenderer.AddChild(this._yAxisBody);
            this._xAxisSquareRenderer.Initialize(this.Game.CurrentScene);
            this._yAxisSquareRenderer.Initialize(this.Game.CurrentScene);
        }

        public override bool Update(FrameTime frameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
            var hadInteractions = false;
            if (mouseState.LeftButton == ButtonState.Pressed) {
                if (this._previousButtonState == ButtonState.Pressed) {
                    hadInteractions = true;

                    if (this.CurrentAxis == GizmoAxis.X) {
                        var newPosition = this.MoveAlongAxis(this.XAxisLineDrawer.StartPoint, this.XAxisLineDrawer.EndPoint, mousePosition);
                        var newLineLength = Vector2.Distance(this.XAxisLineDrawer.StartPoint, newPosition);
                        this.XAxisLineDrawer.EndPoint = newPosition;

                        var multiplier = this.GetScaleSign(newPosition, selectedComponent.Component) * newLineLength / this._defaultLineLength;
                        var newScale = this._unmovedWorldScale;

                        if (keyboardState.IsKeyDown(Keys.LeftShift)) {
                            newScale *= multiplier;
                        }
                        else {
                            newScale = new Vector2(newScale.X * multiplier, newScale.Y);
                        }

                        this.UpdateScale(selectedComponent, newScale);
                    }
                    else if (this.CurrentAxis == GizmoAxis.Y) {
                        var newPosition = this.MoveAlongAxis(this.YAxisLineDrawer.StartPoint, this.YAxisLineDrawer.EndPoint, mousePosition);
                        var newLineLength = Vector2.Distance(this.YAxisLineDrawer.StartPoint, newPosition);
                        this.YAxisLineDrawer.EndPoint = newPosition;

                        var multiplier = this.GetScaleSign(newPosition, selectedComponent.Component) * newLineLength / this._defaultLineLength;
                        var newScale = this._unmovedWorldScale;

                        if (keyboardState.IsKeyDown(Keys.LeftShift)) {
                            newScale *= multiplier;
                        }
                        else {
                            newScale = new Vector2(newScale.X, newScale.Y * multiplier);
                        }

                        this.UpdateScale(selectedComponent, newScale);
                    }
                }
                else {
                    if (this._xAxisBody.Collider.Contains(mousePosition)) {
                        this.StartDrag(GizmoAxis.X, selectedComponent.Component.WorldTransform.Scale);
                        hadInteractions = true;
                    }
                    else if (this._yAxisBody.Collider.Contains(mousePosition)) {
                        this.StartDrag(GizmoAxis.Y, selectedComponent.Component.WorldTransform.Scale);
                        hadInteractions = true;
                    }
                }
            }
            else {
                this.EndDrag(selectedComponent);
            }

            return hadInteractions;
        }

        protected override void DrawGizmo(FrameTime frameTime, BaseComponent selectedComponent, BoundingArea viewBoundingArea, float viewRatio, float lineLength) {
            if (this._previousButtonState == ButtonState.Released) {
                this.ResetEndPoint(selectedComponent, lineLength);
            }

            this._defaultLineLength = lineLength;

            var scale = viewRatio * 0.25f;
            this._xAxisSquareRenderer.Color = this.XAxisColor;
            this._xAxisSquareRenderer.LocalPosition = this.XAxisLineDrawer.EndPoint;
            this._xAxisSquareRenderer.LocalScale = new Vector2(scale);
            this._xAxisSquareRenderer.Draw(frameTime, viewBoundingArea);

            this._yAxisSquareRenderer.Color = this.YAxisColor;
            this._yAxisSquareRenderer.LocalPosition = this.YAxisLineDrawer.EndPoint;
            this._yAxisSquareRenderer.LocalScale = new Vector2(scale);
            this._yAxisSquareRenderer.Draw(frameTime, viewBoundingArea);

            this._xAxisBody.LocalScale = new Vector2(GameSettings.Instance.InversePixelsPerUnit);
            this._yAxisBody.LocalScale = new Vector2(GameSettings.Instance.InversePixelsPerUnit);

            if (selectedComponent is IRotatable rotatable) {
                this._xAxisSquareRenderer.Rotation = rotatable.Rotation;
                this._yAxisSquareRenderer.Rotation = rotatable.Rotation;
            }
            else {
                this._xAxisSquareRenderer.Rotation = 0f;
                this._yAxisSquareRenderer.Rotation = 0f;
            }
        }

        private void EndDrag(ComponentWrapper selectedComponent) {
            if (this.CurrentAxis != GizmoAxis.None) {
                var originalScale = this._unmovedWorldScale;
                var newScale = selectedComponent.Component.WorldTransform.Scale;
                var undoCommand = new UndoCommand(() => {
                    this.UpdateScale(selectedComponent, newScale);
                },
                () => {
                    this.UpdateScale(selectedComponent, originalScale);
                });

                this.UndoService.Do(undoCommand);
                this.CurrentAxis = GizmoAxis.None;
                System.Windows.Input.Mouse.OverrideCursor = null;
            }

            this._previousButtonState = ButtonState.Released;
            this.XAxisColor = new Color(this.XAxisColor, 1f);
            this.YAxisColor = new Color(this.YAxisColor, 1f);
        }

        private float GetScaleSign(Vector2 dragPosition, BaseComponent selectedComponent) {
            Vector2 dragStartPoint;
            var worldTransform = selectedComponent.WorldTransform;
            if (selectedComponent is IRotatable rotatable) {
                dragStartPoint = worldTransform.Position + (this.CurrentAxis == GizmoAxis.X ? Vector2.UnitX.RotateRadians(rotatable.Rotation) : -Vector2.UnitY.RotateRadians(-rotatable.Rotation)) * this._defaultLineLength;
            }
            else {
                dragStartPoint = worldTransform.Position + (this.CurrentAxis == GizmoAxis.X ? new Vector2(this._defaultLineLength, 0f) : new Vector2(0f, this._defaultLineLength));
            }

            var dragDistanceFromComponent = Vector2.Distance(dragPosition, worldTransform.Position);
            var totalDragDistance = Vector2.Distance(dragPosition, dragStartPoint);
            return (totalDragDistance > this._defaultLineLength && dragDistanceFromComponent < totalDragDistance) ? -1f : 1f;
        }

        private void StartDrag(GizmoAxis axis, Vector2 currentScale) {
            this._previousButtonState = ButtonState.Pressed;
            this._unmovedWorldScale = currentScale;
            this.CurrentAxis = axis;
            System.Windows.Input.Mouse.OverrideCursor = Cursors.SizeAll;
            this.XAxisColor = new Color(this.XAxisColor, 0.5f);
            this.YAxisColor = new Color(this.YAxisColor, 0.5f);
        }

        private void UpdateScale(ComponentWrapper component, Vector2 newScale) {
            component.Component.SetWorldScale(newScale);
            component.UpdateProperty(nameof(BaseComponent.LocalScale), component.Component.LocalScale);
        }
    }
}