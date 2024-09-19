namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// An updateable entity that is updated on a fixed time step.
/// </summary>
public interface IFixedUpdateableEntity : INotifyPropertyChanged, IEnableable {

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
    private int _updateOrder;

    [DataMember]
    public bool ShouldUpdate {
        get => this._shouldUpdate && this.IsEnabled;
        set => this.Set(ref this._shouldUpdate, value);
    }

    /// <inheritdoc />
    [DataMember]
    public int UpdateOrder {
        get => this._updateOrder;
        set => this.Set(ref this._updateOrder, value);
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