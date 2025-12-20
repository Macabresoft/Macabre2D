namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// A <see cref="IEntity" /> which implements <see cref="IUpdateableGameObject" />.
/// </summary>
public interface IUpdateableEntity : IEntity, IUpdateableGameObject {
    /// <summary>
    /// Called when <see cref="UpdateOrder" /> changes.
    /// </summary>
    event EventHandler? UpdateOrderChanged;

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

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <inheritdoc />
    public event EventHandler? UpdateOrderChanged;

    /// <inheritdoc />
    [DataMember]
    public bool ShouldUpdate {
        get => this._shouldUpdate && this.IsEnabled;
        set {
            if (this.Set(ref this._shouldUpdate, value) && this.IsInitialized) {
                this.ShouldUpdateChanged.SafeInvoke(this);
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    [PredefinedInteger(PredefinedIntegerKind.UpdateOrder)]
    public int UpdateOrder {
        get;
        set {
            if (this.Set(ref field, value) && this.IsInitialized) {
                this.UpdateOrderChanged.SafeInvoke(this);
            }
        }
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