namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Diagnostics;
    using Macabre2D.Framework.Physics;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;

    public sealed class SelectionDrawer {
        private readonly SceneEditor _sceneEditor;
        private readonly ISelectionService _selectionService;

        private BoundingAreaDrawer _boundingAreaDrawer = new BoundingAreaDrawer() {
            Color = Color.Red,
            LineThickness = 1,
            UseDynamicLineThickness = true
        };

        private ColliderDrawer _colliderDrawer = new ColliderDrawer() {
            Color = Color.Green,
            LineThickness = 1,
            UseDynamicLineThickness = true
        };

        public SelectionDrawer(SceneEditor sceneEditor) {
            this._sceneEditor = sceneEditor;
            this._selectionService = ViewContainer.Resolve<ISelectionService>();
            this._selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
        }

        public void Draw(GameTime gameTime, float viewHeight) {
            this._boundingAreaDrawer.Draw(gameTime, viewHeight);
            this._colliderDrawer.Draw(gameTime, viewHeight);
        }

        public void Reinitialize() {
            this._boundingAreaDrawer = new BoundingAreaDrawer() {
                Color = Color.Red,
                LineThickness = 1,
                UseDynamicLineThickness = true
            };

            this._colliderDrawer = new ColliderDrawer() {
                Color = Color.Green,
                LineThickness = 1,
                UseDynamicLineThickness = true
            };

            this.ResetDependencies(this._selectionService.SelectedItem);
            this._boundingAreaDrawer.Initialize(this._sceneEditor.CurrentScene);
            this._colliderDrawer.Initialize(this._sceneEditor.CurrentScene);
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

        private void SelectionService_SelectionChanged(object sender, ValueChangedEventArgs<object> e) {
            if (e.NewValue is ComponentWrapper wrapper) {
                this.ResetDependencies(wrapper.Component);
            }
            else {
                this.ResetDependencies(null);
            }
        }
    }
}