namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System.Windows.Input;

    public sealed class TranslationGizmo : BaseAxisGizmo {

        private readonly SimpleBody _neutralAxisBody = new SimpleBody() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly SpriteRenderer _xAxisArrowRenderer = new SpriteRenderer();

        private readonly SimpleBody _xAxisBody = new SimpleBody() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly SpriteRenderer _xAxisTriangleRenderer = new SpriteRenderer();

        private readonly SpriteRenderer _yAxisArrowRenderer = new SpriteRenderer();

        private readonly SimpleBody _yAxisBody = new SimpleBody() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly SpriteRenderer _yAxisTriangleRenderer = new SpriteRenderer();
        private ButtonState _previousButtonState = ButtonState.Released;
        private Vector2 _unmovedWorldPosition;

        public TranslationGizmo(IUndoService undoService) : base(undoService) {
        }

        public override string EditingPropertyName {
            get {
                return nameof(BaseComponent.LocalPosition);
            }
        }

        public override void Initialize(EditorGame game) {
            base.Initialize(game);

            this._xAxisArrowRenderer.Sprite = PrimitiveDrawer.CreateForwardArrowSprite(this.Game.GraphicsDevice, 64);
            this._xAxisArrowRenderer.Offset.Type = PixelOffsetType.Center;
            this._xAxisArrowRenderer.AddChild(this._xAxisBody);
            this._xAxisArrowRenderer.Initialize(this.Game.CurrentScene);

            this._yAxisArrowRenderer.Sprite = PrimitiveDrawer.CreateUpwardsArrowSprite(this.Game.GraphicsDevice, 64);
            this._yAxisArrowRenderer.Offset.Type = PixelOffsetType.Center;
            this._yAxisArrowRenderer.AddChild(this._yAxisBody);
            this._yAxisArrowRenderer.Initialize(this.Game.CurrentScene);

            var triangleSprite = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(this.Game.GraphicsDevice, new Point(64));
            this._xAxisTriangleRenderer.Sprite = triangleSprite;
            this._xAxisTriangleRenderer.Offset.Type = PixelOffsetType.Center;
            this._xAxisTriangleRenderer.AddChild(this._neutralAxisBody);
            this._xAxisTriangleRenderer.Initialize(this.Game.CurrentScene);

            this._yAxisTriangleRenderer.Sprite = triangleSprite;
            this._yAxisTriangleRenderer.Offset.Type = PixelOffsetType.Center;
            this._yAxisTriangleRenderer.Initialize(this.Game.CurrentScene);
        }

        public override bool Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
            var hadInteractions = false;
            if (mouseState.LeftButton == ButtonState.Pressed) {
                if (this._previousButtonState == ButtonState.Pressed) {
                    hadInteractions = true;

                    if (this.CurrentAxis == GizmoAxis.Neutral) {
                        this.UpdatePosition(selectedComponent, mousePosition);
                    }
                    else if (this.CurrentAxis == GizmoAxis.X) {
                        var newPosition = this.MoveAlongAxis(this.XAxisLineDrawer.StartPoint, this.XAxisLineDrawer.EndPoint, mousePosition);
                        var distance = this.XAxisLineDrawer.EndPoint - this.XAxisLineDrawer.StartPoint;
                        this.UpdatePosition(selectedComponent, newPosition - distance);
                    }
                    else if (this.CurrentAxis == GizmoAxis.Y) {
                        var newPosition = this.MoveAlongAxis(this.YAxisLineDrawer.StartPoint, this.YAxisLineDrawer.EndPoint, mousePosition);
                        var distance = this.YAxisLineDrawer.EndPoint - this.YAxisLineDrawer.StartPoint;
                        this.UpdatePosition(selectedComponent, newPosition - distance);
                    }
                }
                else {
                    if (this._neutralAxisBody.Collider.Contains(mousePosition)) {
                        this.StartDrag(GizmoAxis.Neutral, selectedComponent.Component.WorldTransform.Position);
                        hadInteractions = true;
                    }
                    else if (this._xAxisBody.Collider.Contains(mousePosition)) {
                        this.StartDrag(GizmoAxis.X, selectedComponent.Component.WorldTransform.Position);
                        hadInteractions = true;
                    }
                    else if (this._yAxisBody.Collider.Contains(mousePosition)) {
                        this.StartDrag(GizmoAxis.Y, selectedComponent.Component.WorldTransform.Position);
                        hadInteractions = true;
                    }
                }
            }
            else {
                this.EndDrag(selectedComponent);
            }

            return hadInteractions;
        }

        protected override void DrawGizmo(GameTime gameTime, BaseComponent selectedComponent, BoundingArea viewBoundingArea, float viewRatio, float lineLength) {
            this.ResetEndPoint(selectedComponent, lineLength);

            var scale = viewRatio * 0.25f;
            this._xAxisArrowRenderer.Color = this.XAxisColor;
            this._xAxisArrowRenderer.LocalPosition = this.XAxisLineDrawer.EndPoint;
            this._xAxisArrowRenderer.LocalScale = new Vector2(scale);
            this._xAxisArrowRenderer.Draw(gameTime, viewBoundingArea);

            this._yAxisArrowRenderer.Color = this.YAxisColor;
            this._yAxisArrowRenderer.LocalPosition = this.YAxisLineDrawer.EndPoint;
            this._yAxisArrowRenderer.LocalScale = new Vector2(scale);
            this._yAxisArrowRenderer.Draw(gameTime, viewBoundingArea);

            this._xAxisTriangleRenderer.Color = this.XAxisColor;
            this._xAxisTriangleRenderer.LocalPosition = this.XAxisLineDrawer.StartPoint;
            this._xAxisTriangleRenderer.LocalScale = new Vector2(scale);
            this._xAxisTriangleRenderer.Draw(gameTime, viewBoundingArea);

            this._yAxisTriangleRenderer.Color = this.YAxisColor;
            this._yAxisTriangleRenderer.LocalPosition = this.YAxisLineDrawer.StartPoint;
            this._yAxisTriangleRenderer.LocalScale = new Vector2(-scale);
            this._yAxisTriangleRenderer.Draw(gameTime, viewBoundingArea);

            if (selectedComponent is IRotatable rotatable) {
                this._xAxisArrowRenderer.Rotation.Angle = rotatable.Rotation.Angle;
                this._yAxisArrowRenderer.Rotation.Angle = rotatable.Rotation.Angle;
                this._xAxisTriangleRenderer.Rotation.Angle = rotatable.Rotation.Angle;
                this._yAxisTriangleRenderer.Rotation.Angle = rotatable.Rotation.Angle;
            }
            else {
                this._xAxisArrowRenderer.Rotation.Angle = 0f;
                this._yAxisArrowRenderer.Rotation.Angle = 0f;
                this._xAxisTriangleRenderer.Rotation.Angle = 0f;
                this._yAxisTriangleRenderer.Rotation.Angle = 0f;
            }
        }

        private void EndDrag(ComponentWrapper selectedComponent) {
            if (this.CurrentAxis != GizmoAxis.None) {
                var originalPosition = this._unmovedWorldPosition;
                var newPosition = selectedComponent.Component.WorldTransform.Position;
                var undoCommand = new UndoCommand(() => {
                    this.UpdatePosition(selectedComponent, newPosition);
                },
                () => {
                    this.UpdatePosition(selectedComponent, originalPosition);
                });

                this.UndoService.Do(undoCommand);
                this.CurrentAxis = GizmoAxis.None;
                System.Windows.Input.Mouse.OverrideCursor = null;
            }

            this._previousButtonState = ButtonState.Released;
            this.XAxisColor = new Color(this.XAxisColor, 1f);
            this.YAxisColor = new Color(this.YAxisColor, 1f);
        }

        private void StartDrag(GizmoAxis axis, Vector2 currentPosition) {
            this._previousButtonState = ButtonState.Pressed;
            this._unmovedWorldPosition = currentPosition;
            this.CurrentAxis = axis;
            System.Windows.Input.Mouse.OverrideCursor = Cursors.SizeAll;
            this.XAxisColor = new Color(this.XAxisColor, 0.5f);
            this.YAxisColor = new Color(this.YAxisColor, 0.5f);
        }

        private void UpdatePosition(ComponentWrapper component, Vector2 newPosition) {
            component.Component.SetWorldPosition(newPosition);
            component.UpdateProperty(nameof(BaseComponent.LocalPosition), component.Component.LocalPosition);
        }
    }
}