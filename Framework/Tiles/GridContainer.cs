namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Macabresoft.Core;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An interface for an entity which contains a grid.
    /// </summary>
    public interface IGridContainer : IEntity {
        /// <summary>
        /// Gets the tile size in world units.
        /// </summary>
        Vector2 WorldTileSize { get; }

        /// <summary>
        /// Gets or sets the tile size in local space, unaffected by the scale of this <see cref="ITransformable" />.
        /// </summary>
        Vector2 LocalTileSize { get; set; }

        /// <summary>
        /// Gets the nearest tile position to the provided position.
        /// </summary>
        /// <param name="position">The position of which to find the nearest tile.</param>
        /// <returns>The nearest tile position in world space.</returns>
        Vector2 GetNearestTilePosition(Vector2 position);

        /// <summary>
        /// Gets the tile position.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The tile position.</returns>
        public Vector2 GetTilePosition(Point tile);
    }

    /// <summary>
    /// An entity which contains a grid.
    /// </summary>
    public class GridContainer : Entity, IGridContainer {
        /// <summary>
        /// An empty grid container. Defaults to a 1 by 1 grid.
        /// </summary>
        public static readonly IGridContainer EmptyGridContainer = new EmptyGridContainer();

        private readonly ResettableLazy<Vector2> _worldTileSize;
        private Vector2 _tileSize = Vector2.One;

        public GridContainer() : base() {
            this._worldTileSize = new ResettableLazy<Vector2>(this.GetWorldTileSize);
        }

        /// <inheritdoc />
        public Vector2 WorldTileSize => this._worldTileSize.Value;

        /// <inheritdoc />
        [DataMember(Name = "Tile Size")]
        [Category("Grid")]
        public Vector2 LocalTileSize {
            get => this._tileSize;
            set {
                if (this.Set(ref this._tileSize, value)) {
                    this.ResetWorldTileSize();
                }
            }
        }

        /// <inheritdoc />
        public Vector2 GetNearestTilePosition(Vector2 position) {
            var transform = this.Transform;
            var x = position.X - transform.Position.X;
            var y = position.Y - transform.Position.Y;

            if (this.WorldTileSize.X > 0f) {
                x = (float)Math.Round(x / this.WorldTileSize.X) * this.WorldTileSize.X;
            }

            if (this.WorldTileSize.Y > 0f) {
                y = (float)Math.Round(y / this.WorldTileSize.Y) * this.WorldTileSize.Y;
            }

            return new Vector2(x, y) + transform.Position;
        }


        /// <summary>
        /// Gets the tile position.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The tile position.</returns>
        public Vector2 GetTilePosition(Point tile) {
            return new Vector2(tile.X * this.WorldTileSize.X, tile.Y * this.WorldTileSize.Y) + this.Transform.Position;
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            base.OnPropertyChanged(sender, e);

            if (e.PropertyName == nameof(ITransformable.Transform)) {
                this.ResetWorldTileSize();
            }
        }

        private Vector2 GetWorldTileSize() {
            return this._tileSize * this.Transform.Scale;
        }

        private void ResetWorldTileSize() {
            this._worldTileSize.Reset();
            this.RaisePropertyChanged(nameof(this.WorldTileSize));
        }
    }

    internal class EmptyGridContainer : Entity.EmptyEntity, IGridContainer {
        public Vector2 WorldTileSize => this.LocalTileSize;

        public Vector2 LocalTileSize {
            get => Vector2.One;
            set { }
        }

        public Vector2 GetNearestTilePosition(Vector2 position) {
            throw new NotImplementedException();
        }

        public Vector2 GetTilePosition(Point tile) {
            throw new NotImplementedException();
        }
    }
}