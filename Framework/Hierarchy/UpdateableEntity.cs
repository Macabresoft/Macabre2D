namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;

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
    private bool _shouldUpdate = true;
    private int _updateOrder;

    [DataMember]
    public bool ShouldUpdate {
        get => this._shouldUpdate && this.IsEnabled;
        set => this.Set(ref this._shouldUpdate, value);
    }

    /// <inheritdoc />
    [DataMember]
    [PredefinedInteger(PredefinedIntegerKind.UpdateOrder)]
    public int UpdateOrder {
        get => this._updateOrder;
        set => this.Set(ref this._updateOrder, value);
    }

    /// <inheritdoc />
    public abstract void Update(FrameTime frameTime, InputState inputState);

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEnableable.IsEnabled) && this._shouldUpdate) {
            this.RaisePropertyChanged(nameof(this.ShouldUpdate));
        }
    }
}