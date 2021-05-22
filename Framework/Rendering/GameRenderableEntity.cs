namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for an entity which can be rendered.
    /// </summary>
    public interface IGameRenderableEntity : IBoundable, IGameEntity {
        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        bool IsVisible { get; }

        /// <summary>
        /// Gets the render order.
        /// </summary>
        /// <value>The render order.</value>
        int RenderOrder => 0;

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="viewBoundingArea">The view bounding area.</param>
        void Render(FrameTime frameTime, BoundingArea viewBoundingArea);
    }

    /// <summary>
    /// A <see cref="IGameEntity" /> which has a default implementation of
    /// <see cref="IGameRenderableEntity" />.
    /// </summary>
    public abstract class GameRenderableEntity : GameEntity, IGameRenderableEntity {
        private bool _isVisible = true;
        private int _renderOrder;

        /// <inheritdoc />
        public abstract BoundingArea BoundingArea { get; }

        /// <inheritdoc />
        [DataMember]
        public bool IsVisible {
            get => this._isVisible && this.IsEnabled;
            set => this.Set(ref this._isVisible, value, this.IsEnabled);
        }

        /// <inheritdoc />
        public int RenderOrder {
            get => this._renderOrder;

            set => this.Set(ref this._renderOrder, value);
        }

        /// <inheritdoc />
        public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

        /// <inheritdoc />
        protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IEnableable.IsEnabled) && this._isVisible) {
                this.RaisePropertyChanged(nameof(this.IsVisible));
            }
        }
    }
}