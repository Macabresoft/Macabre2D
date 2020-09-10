namespace Macabresoft.MonoGame.Core {

    using System.ComponentModel;

    /// <summary>
    /// Interface for a component which can be rendered.
    /// </summary>
    public interface IGameRenderableComponent : IBoundable, IGameComponent {

        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        bool IsVisible { get; }

        /// <summary>
        /// Gets the render order.
        /// </summary>
        /// <value>The render order.</value>
        int RenderOrder { get => 0; }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="viewBoundingArea">The view bounding area.</param>
        void Render(FrameTime frameTime, BoundingArea viewBoundingArea);
    }

    /// <summary>
    /// A <see cref="IGameComponent" /> which has a default implementation of <see
    /// cref="IGameRenderableComponent" />.
    /// </summary>
    public abstract class GameRenderableComponent : GameComponent, IGameRenderableComponent {
        private bool _isVisible = true;
        private int _renderOrder;

        /// <inheritdoc />
        public abstract BoundingArea BoundingArea { get; }

        /// <inheritdoc />
        public bool IsVisible {
            get {
                return this._isVisible && this.Entity.IsEnabled;
            }

            set {
                this.Set(ref this._isVisible, value, this.Entity.IsEnabled);
            }
        }

        /// <inheritdoc />
        public int RenderOrder {
            get {
                return this._renderOrder;
            }

            set {
                this.Set(ref this._renderOrder, value);
            }
        }

        /// <inheritdoc />
        public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

        protected override void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameEntity.IsEnabled) && this._isVisible) {
                this.RaisePropertyChanged(nameof(this.IsVisible));
            }
        }
    }
}