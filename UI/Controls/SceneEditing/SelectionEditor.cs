namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Physics;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class SelectionEditor {
        private readonly EditorGame _editorGame;
        private readonly IComponentSelectionService _selectionService;

        private BoundingAreaDrawer _boundingAreaDrawer = new BoundingAreaDrawer() {
            LineThickness = 0.6f,
            UseDynamicLineThickness = true
        };

        private ColliderDrawer _colliderDrawer = new ColliderDrawer() {
            LineThickness = 0.6f,
            UseDynamicLineThickness = true
        };

        private ButtonState _previousLeftMouseButtonState = ButtonState.Released;
        private TranslateGizmo _translateGizmo;

        public SelectionEditor(EditorGame editorGame) {
            this._editorGame = editorGame;
            this._selectionService = ViewContainer.Resolve<IComponentSelectionService>();
            this._selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
            this._translateGizmo = new TranslateGizmo(editorGame);
        }

        public void Draw(GameTime gameTime, float viewHeight) {
            if (this._editorGame.CurrentScene != null) {
                var contrastingColor = this._editorGame.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite();
                this._boundingAreaDrawer.Color = contrastingColor;
                this._colliderDrawer.Color = contrastingColor;
            }

            this._boundingAreaDrawer.Draw(gameTime, viewHeight);
            this._colliderDrawer.Draw(gameTime, viewHeight);
            this._translateGizmo.Draw(gameTime, viewHeight, this._selectionService.SelectedItem?.Component);
        }

        public void Reinitialize() {
            this._boundingAreaDrawer = new BoundingAreaDrawer() {
                LineThickness = 0.6f,
                UseDynamicLineThickness = true
            };

            this._colliderDrawer = new ColliderDrawer() {
                LineThickness = 0.6f,
                UseDynamicLineThickness = true
            };

            this.ResetDependencies(this._selectionService.SelectedItem);
            this._boundingAreaDrawer.Initialize(this._editorGame.CurrentScene);
            this._colliderDrawer.Initialize(this._editorGame.CurrentScene);
            this._translateGizmo.Initialize();
        }

        public void Update(GameTime gameTime, MouseState mouseState) {
            if (this._editorGame.CurrentScene != null && this._editorGame.CurrentCamera != null) {
                var hadInteractions = false;
                var mousePosition = this._editorGame.CurrentCamera.ConvertPointFromScreenSpaceToWorldSpace(mouseState.Position);

                if (this._selectionService.SelectedItem?.Component != null) {
                    hadInteractions = this._translateGizmo.Update(gameTime, mouseState, mousePosition, this._selectionService.SelectedItem);
                }

                if (hadInteractions) {
                    // We must force the editor to recognize that we've made a change.
                    this._selectionService.SelectedItem.RaisePropertyChanged("Position");
                }
                else if (mouseState.LeftButton == ButtonState.Pressed && this._previousLeftMouseButtonState == ButtonState.Released) {
                    foreach (var drawable in this._editorGame.CurrentScene.GetVisibleDrawableComponents()) {
                        if (drawable.BoundingArea.Contains(mousePosition) && drawable is BaseComponent drawableComponent) {
                            this._selectionService.SelectComponent(drawableComponent);
                        }
                    }
                }
            }

            this._previousLeftMouseButtonState = mouseState.LeftButton;
        }

        private void ResetDependencies(object newValue) {
            if (newValue is Body body) {
                this._boundingAreaDrawer.Boundable = body;
                this._colliderDrawer.Body = body;
            }
            else if (newValue is IBoundable boundable) {
                this._boundingAreaDrawer.Boundable = boundable;
                this._colliderDrawer.Body = null;
            }
            else {
                this._boundingAreaDrawer.Boundable = null;
                this._colliderDrawer.Body = null;
            }
        }

        private void SelectionService_SelectionChanged(object sender, ValueChangedEventArgs<ComponentWrapper> e) {
            if (e.NewValue is ComponentWrapper wrapper) {
                this.ResetDependencies(wrapper.Component);
            }
            else {
                this.ResetDependencies(null);
            }
        }
    }
}