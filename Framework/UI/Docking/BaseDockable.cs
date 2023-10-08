﻿namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A base for <see cref="IDockable" /> implementations.
/// </summary>
public abstract class BaseDockable : Entity, IDockable {
    private const string DockingCategoryName = "Docking (UI)";
    private DockLocation _location = DockLocation.Center;
    private Vector2 _margin = Vector2.Zero;

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
                this.RequestReset();
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    [Category(DockingCategoryName)]
    public Vector2 Margin {
        get => this._margin;
        set {
            this._margin = value;
            this.RequestReset();
        }
    }

    /// <summary>
    /// Requests a refresh from its parent.
    /// </summary>
    protected void RequestReset() {
        if (this.Parent is IDockingContainer container) {
            container.RequestRearrange(this);
        }
    }
}