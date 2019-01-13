namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Physics;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;

    public sealed class SelectionDrawer {
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

        public SelectionDrawer(EditorGame editorGame) {
            this._editorGame = editorGame;
            this._selectionService = ViewContainer.Resolve<IComponentSelectionService>();
            this._selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
        }

        public void Draw(GameTime gameTime, float viewHeight) {
            if (this._editorGame.CurrentScene != null) {
                var contrastingColor = this._editorGame.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite();
                this._boundingAreaDrawer.Color = contrastingColor;
                this._colliderDrawer.Color = contrastingColor;
            }

            this._boundingAreaDrawer.Draw(gameTime, viewHeight);
            this._colliderDrawer.Draw(gameTime, viewHeight);
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