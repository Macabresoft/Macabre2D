namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Physics;
    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class TranslateGizmo : BaseGizmo {

        private readonly Body _neutralAxisBody = new Body() {
            Collider = new CircleCollider(1f, RadiusScalingType.X)
        };

        private readonly SpriteRenderer _xAxisArrowRenderer = new SpriteRenderer();
        private readonly SpriteRenderer _xAxisTriangleRenderer = new SpriteRenderer();
        private readonly SpriteRenderer _yAxisArrowRenderer = new SpriteRenderer();
        private readonly SpriteRenderer _yAxisTriangleRenderer = new SpriteRenderer();
        private ButtonState _previousButtonState = ButtonState.Released;
        private IUndoService _undoService;
        private Vector2 _unmovedWorldPosition;
        private bool _wasMoved;

        public TranslateGizmo(EditorGame editorGame) : base(editorGame) {
            this._undoService = ViewContainer.Resolve<IUndoService>();
        }

        public override void Initialize() {
            base.Initialize();
            var arrowSprite = PrimitiveDrawer.CreateArrowSprite(this.EditorGame.GraphicsDevice, 64);
            this._xAxisArrowRenderer.Sprite = arrowSprite;
            this._yAxisArrowRenderer.Sprite = arrowSprite;
            this._xAxisArrowRenderer.OffsetType = OffsetType.Center;
            this._yAxisArrowRenderer.OffsetType = OffsetType.Center;
            this._xAxisArrowRenderer.Initialize(this.EditorGame.CurrentScene);
            this._yAxisArrowRenderer.Initialize(this.EditorGame.CurrentScene);

            var triangleSprite = PrimitiveDrawer.CreateRightTriangleSprite(this.EditorGame.GraphicsDevice, new Point(64));
            this._xAxisTriangleRenderer.Sprite = triangleSprite;
            this._yAxisTriangleRenderer.Sprite = triangleSprite;
            this._xAxisTriangleRenderer.OffsetType = OffsetType.Center;
            this._yAxisTriangleRenderer.OffsetType = OffsetType.Center;
            this._xAxisTriangleRenderer.AddChild(this._neutralAxisBody);
            this._xAxisTriangleRenderer.Initialize(this.EditorGame.CurrentScene);
            this._yAxisTriangleRenderer.Initialize(this.EditorGame.CurrentScene);
        }

        public override bool Update(GameTime gameTime, MouseState mouseState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
            var hadInteractions = false;
            if (mouseState.LeftButton == ButtonState.Pressed) {
                if (this._previousButtonState == ButtonState.Pressed) {
                    selectedComponent.Component.SetWorldPosition(mousePosition);
                    selectedComponent.UpdateProperty(nameof(selectedComponent.Component.LocalPosition), selectedComponent.Component.LocalPosition);
                    hadInteractions = true;
                    this._wasMoved = true;
                }
                else {
                    if (this._neutralAxisBody.Collider.Contains(mousePosition)) {
                        this._previousButtonState = ButtonState.Pressed;
                        this._unmovedWorldPosition = selectedComponent.Component.WorldTransform.Position;
                        hadInteractions = true;
                    }
                }
            }
            else {
                if (this._wasMoved) {
                    var originalPosition = this._unmovedWorldPosition;
                    var newPosition = selectedComponent.Component.WorldTransform.Position;
                    var undoCommand = new UndoCommand(() => {
                        selectedComponent.Component.SetWorldPosition(newPosition);
                        selectedComponent.UpdateProperty(nameof(selectedComponent.Component.LocalPosition), selectedComponent.Component.LocalPosition);
                    },
                    () => {
                        selectedComponent.Component.SetWorldPosition(originalPosition);
                        selectedComponent.UpdateProperty(nameof(selectedComponent.Component.LocalPosition), selectedComponent.Component.LocalPosition);
                    });

                    this._undoService.Do(undoCommand);
                    this._wasMoved = false;
                }

                this._previousButtonState = ButtonState.Released;
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
    }
}