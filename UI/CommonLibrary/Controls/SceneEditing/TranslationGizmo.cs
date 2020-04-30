namespace Macabre2D.UI.CommonLibrary.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Models.FrameworkWrappers;
    using Macabre2D.UI.CommonLibrary.Services;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Windows.Input;

    public sealed class TranslationGizmo : BaseAxisGizmo {

        private readonly SimpleBodyComponent _neutralAxisBody = new SimpleBodyComponent() {
            Collider = new CircleCollider(64f, RadiusScalingType.X)
        };

        private readonly IStatusService _statusService;
        private readonly SpriteRenderComponent _xAxisArrowRenderer = new SpriteRenderComponent();

        private readonly SimpleBodyComponent _xAxisBody = new SimpleBodyComponent() {
            Collider = new CircleCollider(64f, RadiusScalingType.X)
        };

        private readonly SpriteRenderComponent _xAxisTriangleRenderer = new SpriteRenderComponent();

        private readonly SpriteRenderComponent _yAxisArrowRenderer = new SpriteRenderComponent();

        private readonly SimpleBodyComponent _yAxisBody = new SimpleBodyComponent() {
            Collider = new CircleCollider(64f, RadiusScalingType.X)
        };

        private readonly SpriteRenderComponent _yAxisTriangleRenderer = new SpriteRenderComponent();
        private ButtonState _previousButtonState = ButtonState.Released;
        private Vector2 _unmovedWorldPosition;

        public TranslationGizmo(IUndoService undoService, IStatusService statusService) : base(undoService) {
            this._statusService = statusService;
        }

        public override string EditingPropertyName {
            get {
                return nameof(BaseComponent.LocalPosition);
            }
        }

        public override void Initialize(SceneEditor game) {
            base.Initialize(game);

            this._xAxisArrowRenderer.Sprite = PrimitiveDrawer.CreateForwardArrowSprite(this.Game.GraphicsDevice, 64);
            this._xAxisArrowRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            this._xAxisArrowRenderer.AddChild(this._xAxisBody);
            this._xAxisArrowRenderer.Initialize(this.Game.CurrentScene);

            this._yAxisArrowRenderer.Sprite = PrimitiveDrawer.CreateUpwardsArrowSprite(this.Game.GraphicsDevice, 64);
            this._yAxisArrowRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            this._yAxisArrowRenderer.AddChild(this._yAxisBody);
            this._yAxisArrowRenderer.Initialize(this.Game.CurrentScene);

            var triangleSprite = PrimitiveDrawer.CreateTopLeftRightTriangleSprite(this.Game.GraphicsDevice, new Point(64));
            this._xAxisTriangleRenderer.Sprite = triangleSprite;
            this._xAxisTriangleRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            this._xAxisTriangleRenderer.AddChild(this._neutralAxisBody);
            this._xAxisTriangleRenderer.Initialize(this.Game.CurrentScene);

            this._yAxisTriangleRenderer.Sprite = triangleSprite;
            this._yAxisTriangleRenderer.RenderSettings.OffsetType = PixelOffsetType.Center;
            this._yAxisTriangleRenderer.Initialize(this.Game.CurrentScene);
        }

        public override bool Update(FrameTime frameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
            var hadInteractions = false;
            if (mouseState.LeftButton == ButtonState.Pressed) {
                if (this._previousButtonState == ButtonState.Pressed) {
                    hadInteractions = true;
                    System.Windows.Input.Mouse.OverrideCursor = Cursors.SizeAll;
                    if (this.CurrentAxis == GizmoAxis.Neutral) {
                        var newPosition = keyboardState.IsKeyDown(Keys.LeftControl) ? this.GetSnappedPosition(mousePosition) : mousePosition;
                        this.UpdatePosition(selectedComponent, newPosition);
                    }
                    else if (this.CurrentAxis == GizmoAxis.X) {
                        var newPosition = this.MoveAlongAxis(this.XAxisLineDrawer.StartPoint, this.XAxisLineDrawer.EndPoint, mousePosition) - (this.XAxisLineDrawer.EndPoint - this.XAxisLineDrawer.StartPoint);
                        newPosition = keyboardState.IsKeyDown(Keys.LeftControl) ? this.GetSnappedPosition(newPosition) : newPosition;
                        this.UpdatePosition(selectedComponent, newPosition);
                    }
                    else if (this.CurrentAxis == GizmoAxis.Y) {
                        var newPosition = this.MoveAlongAxis(this.YAxisLineDrawer.StartPoint, this.YAxisLineDrawer.EndPoint, mousePosition) - (this.YAxisLineDrawer.EndPoint - this.YAxisLineDrawer.StartPoint);
                        newPosition = keyboardState.IsKeyDown(Keys.LeftControl) ? this.GetSnappedPosition(newPosition) : newPosition;
                        this.UpdatePosition(selectedComponent, newPosition);
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

        protected override void DrawGizmo(FrameTime frameTime, BaseComponent selectedComponent, BoundingArea viewBoundingArea, float viewRatio, float lineLength) {
            this.ResetEndPoint(selectedComponent, lineLength);

            var scale = viewRatio * 0.25f;
            this._xAxisArrowRenderer.Color = this.XAxisColor;
            this._xAxisArrowRenderer.LocalPosition = this.XAxisLineDrawer.EndPoint;
            this._xAxisArrowRenderer.LocalScale = new Vector2(scale);
            this._xAxisArrowRenderer.Draw(frameTime, viewBoundingArea);

            this._yAxisArrowRenderer.Color = this.YAxisColor;
            this._yAxisArrowRenderer.LocalPosition = this.YAxisLineDrawer.EndPoint;
            this._yAxisArrowRenderer.LocalScale = new Vector2(scale);
            this._yAxisArrowRenderer.Draw(frameTime, viewBoundingArea);

            this._xAxisTriangleRenderer.Color = this.XAxisColor;
            this._xAxisTriangleRenderer.LocalPosition = this.XAxisLineDrawer.StartPoint;
            this._xAxisTriangleRenderer.LocalScale = new Vector2(scale);
            this._xAxisTriangleRenderer.Draw(frameTime, viewBoundingArea);

            this._yAxisTriangleRenderer.Color = this.YAxisColor;
            this._yAxisTriangleRenderer.LocalPosition = this.YAxisLineDrawer.StartPoint;
            this._yAxisTriangleRenderer.LocalScale = new Vector2(-scale);
            this._yAxisTriangleRenderer.Draw(frameTime, viewBoundingArea);

            this._neutralAxisBody.LocalScale = new Vector2(GameSettings.Instance.InversePixelsPerUnit);
            this._xAxisBody.LocalScale = new Vector2(GameSettings.Instance.InversePixelsPerUnit);
            this._yAxisBody.LocalScale = new Vector2(GameSettings.Instance.InversePixelsPerUnit);

            if (selectedComponent is IRotatable rotatable) {
                this._xAxisArrowRenderer.Rotation = rotatable.Rotation;
                this._yAxisArrowRenderer.Rotation = rotatable.Rotation;
                this._xAxisTriangleRenderer.Rotation = rotatable.Rotation;
                this._yAxisTriangleRenderer.Rotation = rotatable.Rotation;
            }
            else {
                this._xAxisArrowRenderer.Rotation = 0f;
                this._yAxisArrowRenderer.Rotation = 0f;
                this._xAxisTriangleRenderer.Rotation = 0f;
                this._yAxisTriangleRenderer.Rotation = 0f;
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

        private Vector2 GetSnappedPosition(Vector2 position) {
            var x = position.X;
            var y = position.Y;

            if (this._statusService.SecondaryGridSize > 0) {
                if (this.CurrentAxis == GizmoAxis.X || this.CurrentAxis == GizmoAxis.Neutral) {
                    x = (int)Math.Round(x / this._statusService.SecondaryGridSize) * this._statusService.SecondaryGridSize;
                }

                if (this.CurrentAxis == GizmoAxis.Y || this.CurrentAxis == GizmoAxis.Neutral) {
                    y = (int)Math.Round(y / this._statusService.SecondaryGridSize) * this._statusService.SecondaryGridSize;
                }

                Console.WriteLine(this._statusService.SecondaryGridSize);
            }

            return new Vector2(x, y);
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