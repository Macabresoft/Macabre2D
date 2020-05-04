namespace Macabre2D.UI.GameEditorLibrary.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using Macabre2D.UI.GameEditorLibrary.Services;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    public sealed class EditorCameraWrapper : NotifyPropertyChanged {
        private readonly ISceneService _sceneService;
        private readonly IStatusService _statusService;
        private Camera _camera = new Camera();
        private SceneEditor _game;
        private MouseState _previousMouseState = new MouseState();
        private GridDrawerComponent _primaryGridDrawer;
        private GridDrawerComponent _secondaryGridDrawer;
        private LineDrawerComponent _xAxisDrawer;
        private LineDrawerComponent _yAxisDrawer;

        public EditorCameraWrapper(ISceneService sceneService, IStatusService statusService) {
            this._sceneService = sceneService;
            this._statusService = statusService;
        }

        public Camera Camera {
            get {
                return this._camera;
            }

            private set {
                if (value == null) {
                    value = new Camera();
                }

                var oldCamera = this._camera;
                if (this.Set(ref this._camera, value)) {
                    if (oldCamera != null) {
                        oldCamera.PropertyChanged -= this.Camera_ViewHeightChanged;
                    }

                    if (this._camera != null) {
                        this._camera.PropertyChanged += this.Camera_ViewHeightChanged;
                    }

                    this.ResetStatusProperties();
                }
            }
        }

        public void Draw(FrameTime frameTime) {
            if (this._game.ShowGrid && this._game.EditingStyle != ComponentEditingStyle.Tile && this._primaryGridDrawer != null && this._secondaryGridDrawer != null) {
                if (this._game.CurrentScene != null) {
                    var contrastingColor = this._game.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite();
                    this._primaryGridDrawer.Color = new Color(contrastingColor, 60);
                    this._secondaryGridDrawer.Color = new Color(contrastingColor, 30);
                }

                this._primaryGridDrawer.Draw(frameTime, this._camera.BoundingArea);
                this._secondaryGridDrawer.Draw(frameTime, this._camera.BoundingArea);
                this._xAxisDrawer.Draw(frameTime, this._camera.BoundingArea);
                this._yAxisDrawer.Draw(frameTime, this._camera.BoundingArea);
            }
        }

        public void Initialize(SceneEditor game) {
            this._game = game;
            this.Camera = this._sceneService.CurrentScene?.Camera;
            this.Camera.Initialize(this._game.CurrentScene);

            var gridSize = this.GetGridSize();
            this._primaryGridDrawer = new GridDrawerComponent() {
                Camera = this._camera,
                Color = new Color(255, 255, 255, 100),
                Grid = new TileGrid(new Vector2(gridSize)),
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            var smallGridSize = gridSize / 2f;
            this._secondaryGridDrawer = new GridDrawerComponent() {
                Camera = this._camera,
                Color = new Color(255, 255, 255, 75),
                Grid = new TileGrid(new Vector2(smallGridSize)),
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this._xAxisDrawer = new LineDrawerComponent() {
                Color = new Color(Color.Red, 0.5f),
                LineThickness = 1f,
                UseDynamicLineThickness = true,
                StartPoint = Vector2.Zero,
                EndPoint = new Vector2(100f, 0f)
            };

            this._yAxisDrawer = new LineDrawerComponent() {
                Color = new Color(Color.Green, 0.5f),
                LineThickness = 1f,
                UseDynamicLineThickness = true,
                StartPoint = Vector2.Zero,
                EndPoint = new Vector2(0f, 100f)
            };

            this._primaryGridDrawer.Initialize(this._game.CurrentScene);
            this._secondaryGridDrawer.Initialize(this._game.CurrentScene);
            this._xAxisDrawer.Initialize(this._game.CurrentScene);
            this._yAxisDrawer.Initialize(this._game.CurrentScene);
            this.ResetStatusProperties();
        }

        public void Update(FrameTime frameTime, MouseState mouseState, KeyboardState keyboardState) {
            if (mouseState.ScrollWheelValue != this._previousMouseState.ScrollWheelValue) {
                var scrollViewChange = (float)(frameTime.SecondsPassed * (this._previousMouseState.ScrollWheelValue - mouseState.ScrollWheelValue) * Math.Sqrt(this._camera.ViewHeight) * 0.25f);

                var isZoomIn = scrollViewChange < 0;
                if (isZoomIn) {
                    this.Camera.ZoomTo(mouseState.Position, scrollViewChange * -1f);
                }
                else {
                    this.Camera.ViewHeight += scrollViewChange;
                }
            }

            ////if (this._game.IsMouseOver) {
            if (mouseState.MiddleButton == ButtonState.Pressed && this._previousMouseState.MiddleButton == ButtonState.Pressed) {
                if (mouseState.Position != this._previousMouseState.Position) {
                    var oldPosition = this.Camera.ConvertPointFromScreenSpaceToWorldSpace(this._previousMouseState.Position);
                    var mousePosition = this.Camera.ConvertPointFromScreenSpaceToWorldSpace(mouseState.Position);

                    var distance = oldPosition - mousePosition;
                    this.Camera.LocalPosition += distance;
                }

                if (System.Windows.Input.Mouse.OverrideCursor == null) {
                    System.Windows.Input.Mouse.OverrideCursor = Cursors.Hand;
                }
            }
            else if (System.Windows.Input.Mouse.OverrideCursor == Cursors.Hand) {
                System.Windows.Input.Mouse.OverrideCursor = null;
            }

            if (!keyboardState.IsModifierKeyDown()) {
                var movementMultiplier = (float)frameTime.SecondsPassed * this.Camera.ViewHeight;
                if (keyboardState.IsKeyDown(Keys.W)) {
                    this.Camera.LocalPosition += new Vector2(0f, movementMultiplier);
                }

                if (keyboardState.IsKeyDown(Keys.A)) {
                    this.Camera.LocalPosition += new Vector2(movementMultiplier * -1f, 0f);
                }

                if (keyboardState.IsKeyDown(Keys.S)) {
                    this.Camera.LocalPosition += new Vector2(0f, movementMultiplier * -1f);
                }

                if (keyboardState.IsKeyDown(Keys.D)) {
                    this.Camera.LocalPosition += new Vector2(movementMultiplier, 0f);
                }
            }
            ////}
            ////else {
            ////    System.Windows.Input.Mouse.OverrideCursor = null;
            ////}

            this._previousMouseState = mouseState;
        }

        private void Camera_ViewHeightChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Camera.ViewHeight)) {
                if (this._primaryGridDrawer != null && this._primaryGridDrawer != null) {
                    var gridSize = this.GetGridSize();
                    this._primaryGridDrawer.Grid = new TileGrid(new Vector2(gridSize));

                    var smallGridSize = gridSize / 2f;
                    this._secondaryGridDrawer.Grid = new TileGrid(new Vector2(smallGridSize));
                }

                this.ResetStatusProperties();
            }
        }

        private int GetGridSize() {
            var gridSize = 1;
            var currentMultiple = 4;
            while (currentMultiple < this.Camera.ViewHeight) {
                gridSize = currentMultiple / 4;
                currentMultiple *= 2;
            }

            return gridSize;
        }

        private void ResetStatusProperties() {
            if (this._camera != null) {
                this._statusService.ViewHeight = this._camera.ViewHeight;
                this._statusService.ViewWidth = this._camera.GetViewWidth();
            }
            else {
                this._statusService.ViewHeight = 0f;
                this._statusService.ViewWidth = 0f;
            }

            this._statusService.PrimaryGridSize = this._primaryGridDrawer != null ? this._primaryGridDrawer.Grid.TileSize.X : 0;
            this._statusService.SecondaryGridSize = this._secondaryGridDrawer != null ? this._secondaryGridDrawer.Grid.TileSize.X : 0f;
        }
    }
}