namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a camera into the game world.
    /// </summary>
    public sealed class Camera : BaseComponent, IBoundable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Matrix> _matrix;
        private int _renderOrder;
        private float _viewHeight = 10f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        public Camera() : base() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._matrix = new ResettableLazy<Matrix>(this.CreateViewMatrix);
        }

        /// <summary>
        /// Occurs when the render order has changed.
        /// </summary>
        public event EventHandler RenderOrderChanged;

        /// <summary>
        /// Occurs when the view height has changed.
        /// </summary>
        public event EventHandler ViewHeightChanged;

        /// <inheritdoc/>
        public BoundingArea BoundingArea {
            get {
                return this._boundingArea.Value;
            }
        }

        /// <summary>
        /// Gets the layers to render.
        /// </summary>
        /// <value>The layers to render.</value>
        [DataMember]
        public Layers LayersToRender { get; set; } = Layers.All;

        /// <summary>
        /// Gets the render order. A lower number will be rendered first.
        /// </summary>
        /// <value>The render order.</value>
        public int RenderOrder {
            get {
                return this._renderOrder;
            }

            set {
                if (this._renderOrder != value) {
                    this._renderOrder = value;
                    this.RenderOrderChanged.SafeInvoke(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the shader.
        /// </summary>
        /// <value>The shader.</value>
        [DataMember]
        public Shader Shader { get; set; }

        /// <summary>
        /// Gets the height of the view.
        /// </summary>
        /// <value>The height of the view.</value>
        [DataMember]
        public float ViewHeight {
            get {
                return this._viewHeight;
            }

            set {
                // If someone wants to view less than one unit, they need to change their pixel
                // density. And if they want to view less than one pixel, they need to use a
                // different engine.
                if (value <= 1f) {
                    value = 1f;
                }

                if (value != this._viewHeight) {
                    this._viewHeight = value;
                    this._boundingArea.Reset();
                    this._matrix.Reset();
                    this.ViewHeightChanged.SafeInvoke(this);
                }
            }
        }

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <value>The view matrix.</value>
        public Matrix ViewMatrix {
            get {
                return this._matrix.Value;
            }
        }

        /// <summary>
        /// Converts the point from screen space to world space.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The world space location of the point.</returns>
        public Vector2 ConvertPointFromScreenSpaceToWorldSpace(Point point) {
            if (MacabreGame.Instance.GraphicsDevice is GraphicsDevice graphicsDevice) {
                var ratio = this.ViewHeight / graphicsDevice.Viewport.Height;
                var pointVector = point.ToVector2();
                var relativeY = graphicsDevice.Viewport.Height - pointVector.Y;
                var vectorPosition = new Vector2(pointVector.X - 0.5f * graphicsDevice.Viewport.Width, relativeY - 0.5f * graphicsDevice.Viewport.Height) * ratio;
                return vectorPosition + this.WorldTransform.Position;
            }

            return Vector2.Zero;
        }

        /// <summary>
        /// Gets the width of the view.
        /// </summary>
        /// <returns>The width of the view.</returns>
        public float GetViewWidth() {
            return this.GetViewWidth(MacabreGame.Instance.GraphicsDevice.Viewport);
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this.Scene.IsInitialized) {
                this.Shader?.Load();
            }

            base.LoadContent();
        }

        /// <summary>
        /// Zooms to a world position.
        /// </summary>
        /// <param name="worldPosition">The world position.</param>
        /// <param name="zoomAmount">The zoom amount.</param>
        public void ZoomTo(Vector2 worldPosition, float zoomAmount) {
            var originalCameraPosition = this.WorldTransform.Position;
            var originalDistanceFromCamera = worldPosition - originalCameraPosition;
            var originalViewHeight = this.ViewHeight;
            this.ViewHeight -= zoomAmount;
            var viewHeightRatio = this.ViewHeight / originalViewHeight;
            this.SetWorldPosition(worldPosition - (originalDistanceFromCamera * viewHeightRatio));
        }

        /// <summary>
        /// Zooms to a screen position.
        /// </summary>
        /// <param name="screenPosition">The screen position.</param>
        /// <param name="zoomAmount">The zoom amount.</param>
        public void ZoomTo(Point screenPosition, float zoomAmount) {
            var worldPosition = this.ConvertPointFromScreenSpaceToWorldSpace(screenPosition);
            this.ZoomTo(worldPosition, zoomAmount);
        }

        /// <summary>
        /// Zooms to a boundable component, fitting it into frame.
        /// </summary>
        /// <param name="boundable">The boundable.</param>
        public void ZoomTo(IBoundable boundable) {
            if (boundable != null) {
                var area = boundable.BoundingArea;
                var difference = area.Maximum - area.Minimum;
                var origin = area.Minimum + difference * 0.5f;

                this.SetWorldPosition(origin);

                this.ViewHeight = difference.Y;
                var currentViewWidth = this.GetViewWidth();
                if (currentViewWidth < difference.X) {
                    this.ViewHeight *= difference.X / currentViewWidth;
                }
            }
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            this._boundingArea.Reset();
            this._matrix.Reset();
            this.TransformChanged += this.Self_TransformChanged;
        }

        private BoundingArea CreateBoundingArea() {
            var viewPort = MacabreGame.Instance.GraphicsDevice.Viewport;
            var ratio = this.ViewHeight / viewPort.Height;
            var halfHeight = this.ViewHeight * 0.5f;
            var halfWidth = this.GetViewWidth(MacabreGame.Instance.GraphicsDevice.Viewport) * 0.5f;

            var points = new List<Vector2> {
                this.GetWorldTransform(new Vector2(-halfWidth, -halfHeight)).Position,
                this.GetWorldTransform(new Vector2(-halfWidth, halfHeight)).Position,
                this.GetWorldTransform(new Vector2(halfWidth, halfHeight)).Position,
                this.GetWorldTransform(new Vector2(halfWidth, -halfHeight)).Position
            };

            var minimumX = points.Min(x => x.X);
            var minimumY = points.Min(x => x.Y);
            var maximumX = points.Max(x => x.X);
            var maximumY = points.Max(x => x.Y);

            return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }

        private Matrix CreateViewMatrix() {
            var pixelsPerUnit = MacabreGame.Instance.Settings.PixelsPerUnit;
            var viewPort = MacabreGame.Instance.GraphicsDevice.Viewport;
            var origin = new Vector2(viewPort.Width * 0.5f, viewPort.Height * 0.5f);
            var zoom = viewPort.Height / (pixelsPerUnit * this.ViewHeight);
            var worldTransform = this.WorldTransform;

            return
                Matrix.CreateTranslation(new Vector3(-worldTransform.Position * pixelsPerUnit, 0f)) *
                Matrix.CreateScale(zoom, -zoom, 0f) *
                Matrix.CreateTranslation(new Vector3(origin, 0f));
        }

        private float GetViewWidth(Viewport viewport) {
            var ratio = this.ViewHeight / viewport.Height;
            return viewport.Width * ratio;
        }

        private void Self_TransformChanged(object sender, EventArgs e) {
            this._boundingArea.Reset();
            this._matrix.Reset();
        }
    }
}