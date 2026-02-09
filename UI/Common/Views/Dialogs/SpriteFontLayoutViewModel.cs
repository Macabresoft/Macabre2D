namespace Macabre2D.UI.Common;

using Macabresoft.AvaloniaEx;
using Unity;

/// <summary>
/// View model for editing a sprite font layout.
/// </summary>
public class SpriteFontLayoutViewModel : BaseDialogViewModel {
    /// <summary>
    /// Initializes a new instance of <see cref="SpriteFontLayoutViewModel" />.
    /// </summary>
    public SpriteFontLayoutViewModel() : base() {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SpriteFontLayoutViewModel" />.
    /// </summary>
    /// <param name="characterLayout">The character layout.</param>
    [InjectionConstructor]
    public SpriteFontLayoutViewModel(string characterLayout) : this() {
        this.Result.CharacterLayout = characterLayout;
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    public SpriteFontLayoutResult Result { get; } = new();
}