namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an object which has a <see cref="Transform" />.
/// </summary>
public interface ITransformable {
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
[Category(CommonCategories.Transform)]
public abstract class Transformable : NotifyPropertyChanged, ITransformable {
    private readonly ResettableLazy<Matrix> _transformMatrix;
    private bool _isTransformUpToDate;
    private Vector2 _localPosition;
    private Vector2 _localScale = Vector2.One;
    private Transform _transform;

    /// <summary>
    /// Initializes a new instance of the <see cref="Transformable" /> class.
    /// </summary>
    protected Transformable() {
        this._transformMatrix = new ResettableLazy<Matrix>(this.GetMatrix);
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
    public Matrix TransformMatrix => this._transformMatrix.Value;

    /// <summary>
    /// Gets or sets the local position.
    /// </summary>
    /// <value>The local position.</value>
    [DataMember(Name = "Local Position")]
    public Vector2 LocalPosition {
        get => this._localPosition;
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
        get => this._localScale;
        set {
            if (this.Set(ref this._localScale, value)) {
                this.HandleMatrixOrTransformChanged();
            }
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
        public Transform Transform => Transform.Origin;

        /// <inheritdoc />
        public Matrix TransformMatrix => Matrix.Identity;

        /// <inheritdoc />
        public Vector2 LocalPosition {
            get => Vector2.Zero;
            set { }
        }

        /// <inheritdoc />
        public Vector2 LocalScale {
            get => Vector2.One;
            set { }
        }

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
        public void SetWorldPosition(Vector2 position) {
        }

        /// <inheritdoc />
        public void SetWorldScale(Vector2 scale) {
        }

        /// <inheritdoc />
        public void SetWorldTransform(Vector2 position, Vector2 scale) {
        }
    }
}