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

    public sealed class TranslateGizmo : BaseGizmo {

        private readonly Body _neutralAxisBody = new Body() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly SpriteRenderer _xAxisArrowRenderer = new SpriteRenderer();

        private readonly Body _xAxisBody = new Body() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly SpriteRenderer _xAxisTriangleRenderer = new SpriteRenderer();

        private readonly SpriteRenderer _yAxisArrowRenderer = new SpriteRenderer();

        private readonly Body _yAxisBody = new Body() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly SpriteRenderer _yAxisTriangleRenderer = new SpriteRenderer();
        private ButtonState _previousButtonState = ButtonState.Released;
        private Vector2 _unmovedWorldPosition;

        public TranslateGizmo(IUndoService undoService) : base(undoService) {
        }

        public override void Initialize(IGame game) {
            base.Initialize(game);
            var arrowSprite = PrimitiveDrawer.CreateArrowSprite(this.Game.GraphicsDevice, 64);
            this._xAxisArrowRenderer.Sprite = arrowSprite;
            this._yAxisArrowRenderer.Sprite = arrowSprite;
            this._xAxisArrowRenderer.OffsetType = OffsetType.Center;
            this._yAxisArrowRenderer.OffsetType = OffsetType.Center;
            this._xAxisArrowRenderer.AddChild(this._xAxisBody);
            this._yAxisArrowRenderer.AddChild(this._yAxisBody);
            this._xAxisArrowRenderer.Initialize(this.Game.CurrentScene);
            this._yAxisArrowRenderer.Initialize(this.Game.CurrentScene);

            var triangleSprite = PrimitiveDrawer.CreateRightTriangleSprite(this.Game.GraphicsDevice, new Point(64));
            this._xAxisTriangleRenderer.Sprite = triangleSprite;
            this._yAxisTriangleRenderer.Sprite = triangleSprite;
            this._xAxisTriangleRenderer.OffsetType = OffsetType.Center;
            this._yAxisTriangleRenderer.OffsetType = OffsetType.Center;
            this._xAxisTriangleRenderer.AddChild(this._neutralAxisBody);
            this._xAxisTriangleRenderer.Initialize(this.Game.CurrentScene);
            this._yAxisTriangleRenderer.Initialize(this.Game.CurrentScene);
        }

        public override bool Update(GameTime gameTime, MouseState mouseState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
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

        private Vector2 MoveAlongAxis(Vector2 start, Vector2 end, Vector2 moveToPosition) {
            var slope = end.X != start.X ? (end.Y - start.Y) / (end.X - start.X) : 1f;
            var yIntercept = end.Y - slope * end.X;
            var newPosition = moveToPosition;

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
            component.UpdateProperty(nameof(component.Component.LocalPosition), component.Component.LocalPosition);
        }
    }
}