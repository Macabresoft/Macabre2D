namespace Macabresoft.MonoGame.Core {

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
        int RenderOrder { get; }

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

        /// <inheritdoc />
        public abstract BoundingArea BoundingArea { get; }

        /// <inheritdoc />
        public bool IsVisible {
            get {
                return this._isVisible && this.Entity.IsEnabled;
            }
        }

        /// <inheritdoc />
        public int RenderOrder => throw new System.NotImplementedException();

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            entity.PropertyChanged += this.Entity_PropertyChanged;
        }

        /// <inheritdoc />
        public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

        private void Entity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            throw new System.NotImplementedException();
        }
    }
}