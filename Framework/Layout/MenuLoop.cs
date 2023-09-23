namespace Macabresoft.Macabre2D.Framework.Layout;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// A loop that handles navigating menus.
/// </summary>
public class MenuLoop : Loop {
    /// <summary>
    /// Gets a reference the the initial menu item that should be selected.
    /// </summary>
    [DataMember]
    public EntityReference<IMenuItem> InitialMenuItem { get; } = new();

    /// <inheritdoc />
    public override LoopKind Kind => LoopKind.PreUpdate;

    /// <summary>
    /// Gets the current menu item.
    /// </summary>
    public IMenuItem? CurrentMenuItem { get; private set; }

    /// <summary>
    /// Gets the <see cref="InputAction" /> for down.
    /// </summary>
    [DataMember]
    public InputAction DownInput { get; set; }

    /// <summary>
    /// Gets the <see cref="InputAction" /> for left.
    /// </summary>
    [DataMember]
    public InputAction LeftInput { get; set; }

    /// <summary>
    /// Gets the <see cref="InputAction" /> for right.
    /// </summary>
    [DataMember]
    public InputAction RightInput { get; set; }

    /// <summary>
    /// Gets the <see cref="InputAction" /> for up.
    /// </summary>
    [DataMember]
    public InputAction UpInput { get; set; }

    /// <inheritdoc />
    public override void Initialize(IScene scene) {
        base.Initialize(scene);

        this.InitialMenuItem.Initialize(this.Scene);
        this.CurrentMenuItem = this.InitialMenuItem.Entity;
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.CurrentMenuItem is { } menuItem) {
        }
        else if (this.InitialMenuItem.Entity != null) {
            // TODO: if any input is pressed, set CurrentMenuItem to InitialMenuItem.Entity
            this.CurrentMenuItem = this.InitialMenuItem.Entity;
        }
    }
}