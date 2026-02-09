namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// A base for <see cref="IDockable" /> implementations.
/// </summary>
public abstract class BaseDockable : Entity, IDockable {
    internal const string DockingCategoryName = "Docking (UI)";
    private DockLocation _location = DockLocation.Center;

    /// <inheritdoc />
    public abstract event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public abstract BoundingArea BoundingArea { get; }

    /// <inheritdoc />
    [DataMember]
    [Category(DockingCategoryName)]
    public DockLocation Location {
        get => this._location;
        set {
            if (value != this._location) {
                this._location = value;
                this.RequestRearrangeFromParent();
            }
        }
    }

    /// <summary>
    /// Requests a refresh from its parent.
    /// </summary>
    protected virtual void RequestRearrangeFromParent() {
        if (this.Parent is IDockingContainer container) {
            container.RequestRearrange(this);
        }
    }
}