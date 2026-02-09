namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// An updateable entity that is updated on a fixed time step.
/// </summary>
public interface IFixedUpdateableEntity : INotifyPropertyChanged, IEnableable {

    /// <summary>
    /// Called when <see cref="ShouldUpdate" /> changes.
    /// </summary>
    event EventHandler? ShouldUpdateChanged;

    /// <summary>
    /// Called when <see cref="UpdateOrder" /> changes.
    /// </summary>
    event EventHandler? UpdateOrderChanged;

    /// <summary>
    /// Gets or sets a value indicating whether this instance should update.
    /// </summary>
    bool ShouldUpdate { get; }

    /// <summary>
    /// Gets the update order.
    /// </summary>
    /// <value>The update order.</value>
    int UpdateOrder => 0;

    /// <summary>
    /// Performs a fixed update.
    /// </summary>
    /// <remarks>
    /// Seconds passed should always be equal to the time step of the fixed update system calling update.
    /// </remarks>
    /// <param name="timeStep">The time step.</param>
    void FixedUpdate(float timeStep);
}

/// <summary>
/// Base implementation of <see cref="IFixedUpdateableEntity" />.
/// </summary>
public abstract class FixedUpdateableEntity : Entity, IFixedUpdateableEntity {
    private bool _shouldUpdate;

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <inheritdoc />
    public event EventHandler? UpdateOrderChanged;

    /// <inheritdoc />
    [DataMember]
    public bool ShouldUpdate {
        get => this._shouldUpdate && this.IsEnabled;
        set {
            if (this.Set(ref this._shouldUpdate, value)) {
                this.ShouldUpdateChanged.SafeInvoke(this);
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    public int UpdateOrder {
        get;
        set {
            if (this.Set(ref field, value)) {
                this.UpdateOrderChanged.SafeInvoke(this);
            }
        }
    }

    /// <inheritdoc />
    public abstract void FixedUpdate(float timeStep);

    /// <inheritdoc />
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();

        if (this._shouldUpdate) {
            this.RaisePropertyChanged(nameof(this.ShouldUpdate));
        }
    }
}