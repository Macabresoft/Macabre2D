namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an object which can be transformed.
/// </summary>
public interface ITransformable {
    /// <summary>
    /// Occurs when the transform changes.
    /// </summary>
    event EventHandler TransformChanged;

    /// <summary>
    /// Gets the world position position.
    /// </summary>
    Vector2 WorldPosition { get; }

    /// <summary>
    /// Gets or sets the local position.
    /// </summary>
    /// <value>The local position.</value>
    Vector2 LocalPosition { get; set; }

    /// <summary>
    /// Gets the world position with an offset.
    /// </summary>
    /// <param name="originOffset">The origin offset.</param>
    /// <returns>The world position.</returns>
    Vector2 GetWorldPosition(Vector2 originOffset);

    /// <summary>
    /// Moves this actor the specified amount.
    /// </summary>
    /// <param name="amount">The amount.</param>
    void Move(Vector2 amount);

    /// <summary>
    /// Sets the world position.
    /// </summary>
    /// <param name="position">The position.</param>
    void SetWorldPosition(Vector2 position);
}

/// <summary>
/// A default implementation of <see cref="ITransformable" /> that handles things in the
/// expected way.
/// </summary>
[DataContract]
[Category(CommonCategories.Transform)]
public abstract class Transformable : PropertyChangedNotifier, ITransformable {
    private readonly ResettableLazy<Vector2> _worldPosition;
    private Vector2 _localPosition;

    /// <inheritdoc />
    public event EventHandler? TransformChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="Transformable" /> class.
    /// </summary>
    protected Transformable() {
        this._worldPosition = new ResettableLazy<Vector2>(this.GetWorldPosition);
    }

    /// <summary>
    /// Gets the world position position.
    /// </summary>
    public Vector2 WorldPosition => this._worldPosition.Value;

    /// <summary>
    /// Gets or sets the local position.
    /// </summary>
    [DataMember(Name = "Local Position")]
    public Vector2 LocalPosition {
        get => this._localPosition;
        set {
            if (this._localPosition != value) {
                this._localPosition = value;
                this.HandleTransformed();

                if (BaseGame.IsDesignMode) {
                    this.RaisePropertyChanged();
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the way this inherits its parent's position.
    /// </summary>
    [DataMember(Name = "Inheritance")]
    public TransformInheritance TransformInheritance { get; set; } = TransformInheritance.Both;

    /// <inheritdoc />
    public Vector2 GetWorldPosition(Vector2 originOffset) {
        return this.WorldPosition + originOffset;
    }

    /// <inheritdoc />
    public void Move(Vector2 amount) {
        this.SetWorldPosition(this.WorldPosition + amount);
    }

    /// <inheritdoc />
    public void SetWorldPosition(Vector2 position) {
        this.LocalPosition = this.TransformInheritance switch {
            TransformInheritance.None => position,
            TransformInheritance.X => new Vector2(this.LocalPosition.X + position.X - this.WorldPosition.X, this.LocalPosition.Y),
            TransformInheritance.Y => new Vector2(this.LocalPosition.X, this.LocalPosition.Y + position.Y - this.WorldPosition.Y),
            TransformInheritance.Both => this.LocalPosition + position - this.WorldPosition,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Gets the parent transformable.
    /// </summary>
    /// <returns>The parent transformable.</returns>
    protected abstract ITransformable GetParentTransformable();

    /// <summary>
    /// Handles the matrix or transform changed.
    /// </summary>
    protected void HandleTransformed() {
        this._worldPosition.Reset();
        this.OnTransformChanged();
    }

    /// <summary>
    /// Called when the transform changes.
    /// </summary>
    protected virtual void OnTransformChanged() {
        this.TransformChanged.SafeInvoke(this);
    }

    private Vector2 GetParentWorldPosition() {
        if (this.GetParentTransformable() is { } transformable && transformable != this) {
            return transformable.WorldPosition;
        }

        return Vector2.Zero;
    }

    private Vector2 GetWorldPosition() {
        return this.TransformInheritance switch {
            TransformInheritance.None => this.LocalPosition,
            TransformInheritance.X => new Vector2(this.LocalPosition.X + this.GetParentWorldPosition().X, this.LocalPosition.Y),
            TransformInheritance.Y => new Vector2(this.LocalPosition.X, this.LocalPosition.Y + this.GetParentWorldPosition().Y),
            TransformInheritance.Both => this.GetParentWorldPosition() + this.LocalPosition,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}