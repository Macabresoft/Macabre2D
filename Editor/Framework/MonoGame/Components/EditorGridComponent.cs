namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public sealed class EditorGridComponent : BaseDrawerComponent {
        private CameraComponent _camera;

        /// <inheritdoc />
        public override BoundingArea BoundingArea => _camera?.BoundingArea ?? BoundingArea.Empty;

        public byte MajorGridSize { get; set; } = 5;
        public byte NumberOfDivisions { get; set; } = 5;

        public override void Initialize(IGameEntity entity) {
            if (!GameScene.IsNullOrEmpty(this.Entity.Scene)) {
                this.Entity.Scene.PropertyChanged -= this.Scene_PropertyChanged;
            }

            base.Initialize(entity);

            this.UseDynamicLineThickness = true;
            if (!this.Entity.TryGetComponent(out this._camera)) {
                throw new ArgumentNullException(nameof(this._camera));
            }

            this.Entity.Scene.PropertyChanged += this.Scene_PropertyChanged;
            this.ResetColor();
        }

        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer == null) {
                return;
            }

            if (this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                if (this.MajorGridSize > 0) {
                    var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                    if (this.NumberOfDivisions > 0) {
                        var minorGridSize = this.MajorGridSize / this.NumberOfDivisions;
                        this.DrawGrid(spriteBatch, viewBoundingArea, minorGridSize, lineThickness, 0.2f);
                    }

                    this.DrawGrid(spriteBatch, viewBoundingArea, this.MajorGridSize, lineThickness, 0.5f);
                }
            }
        }

        private void DrawGrid(SpriteBatch spriteBatch, BoundingArea boundingArea, float gridSize, float lineThickness, float alpha) {
            var columns = GridDrawerComponent.GetGridPositions(boundingArea.Minimum.X, boundingArea.Maximum.X, gridSize, 0f);
            var color = this.Color * alpha;
            foreach (var column in columns) {
                this.PrimitiveDrawer.DrawLine(
                    spriteBatch,
                    new Vector2(column, boundingArea.Minimum.Y),
                    new Vector2(column, boundingArea.Maximum.Y),
                    color,
                    lineThickness);
            }

            var rows = GridDrawerComponent.GetGridPositions(boundingArea.Minimum.Y, boundingArea.Maximum.Y, gridSize, 0f);
            foreach (var row in rows) {
                this.PrimitiveDrawer.DrawLine(
                    spriteBatch,
                    new Vector2(boundingArea.Minimum.X, row),
                    new Vector2(boundingArea.Maximum.X, row),
                    color,
                    lineThickness);
            }
        }

        private void ResetColor() {
            this.Color = this.Entity.Scene.BackgroundColor.GetContrastingBlackOrWhite();
        }

        private void Scene_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameScene.BackgroundColor)) {
                this.ResetColor();
            }
        }
    }
}