namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component which will render a single sprite.
    /// </summary>
    public class SpriteRenderComponent : GameRenderableComponent, IAssetComponent<Sprite>, IRotatable {
        private readonly ResettableLazy<BoundingArea> _boundingArea;
        private readonly ResettableLazy<Transform> _pixelTransform;
        private readonly ResettableLazy<Transform> _rotatableTransform;
        private Color _color = Color.White;
        private float _rotation;
        private bool _snapToPixels;
        private Sprite? _sprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderComponent" /> class.
        /// </summary>
        public SpriteRenderComponent() {
            this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
            this._pixelTransform = new ResettableLazy<Transform>(this.CreatePixelTransform);
            this._rotatableTransform = new ResettableLazy<Transform>(this.CreateRotatableTransform);
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea {
            get {
                return this._boundingArea.Value;
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember(Order = 1)]
        public Color Color {
            get {
                return this._color;
            }

            set {
                this.Set(ref this._color, value);
            }
        }

        /// <summary>
        /// Gets or sets the render settings.
        /// </summary>
        /// <value>The render settings.</value>
        [DataMember(Order = 4, Name = "Render Settings")]
        public RenderSettings RenderSettings { get; private set; } = new RenderSettings();

        /// <inheritdoc />
        [DataMember(Order = 3)]
        public float Rotation {
            get {
                return this._snapToPixels ? 0f : this._rotation;
            }

            set {
                if (this.Set(ref this._rotation, value.NormalizeAngle())) {
                    if (!this._snapToPixels) {
                        this._boundingArea.Reset();
                        this._rotatableTransform.Reset();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this sprite renderer should snap to the pixel
        /// ratio defined in <see cref="IGameSettings" />.
        /// </summary>
        /// <remarks>Snapping to pixels will disable rotations on this renderer.</remarks>
        /// <value><c>true</c> if this should snap to pixels; otherwise, <c>false</c>.</value>
        [DataMember(Order = 2, Name = "Snap to Pixels")]
        public bool SnapToPixels {
            get {
                return this._snapToPixels;
            }

            set {
                if (this.Set(ref this._snapToPixels, value)) {
                    if (!this._snapToPixels) {
                        this._rotatableTransform.Reset();
                    }
                    else {
                        this._pixelTransform.Reset();
                    }

                    this._boundingArea.Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        [DataMember(Order = 0)]
        public Sprite? Sprite {
            get {
                return this._sprite;
            }
            set {
                if (this.Set(ref this._sprite, value)) {
                    this.Sprite?.Load();
                    this._boundingArea.Reset();
                    this.RenderSettings.InvalidateSize();
                }
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<Guid> GetOwnedAssetIds() {
            return this.Sprite != null ? new[] { this.Sprite.Id } : new Guid[0];
        }

        /// <inheritdoc />
        public virtual bool HasAsset(Guid id) {
            return this._sprite?.Id == id;
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.Sprite?.Load();
            this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
            this.RenderSettings.Initialize(this.CreateSize);
        }

        /// <inheritdoc />
        public virtual void RefreshAsset(Sprite newInstance) {
            if (newInstance != null && this.Sprite?.Id == newInstance.Id) {
                this.Sprite = newInstance;
            }
        }

        /// <inheritdoc />
        public virtual bool RemoveAsset(Guid id) {
            var result = this.HasAsset(id);
            if (result) {
                this.Sprite = null;
            }

            return result;
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Sprite != null) {
                if (this._snapToPixels) {
                    this.Entity.Scene.Game.SpriteBatch?.Draw(this.Sprite, this._pixelTransform.Value, this.Color, this.RenderSettings.Orientation);
                }
                else {
                    this.Entity.Scene.Game.SpriteBatch?.Draw(this.Sprite, this._rotatableTransform.Value, this.Color, this.RenderSettings.Orientation);
                }
            }
        }

        /// <inheritdoc />
        public virtual bool TryGetAsset(Guid id, out Sprite? asset) {
            var result = this.Sprite?.Id == id;
            asset = result ? this.Sprite : null;
            return result;
        }

        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameEntity.Transform)) {
                this.RenderSettings.InvalidateSize();
                this._boundingArea.Reset();
                this._pixelTransform.Reset();
                this._rotatableTransform.Reset();
            }
            else if (e.PropertyName == nameof(IGameEntity.IsEnabled)) {
                this.RaisePropertyChanged(nameof(this.IsVisible));
            }
        }

        private BoundingArea CreateBoundingArea() {
            BoundingArea result;
            if (this.Sprite != null) {
                var width = this.Sprite.Size.X * GameSettings.Instance.InversePixelsPerUnit;
                var height = this.Sprite.Size.Y * GameSettings.Instance.InversePixelsPerUnit;
                var offset = this.RenderSettings.Offset * GameSettings.Instance.InversePixelsPerUnit;

                var points = new List<Vector2> {
                    this.Entity.GetWorldTransform(offset, this.Rotation).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(width, 0f), this.Rotation).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(width, height), this.Rotation).Position,
                    this.Entity.GetWorldTransform(offset + new Vector2(0f, height), this.Rotation).Position
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

                result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
            }
            else {
                result = new BoundingArea();
            }

            return result;
        }

        private Transform CreatePixelTransform() {
            var worldTransform = this.Entity.GetWorldTransform(this.RenderSettings.Offset * GameSettings.Instance.InversePixelsPerUnit).ToPixelSnappedValue();
            return worldTransform;
        }

        private Transform CreateRotatableTransform() {
            return this.Entity.GetWorldTransform(this.RenderSettings.Offset * GameSettings.Instance.InversePixelsPerUnit, this.Rotation);
        }

        private Vector2 CreateSize() {
            var result = Vector2.Zero;
            if (this.Sprite != null) {
                return new Vector2(this.Sprite.Size.X, this.Sprite.Size.Y);
            }

            return result;
        }

        private void RenderSettings_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.RenderSettings.Offset)) {
                this._pixelTransform.Reset();
                this._rotatableTransform.Reset();
                this._boundingArea.Reset();
            }
        }
    }
}