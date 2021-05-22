namespace Macabresoft.Macabre2D.Framework {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for an object which has a <see cref="Transform" />.
    /// </summary>
    public interface ITransformable {

        /// <summary>
        /// Gets or sets the local position.
        /// </summary>
        /// <value>The local position.</value>
        Vector2 LocalPosition { get; set; }

        /// <summary>
        /// Gets or sets the local scale.
        /// </summary>
        /// <value>The local scale.</value>
        Vector2 LocalScale { get; set; }

        /// <summary>
        /// Gets the transform.
        /// </summary>
        /// <value>The transform.</value>
        Transform Transform { get; }

        /// <summary>
        /// Gets the transform matrix.
        /// </summary>
        /// <value>The transform matrix.</value>
        Matrix TransformMatrix { get; }

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(float rotationAngle);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="originOffset">The origin offset.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(Vector2 originOffset);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="originOffset">The origin offset.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(Vector2 originOffset, float rotationAngle);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="originOffset">The origin offset.</param>
        /// <param name="overrideScale">An override value for scale.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale, float rotationAngle);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <param name="originOffset">The origin offset.</param>
        /// <param name="overrideScale">An override value for scale.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <remarks>This is to be used in conjunction with a <see cref="TileGrid" />.</remarks>
        /// <param name="grid">The grid.</param>
        /// <param name="gridTileLocation">
        /// The grid tile location. This is the (x, y) coordinate on the grid for which one is
        /// getting the world transform.
        /// </param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(TileGrid grid, Point gridTileLocation);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <remarks>This is to be used in conjunction with a <see cref="TileGrid" />.</remarks>
        /// <param name="grid">The grid.</param>
        /// <param name="gridTileLocation">
        /// The grid tile location. This is the (x, y) coordinate on the grid for which one is
        /// getting the world transform.
        /// </param>
        /// <param name="offset">The offset.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset);

        /// <summary>
        /// Gets the world transform.
        /// </summary>
        /// <remarks>This is to be used in conjunction with a <see cref="TileGrid" />.</remarks>
        /// <param name="grid">The grid.</param>
        /// <param name="gridTileLocation">
        /// The grid tile location. This is the (x, y) coordinate on the grid for which one is
        /// getting the world transform.
        /// </param>
        /// <param name="offset">The offset.</param>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <returns>The world transform.</returns>
        Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset, float rotationAngle);

        /// <summary>
        /// Sets the world position.
        /// </summary>
        /// <param name="position">The position.</param>
        void SetWorldPosition(Vector2 position);

        /// <summary>
        /// Sets the world scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        void SetWorldScale(Vector2 scale);

        /// <summary>
        /// Sets the world transform.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        void SetWorldTransform(Vector2 position, Vector2 scale);
    }

    /// <summary>
    /// A default implementation of <see cref="ITransformable" /> that handles things in the
    /// expected way.
    /// </summary>
    [DataContract]
    public abstract class Transformable : NotifyPropertyChanged, ITransformable {
        private readonly ResettableLazy<Matrix> _transformMatrix;
        private bool _isTransformUpToDate;
        private Vector2 _localPosition;
        private Vector2 _localScale = Vector2.One;
        private Transform _transform = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Transformable" /> class.
        /// </summary>
        protected Transformable() {
            this._transformMatrix = new ResettableLazy<Matrix>(this.GetMatrix);
        }

        /// <summary>
        /// Gets or sets the local position.
        /// </summary>
        /// <value>The local position.</value>
        [DataMember(Name = "Local Position")]
        public Vector2 LocalPosition {
            get {
                return this._localPosition;
            }
            set {
                if (this.Set(ref this._localPosition, value)) {
                    this.HandleMatrixOrTransformChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the local scale.
        /// </summary>
        /// <value>The local scale.</value>
        [DataMember(Name = "Local Scale")]
        public Vector2 LocalScale {
            get {
                return this._localScale;
            }
            set {
                if (this.Set(ref this._localScale, value)) {
                    this.HandleMatrixOrTransformChanged();
                }
            }
        }

        /// <inheritdoc />
        public Transform Transform {
            get {
                if (!this._isTransformUpToDate) {
                    this._transform = this.TransformMatrix.DecomposeWithoutRotation2D();
                    this._isTransformUpToDate = true;
                }

                return this._transform;
            }
        }

        /// <inheritdoc />
        public Matrix TransformMatrix {
            get {
                return this._transformMatrix.Value;
            }
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(float rotationAngle) {
            var transform = this.Transform;
            var matrix =
                Matrix.CreateScale(transform.Scale.X, transform.Scale.Y, 1f) *
                Matrix.CreateRotationZ(rotationAngle) *
                Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);

            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(Vector2 originOffset) {
            var matrix = Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) * this.TransformMatrix;
            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(Vector2 originOffset, float rotationAngle) {
            var transform = this.Transform;

            var matrix =
                Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) *
                Matrix.CreateScale(transform.Scale.X, transform.Scale.Y, 1f) *
                Matrix.CreateRotationZ(rotationAngle) *
                Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);

            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale, float rotationAngle) {
            var transform = this.Transform;

            var matrix =
                Matrix.CreateScale(overrideScale.X, overrideScale.Y, 1f) *
                Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) *
                Matrix.CreateRotationZ(rotationAngle) *
                Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);

            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale) {
            var transform = this.Transform;

            var matrix =
                Matrix.CreateScale(overrideScale.X, overrideScale.Y, 1f) *
                Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) *
                Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);

            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation) {
            var position = new Vector2(gridTileLocation.X * grid.TileSize.X, gridTileLocation.Y * grid.TileSize.Y) + grid.Offset;
            return this.GetWorldTransform(position);
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset) {
            var position = new Vector2(gridTileLocation.X * grid.TileSize.X, gridTileLocation.Y * grid.TileSize.Y) + grid.Offset + offset;
            return this.GetWorldTransform(position);
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset, float rotationAngle) {
            var position = new Vector2(gridTileLocation.X * grid.TileSize.X, gridTileLocation.Y * grid.TileSize.Y) + grid.Offset + offset;
            return this.GetWorldTransform(position, rotationAngle);
        }

        /// <inheritdoc />
        public void SetWorldPosition(Vector2 position) {
            this.SetWorldTransform(position, this.Transform.Scale);
        }

        /// <inheritdoc />
        public void SetWorldScale(Vector2 scale) {
            this.SetWorldTransform(this.Transform.Position, scale);
        }

        /// <inheritdoc />
        public void SetWorldTransform(Vector2 position, Vector2 scale) {
            var matrix =
                Matrix.CreateScale(scale.X, scale.Y, 1f) *
                Matrix.CreateTranslation(position.X, position.Y, 0f);

            var parent = this.GetParentTransformable();
            if (parent != this) {
                matrix *= Matrix.Invert(parent.TransformMatrix);
            }

            var localTransform = matrix.ToTransform();
            this._localPosition = localTransform.Position;
            this._localScale = localTransform.Scale;
            this.HandleMatrixOrTransformChanged();
            this.RaisePropertyChanged(nameof(this.LocalPosition));
            this.RaisePropertyChanged(nameof(this.LocalScale));
        }

        /// <summary>
        /// Gets the parent transformable.
        /// </summary>
        /// <returns>The parent transformable.</returns>
        protected abstract ITransformable GetParentTransformable();

        /// <summary>
        /// Handles the matrix or transform changed.
        /// </summary>
        protected void HandleMatrixOrTransformChanged() {
            this._transformMatrix.Reset();
            this._isTransformUpToDate = false;
            this.RaisePropertyChanged(true, nameof(this.Transform));
        }

        private Matrix GetMatrix() {
            var transformMatrix =
                Matrix.CreateScale(this.LocalScale.X, this.LocalScale.Y, 1f) *
                Matrix.CreateTranslation(this.LocalPosition.X, this.LocalPosition.Y, 0f);

            var parent = this.GetParentTransformable();
            if (parent != this) {
                transformMatrix *= parent.TransformMatrix;
            }

            return transformMatrix;
        }

        internal class EmptyTransformable : ITransformable {

            /// <inheritdoc />
            public Vector2 LocalPosition {
                get => Vector2.Zero;
                set { return; }
            }

            /// <inheritdoc />
            public Vector2 LocalScale {
                get => Vector2.One;
                set { return; }
            }

            /// <inheritdoc />
            public Transform Transform => Transform.Origin;

            /// <inheritdoc />
            public Matrix TransformMatrix => Matrix.Identity;

            /// <inheritdoc />
            public Transform GetWorldTransform(float rotationAngle) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(Vector2 originOffset) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(Vector2 originOffset, float rotationAngle) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale, float rotationAngle) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset, float rotationAngle) {
                return this.Transform;
            }

            /// <inheritdoc />
            public void SetWorldPosition(Vector2 position) {
                return;
            }

            /// <inheritdoc />
            public void SetWorldScale(Vector2 scale) {
                return;
            }

            /// <inheritdoc />
            public void SetWorldTransform(Vector2 position, Vector2 scale) {
                return;
            }
        }
    }
}