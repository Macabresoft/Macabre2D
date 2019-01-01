namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using MonoGame.Framework.WpfInterop.Input;

    internal sealed class EditorCameraWrapper : NotifyPropertyChanged {
        private readonly SceneEditor _sceneEditor;
        private readonly ISceneService _sceneService;
        private Camera _camera;
        private GridDrawer _gridDrawer;

        private int _previousScrollWheelValue = 0;

        internal EditorCameraWrapper(SceneEditor sceneEditor) {
            this._sceneEditor = sceneEditor;
            this._sceneService = ViewContainer.Resolve<ISceneService>();
        }

        internal Camera Camera {
            get {
                return this._camera;
            }

            set {
                if (this.Set(ref this._camera, value) && this._camera != null && this._sceneEditor.CurrentScene != null && this._sceneEditor.IsInitialized) {
                    this._camera.Initialize(this._sceneEditor.CurrentScene);
                    this._gridDrawer = new GridDrawer() {
                        Camera = this._camera,
                        Color = new Color(255, 255, 255, 100),
                        LineThickness = 1f,
                        UseDynamicLineThickness = true
                    };

                    this._gridDrawer.Initialize(this._sceneEditor.CurrentScene);
                }
            }
        }

        internal void Draw(GameTime gameTime) {
            this._gridDrawer.Draw(gameTime, this._camera.ViewHeight);
        }

        internal void RefreshCamera() {
            this.Camera = this._sceneService.CurrentScene?.SceneAsset?.Camera ?? new Camera();
            if (this._sceneEditor.IsInitialized && this._sceneEditor.CurrentScene != null) {
                this.Camera.Initialize(this._sceneEditor.CurrentScene);
            }
        }

        internal void Update(GameTime gameTime, WpfMouse mouse, WpfKeyboard keyboard) {
            var mouseState = mouse.GetState();
            if (this.IsMouseInsideEditor(mouseState)) {
                if (mouseState.ScrollWheelValue != this._previousScrollWheelValue) {
                    var scrollViewChange = (float)(gameTime.ElapsedGameTime.TotalSeconds * (this._previousScrollWheelValue - mouseState.ScrollWheelValue)) * 2f;

                    var isZoomIn = scrollViewChange < 0;
                    if (isZoomIn) {
                        this.Camera.ZoomTo(mouseState.Position, scrollViewChange * -1f);
                    }
                    else {
                        this.Camera.ViewHeight += scrollViewChange;
                    }

                    this._previousScrollWheelValue = mouseState.ScrollWheelValue;
                }

                if (this._sceneEditor.IsFocused) {
                    var keyboardState = keyboard.GetState();
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
        }

        private bool IsMouseInsideEditor(MouseState mouseState) {
            var mousePosition = new Point(mouseState.X, mouseState.Y);
            return this._sceneEditor.GraphicsDevice.Viewport.Bounds.Contains(mousePosition);
        }
    }
}