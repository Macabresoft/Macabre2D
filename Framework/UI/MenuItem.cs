namespace Macabresoft.Macabre2D.Framework;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

/// <summary>
/// Interface for a menu item.
/// </summary>
public interface IMenuItem : IEntity {
    /// <summary>
    /// Gets or sets a value indicating whether or not this instance is focused.
    /// </summary>
    bool IsFocused { get; set; }

    /// <summary>
    /// Handles input.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="inputState">The input state.</param>
    void HandleInput(FrameTime frameTime, InputState inputState);

    bool TryGetNextMenuItem(CardinalDirections direction, [NotNullWhen(true)] out IMenuItem? menuItem);
}

/// <summary>
/// A menu item.
/// </summary>
public abstract class MenuItem : DockableWrapper, IMenuItem {
    /// <summary>
    /// Gets a reference to the menu item below this instance.
    /// </summary>
    [DataMember]
    public EntityReference<MenuItem> MenuItemDownReference { get; } = new();

    /// <summary>
    /// Gets a reference to the menu item to the left of this instance.
    /// </summary>
    [DataMember]
    public EntityReference<MenuItem> MenuItemLeftReference { get; } = new();

    /// <summary>
    /// Gets a reference to the menu item to the right of this instance.
    /// </summary>
    [DataMember]
    public EntityReference<MenuItem> MenuItemRightReference { get; } = new();

    /// <summary>
    /// Gets a reference to the menu item above this instance.
    /// </summary>
    [DataMember]
    public EntityReference<MenuItem> MenuItemUpReference { get; } = new();

    /// <inheritdoc />
    public bool IsFocused { get; set; }

    /// <inheritdoc />
    public abstract void HandleInput(FrameTime frameTime, InputState inputState);

    /// <inheritdoc />
    public bool TryGetNextMenuItem(CardinalDirections direction, [NotNullWhen(true)] out IMenuItem? menuItem) {
        menuItem = direction switch {
            CardinalDirections.North => this.MenuItemUpReference.Entity,
            CardinalDirections.West => this.MenuItemLeftReference.Entity,
            CardinalDirections.East => this.MenuItemRightReference.Entity,
            CardinalDirections.South => this.MenuItemDownReference.Entity,
            _ => null
        };

        return menuItem != null;
    }
}