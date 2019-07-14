namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class EditorCameraWrapper : NotifyPropertyChanged {
        private readonly ISceneService _sceneService;
        private Camera _camera = new Camera();
        private EditorGame _game;
        private int _previousScrollWheelValue = 0;
        private GridDrawer _primaryGridDrawer;
        private GridDrawer _secondaryGridDrawer;

        public EditorCameraWrapper(ISceneService sceneService) {
            this._sceneService = sceneService;
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
                        oldCamera.ViewHeightChanged -= this.Camera_ViewHeightChanged;
                    }

                    if (this._camera != null) {
                        this._camera.ViewHeightChanged += this.Camera_ViewHeightChanged;
                    }
                }
            }
        }

        public void Draw(GameTime gameTime) {
            if (this._game.ShowGrid && this._game.EditingStyle != ComponentEditingStyle.Tile && this._primaryGridDrawer != null && this._secondaryGridDrawer != null) {
                if (this._game.CurrentScene != null) {
                    var contrastingColor = this._game.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite();
                    this._primaryGridDrawer.Color = new Color(contrastingColor, 60);
                    this._secondaryGridDrawer.Color = new Color(contrastingColor, 30);
                }

                this._primaryGridDrawer.Draw(gameTime, this._camera.BoundingArea);
                this._secondaryGridDrawer.Draw(gameTime, this._camera.BoundingArea);
            }
        }

        public void Initialize(EditorGame game) {
            this._game = game;
            this.Camera = this._sceneService.CurrentScene?.SceneAsset?.Camera;
            this.Camera.Initialize(this._game.CurrentScene);

            var gridSize = this.GetGridSize();
            this._primaryGridDrawer = new GridDrawer() {
                Camera = this._camera,
                Color = new Color(255, 255, 255, 100),
                Grid = new TileGrid(new Vector2(gridSize)),
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this._primaryGridDrawer.Initialize(this._game.CurrentScene);

            var smallGridSize = gridSize / 2f;
            this._secondaryGridDrawer = new GridDrawer() {
                Camera = this._camera,
                Color = new Color(255, 255, 255, 75),
                Grid = new TileGrid(new Vector2(smallGridSize)),
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this._secondaryGridDrawer.Initialize(this._game.CurrentScene);
        }

        public void Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState) {
            if (mouseState.ScrollWheelValue != this._previousScrollWheelValue) {
                var scrollViewChange = (float)(gameTime.ElapsedGameTime.TotalSeconds * (this._previousScrollWheelValue - mouseState.ScrollWheelValue)) * 5f;

                var isZoomIn = scrollViewChange < 0;
                if (isZoomIn) {
                    this.Camera.ZoomTo(mouseState.Position, scrollViewChange * -1f);
                }
                else {
                    this.Camera.ViewHeight += scrollViewChange;
                }

                this._previousScrollWheelValue = mouseState.ScrollWheelValue;
            }

            if (this._game.IsFocused && !keyboardState.IsModifierKeyDown()) {
                var movementMultiplier = (float)gameTime.ElapsedGameTime.TotalSeconds * this.Camera.ViewHeight;
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
        }

        private void Camera_ViewHeightChanged(object sender, System.EventArgs e) {
            if (this._primaryGridDrawer != null) {
                var gridSize = this.GetGridSize();
                this._primaryGridDrawer.Grid = new TileGrid(new Vector2(gridSize));

                var smallGridSize = gridSize / 2f;
                this._secondaryGridDrawer.Grid = new TileGrid(new Vector2(smallGridSize));
            }
        }

        private int GetGridSize() {
            var gridSize = 1;
            var currentMultiple = 4;
            while (currentMultiple < this.Camera.ViewHeight) {
                gridSize = currentMultiple / 4;
                currentMultiple = currentMultiple * 2;
            }

            return gridSize;
        }
    }
}