namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// An updateable entity that is updated on a fixed time step.
/// </summary>
public interface IFixedUpdateableEntity : INotifyPropertyChanged, IEnableable {
    /// <summary>
    /// Gets the update order.
    /// </summary>
    /// <value>The update order.</value>
    int UpdateOrder => 0;

    /// <summary>
    /// Performs a fixed update.
    /// </summary>
    /// <remarks>
    /// Seconds passed should always be equal to the time step of the fixed update loop calling update.
    /// </remarks>
    /// <param name="timeStep">The time step.</param>
    void FixedUpdate(float timeStep);
}

/// <summary>
/// Base implementation of <see cref="IFixedUpdateableEntity" />.
/// </summary>
public abstract class FixedUpdateableEntity : Entity, IFixedUpdateableEntity {
    private int _updateOrder;

    /// <inheritdoc />
    [DataMember]
    public int UpdateOrder {
        get => this._updateOrder;
        set => this.Set(ref this._updateOrder, value);
    }

    /// <inheritdoc />
    public abstract void FixedUpdate(float timeStep);
}