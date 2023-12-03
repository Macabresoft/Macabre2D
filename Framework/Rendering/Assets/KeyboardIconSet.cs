namespace Macabresoft.Macabre2D.Framework;

using Microsoft.Xna.Framework.Input;

/// <summary>
/// A set of icons corresponding to keyboard <see cref="Keys" />.
/// </summary>
public sealed class KeyboardIconSet : BaseIconSet<Keys> {
    /// <summary>
    /// The default name.
    /// </summary>
    public const string DefaultName = "Keyboard Icons";

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardIconSet" /> class.
    /// </summary>
    public KeyboardIconSet() : base() {
        this.Name = DefaultName;
    }
}