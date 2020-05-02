namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a camera into the game world.
    /// </summary>
    public sealed class Camera : BaseComponent, IBoundable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Matrix> _matrix;
        private Layers _layersToRender = Layers.All;
        private int _renderOrder;
        private SamplerStateType _samplerStateType = SamplerStateType.PointClamp;
        private bool _snapToPixels;
        private float _viewHeight = 10f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        public Camera() : base() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._matrix = new ResettableLazy<Matrix>(this.CreateViewMatrix);
        }

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
        [DataMember(Name = "Layers to Render")]
        public Layers LayersToRender {
            get {
                return this._layersToRender;
            }

            set {
                this.Set(ref this._layersToRender, value);
            }
        }

        /// <summary>
        /// Gets the render order. A lower number will be rendered first.
        /// </summary>
        /// <value>The render order.</value>
        public int RenderOrder {
            get {
                return this._renderOrder;
            }

            set {
                this.Set(ref this._renderOrder, value, true);
            }
        }

        /// <summary>
        /// Gets the state of the sampler.
        /// </summary>
        /// <value>The state of the sampler.</value>
        public SamplerState SamplerState { get; private set; }

        /// <summary>
        /// Gets or sets the type of the sampler state.
        /// </summary>
        /// <value>The type of the sampler state.</value>
        [DataMember(Name = "Sampler State")]
        public SamplerStateType SamplerStateType {
            get {
                return this._samplerStateType;
            }

            set {
                this.Set(ref this._samplerStateType, value);
                this.SamplerState = this._samplerStateType.ToSamplerState();
                this.RaisePropertyChanged(nameof(this.SamplerState));
            }
        }

        /// <summary>
        /// Gets or sets the shader.
        /// </summary>
        /// <value>The shader.</value>
        [DataMember]
        public Shader Shader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this camera should snap to the pixel ratio
        /// defined in <see cref="IGameSettings"/>.
        /// </summary>
        /// <value><c>true</c> if this should snap to pixels; otherwise, <c>false</c>.</value>
        [DataMember(Name = "Snap to Pixels")]
        public bool SnapToPixels {
            get {
                return this._snapToPixels;
            }

            set {
                if (this.Set(ref this._snapToPixels, value)) {
                    this._matrix.Reset();
                    this._boundingArea.Reset();
                }
            }
        }

        /// <summary>
        /// Gets the height of the view.
        /// </summary>
        /// <value>The height of the view.</value>
        [DataMember(Name = "View Height")]
        public float ViewHeight {
            get {
                return this._viewHeight;
            }

            set {
                // This is kind of an arbitrary value, but imagine the chaos if we allowed the
                // camera to reach 0.
                if (value <= 0.1f) {
                    value = 0.1f;
                }

                if (this.Set(ref this._viewHeight, value)) {
                    this._boundingArea.Reset();
                    this._matrix.Reset();
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
            this.SamplerState = this._samplerStateType.ToSamplerState();
            this.PropertyChanged += this.Self_PropertyChanged;
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

            if (this.SnapToPixels) {
                minimumX = minimumX.ToPixelSnappedValue();
                minimumY = minimumY.ToPixelSnappedValue();
                maximumX = maximumX.ToPixelSnappedValue();
                maximumY = maximumY.ToPixelSnappedValue();
            }

            return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }

        private Matrix CreateViewMatrix() {
            var pixelsPerUnit = MacabreGame.Instance.Settings.PixelsPerUnit;
            var viewPort = MacabreGame.Instance.GraphicsDevice.Viewport;
            var origin = new Vector2(viewPort.Width * 0.5f, viewPort.Height * 0.5f);
            var zoom = viewPort.Height / (pixelsPerUnit * this.ViewHeight);
            var worldTransform = this.WorldTransform;

            return
                Matrix.CreateTranslation(new Vector3(-worldTransform.Position.ToPixelSnappedValue() * pixelsPerUnit, 0f)) *
                Matrix.CreateScale(zoom, -zoom, 0f) *
                Matrix.CreateTranslation(new Vector3(origin, 0f));
        }

        private float GetViewWidth(Viewport viewport) {
            var ratio = this.ViewHeight / viewport.Height;
            return viewport.Width * ratio;
        }

        private void Self_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.WorldTransform)) {
                this._boundingArea.Reset();
                this._matrix.Reset();
            }
        }
    }
}