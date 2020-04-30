namespace Macabre2D.UI.CommonLibrary.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Models.FrameworkWrappers;
    using Macabre2D.UI.CommonLibrary.Services;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;

    public sealed class SelectionEditor {
        private readonly IComponentService _componentService;
        private readonly Dictionary<ComponentEditingStyle, IGizmo> _editingStyleToGizmo = new Dictionary<ComponentEditingStyle, IGizmo>();

        private BoundingAreaDrawerComponent _boundingAreaDrawer = new BoundingAreaDrawerComponent() {
            Color = new Color(255, 255, 255, 150),
            LineThickness = 1f,
            UseDynamicLineThickness = true
        };

        private ColliderDrawerComponent _colliderDrawer = new ColliderDrawerComponent() {
            Color = new Color(255, 255, 255, 150),
            LineThickness = 1f,
            UseDynamicLineThickness = true
        };

        private SceneEditor _game;
        private ButtonState _previousLeftMouseButtonState = ButtonState.Released;

        public SelectionEditor(
            IComponentService componentService,
            RotationGizmo rotationGizmo,
            ScaleGizmo scaleGizmo,
            TileGizmo tileGizmo,
            TranslationGizmo translationGizmo) {
            this._componentService = componentService;
            this._componentService.SelectionChanged += this.ComponentService_SelectionChanged;

            this._editingStyleToGizmo.Add(ComponentEditingStyle.Rotation, rotationGizmo);
            this._editingStyleToGizmo.Add(ComponentEditingStyle.Scale, scaleGizmo);
            this._editingStyleToGizmo.Add(ComponentEditingStyle.Tile, tileGizmo);
            this._editingStyleToGizmo.Add(ComponentEditingStyle.Translation, translationGizmo);
        }

        public void Draw(FrameTime frameTime, BoundingArea viewBoundingArea) {
            this._editingStyleToGizmo.TryGetValue(this._game.EditingStyle, out var gizmo);

            if (gizmo?.OverrideSelectionDisplay != true) {
                if (this._game.CurrentScene != null) {
                    var contrastingColor = this._game.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite();
                    this._boundingAreaDrawer.Color = contrastingColor;
                    this._colliderDrawer.Color = contrastingColor;
                }

                if (this._game.ShowSelection) {
                    this._boundingAreaDrawer.Draw(frameTime, viewBoundingArea);
                    this._colliderDrawer.Draw(frameTime, viewBoundingArea);
                }
            }

            gizmo?.Draw(frameTime, viewBoundingArea, this._componentService.SelectedItem?.Component);
        }

        public void Initialize(SceneEditor game) {
            this._game = game;

            this._boundingAreaDrawer = new BoundingAreaDrawerComponent() {
                Color = new Color(255, 255, 255, 150),
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this._colliderDrawer = new ColliderDrawerComponent() {
                Color = new Color(255, 255, 255, 150),
                LineThickness = 1f,
                UseDynamicLineThickness = true
            };

            this.ResetDependencies(this._componentService.SelectedItem);
            this._boundingAreaDrawer.Initialize(this._game.CurrentScene);
            this._colliderDrawer.Initialize(this._game.CurrentScene);

            foreach (var gizmo in this._editingStyleToGizmo.Values) {
                gizmo.Initialize(this._game);
            }
        }

        public void Update(FrameTime frameTime, MouseState mouseState, KeyboardState keyboardState) {
            if (this._game?.CurrentScene != null && this._game.CurrentCamera != null) {
                var hadInteractions = false;
                var mousePosition = this._game.CurrentCamera.ConvertPointFromScreenSpaceToWorldSpace(mouseState.Position);

                if (this._componentService.SelectedItem?.Component != null) {
                    if (this._editingStyleToGizmo.TryGetValue(this._game.EditingStyle, out var gizmo) && gizmo.Update(frameTime, mouseState, keyboardState, mousePosition, this._componentService.SelectedItem)) {
                        if (!string.IsNullOrWhiteSpace(gizmo.EditingPropertyName)) {
                            this._componentService.SelectedItem.RaisePropertyChanged(gizmo.EditingPropertyName);
                        }

                        hadInteractions = true;
                    }
                }

                if (!hadInteractions && mouseState.LeftButton == ButtonState.Pressed && this._previousLeftMouseButtonState == ButtonState.Released) {
                    this._componentService.SelectComponent(null);
                    foreach (var drawable in this._game.CurrentScene.GetVisibleDrawableComponents()) {
                        if (drawable.BoundingArea.Contains(mousePosition) && drawable is BaseComponent drawableComponent) {
                            if (!(drawableComponent is ITileable tileable) || tileable.HasActiveTileAt(mousePosition)) {
                                this._componentService.SelectComponent(drawableComponent);
                            }
                        }
                    }
                }
            }

            this._previousLeftMouseButtonState = mouseState.LeftButton;
        }

        private void ComponentService_SelectionChanged(object sender, ValueChangedEventArgs<ComponentWrapper> e) {
            if (e.NewValue is ComponentWrapper wrapper) {
                this.ResetDependencies(wrapper.Component);
            }
            else {
                this.ResetDependencies(null);
            }
        }

        private void ResetDependencies(object newValue) {
            if (newValue is IPhysicsBody body) {
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
    }
}