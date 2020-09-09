namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a camera into the game world.
    /// </summary>
    public sealed class CameraComponent : GameComponent, IBoundable, IGameCameraComponent {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Matrix> _matrix;
        private Layers _layersToRender = Layers.All;
        private int _renderOrder;
        private SamplerStateType _samplerStateType = SamplerStateType.PointClamp;
        private bool _snapToPixels;
        private float _viewHeight = 10f;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraComponent" /> class.
        /// </summary>
        public CameraComponent() : base() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._matrix = new ResettableLazy<Matrix>(this.CreateViewMatrix);
        }

        /// <inheritdoc />
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
        /// Gets the offset settings.
        /// </summary>
        /// <value>The offset settings.</value>
        [DataMember(Name = "Offset Settings")]
        public OffsetSettings OffsetSettings { get; } = new OffsetSettings(Vector2.Zero, PixelOffsetType.Center);

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
        public SamplerState SamplerState { get; private set; } = SamplerState.PointClamp;

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
        public Shader? Shader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this camera should snap to the pixel ratio
        /// defined in <see cref="IGameSettings" />.
        /// </summary>
        /// <value><c>true</c> if this should snap to pixels; otherwise, <c>false</c>.</value>
        [DataMember(Name = "Snap to Pixels")]
        public bool SnapToPixels {
            get {
                return this._snapToPixels;
            }

            set {
                if (this.Set(ref this._snapToPixels, value)) {
                    this.ResetLazyValues();
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
                    this.ResetLazyValues();
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
            var result = Vector2.Zero;

            if (this.OffsetSettings.Size.Y != 0f) {
                var ratio = this.ViewHeight / this.OffsetSettings.Size.Y;
                var pointVector = point.ToVector2();
                var vectorPosition = new Vector2(pointVector.X + this.OffsetSettings.Offset.X, -pointVector.Y + this.OffsetSettings.Size.Y + this.OffsetSettings.Offset.Y) * ratio;
                result = this.Entity.GetWorldTransform(vectorPosition).Position;
            }

            return result;
        }

        /// <summary>
        /// Gets the width of the view.
        /// </summary>
        /// <returns>The width of the view.</returns>
        public float GetViewWidth() {
            var size = this.OffsetSettings.Size;
            var ratio = size.Y != 0 ? this.ViewHeight / this.OffsetSettings.Size.Y : 0f;
            return size.X * ratio;
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.OffsetSettings.Initialize(this.CreateSize);
            this.ResetLazyValues();
            this.SamplerState = this._samplerStateType.ToSamplerState();

            this.Entity.Scene.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
            this.OffsetSettings.PropertyChanged += this.OffsetSettings_PropertyChanged;

            this.Shader?.Load();
        }

        /// <summary>
        /// Zooms to a world position.
        /// </summary>
        /// <param name="worldPosition">The world position.</param>
        /// <param name="zoomAmount">The zoom amount.</param>
        public void ZoomTo(Vector2 worldPosition, float zoomAmount) {
            var originalCameraPosition = this.Entity.Transform.Position;
            var originalDistanceFromCamera = worldPosition - originalCameraPosition;
            var originalViewHeight = this.ViewHeight;
            this.ViewHeight -= zoomAmount;
            var viewHeightRatio = this.ViewHeight / originalViewHeight;
            this.Entity.SetWorldPosition(worldPosition - (originalDistanceFromCamera * viewHeightRatio));
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

                this.Entity.SetWorldPosition(origin);

                this.ViewHeight = difference.Y;
                var currentViewWidth = this.GetViewWidth();
                if (currentViewWidth < difference.X) {
                    this.ViewHeight *= difference.X / currentViewWidth;
                }
            }
        }

        /// <inheritdoc />
        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameEntity.Transform)) {
                this.ResetLazyValues();
            }
        }

        private BoundingArea CreateBoundingArea() {
            var ratio = this.ViewHeight / this.OffsetSettings.Size.Y;
            var width = this.OffsetSettings.Size.X * ratio;
            var height = this.OffsetSettings.Size.Y * ratio;
            var offset = this.OffsetSettings.Offset * ratio;

            var points = new List<Vector2> {
                    this.Entity.GetWorldTransform(offset).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(width, 0f)).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(width, height)).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(0f, height)).Position
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

        private Vector2 CreateSize() {
            return new Vector2(this.Entity.Scene.Game.ViewportSize.X, this.Entity.Scene.Game.ViewportSize.Y);
        }

        private Matrix CreateViewMatrix() {
            var pixelsPerUnit = this.Entity.Scene.Game.Settings.PixelsPerUnit;
            var zoom = this.OffsetSettings.Size.Y / (pixelsPerUnit * this.ViewHeight);
            var worldTransform = this.Entity.Transform;

            return
                Matrix.CreateTranslation(new Vector3(-worldTransform.Position.ToPixelSnappedValue() * pixelsPerUnit, 0f)) *
                Matrix.CreateScale(zoom, -zoom, 0f) *
                Matrix.CreateTranslation(new Vector3(-this.OffsetSettings.Offset.X, this.OffsetSettings.Size.Y + this.OffsetSettings.Offset.Y, 0f));
        }

        private void Game_ViewportSizeChanged(object sender, Point e) {
            this.OffsetSettings.InvalidateSize();
            this.ResetLazyValues();
        }

        private void OffsetSettings_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.OffsetSettings.Offset)) {
                this._boundingArea.Reset();
            }
        }

        private void ResetLazyValues() {
            this._boundingArea.Reset();
            this._matrix.Reset();
        }
    }
}