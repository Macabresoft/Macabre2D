namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Core;

/// <summary>
/// Result of a font auto layout.
/// </summary>
public class SpriteFontLayoutResult : PropertyChangedNotifier {
    private bool _automaticallyApplyKerning;
    private string _characterLayout = string.Empty;
    private bool _performAutoLayout;

    /// <summary>
    /// Gets or sets a value indicating whether to automatically apply kerning to each character based on its whitespace.
    /// </summary>
    public bool AutomaticallyApplyKerning {
        get => this._automaticallyApplyKerning;
        set => this.Set(ref this._automaticallyApplyKerning, value);
    }

    /// <summary>
    /// Gets or sets the character layout.
    /// </summary>
    public string CharacterLayout {
        get => this._characterLayout;
        set => this.Set(ref this._characterLayout, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to perform an auto layout.
    /// </summary>
    public bool PerformAutoLayout {
        get => this._performAutoLayout;
        set => this.Set(ref this._performAutoLayout, value);
    }
}