namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework.Input;

/// <summary>
/// A set of icons corresponding to game pad <see cref="Buttons" />.
/// </summary>
public sealed class GamePadIconSet : BaseIconSet<Buttons> {
    /// <summary>
    /// The default name.
    /// </summary>
    public const string DefaultName = "Game Pad Icons";

    /// <summary>
    /// Initializes a new instance of the <see cref="GamePadIconSet" /> class.
    /// </summary>
    public GamePadIconSet() : base() {
        this.Name = DefaultName;
    }
}