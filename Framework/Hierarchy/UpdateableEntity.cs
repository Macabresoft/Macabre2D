namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// A <see cref="IEntity" /> which implements <see cref="IUpdateableGameObject" />,
/// <see
///     cref="IEnableable" />
/// , and can be sorted.
/// </summary>
public interface IUpdateableEntity : IEntity, IUpdateableGameObject {
    /// <summary>
    /// Gets the update order.
    /// </summary>
    /// <value>The update order.</value>
    int UpdateOrder => 0;
}

/// <summary>
/// A base implementation of <see cref="IUpdateableEntity" />.
/// </summary>
public abstract class UpdateableEntity : Entity, IUpdateableEntity {
    private int _updateOrder;

    /// <inheritdoc />
    [DataMember]
    public int UpdateOrder {
        get => this._updateOrder;

        set => this.Set(ref this._updateOrder, value);
    }

    /// <inheritdoc />
    public abstract void Update(FrameTime frameTime, InputState inputState);
}