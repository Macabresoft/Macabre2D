namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// A <see cref="IEntity" /> which implements <see cref="IUpdateableGameObject" />.
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
[Category("Updateable")]
public abstract class UpdateableEntity : Entity, IUpdateableEntity {
    /// <inheritdoc />
    [DataMember]
    public int UpdateOrder { get; set; }

    /// <inheritdoc />
    public abstract void Update(FrameTime frameTime, InputState inputState);
}