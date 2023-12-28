namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// Various fallback values that entities use.
/// </summary>
[DataContract]
[Category(CommonCategories.Fallback)]
public class ProjectFallbacks {
    /// <summary>
    /// Gets the <see cref="GamePadIconSetReference" /> for "Game Pad N".
    /// </summary>
    [DataMember]
    public GamePadIconSetReference GamePadNReference { get; } = new();

    /// <summary>
    /// Gets the <see cref="GamePadIconSetReference" /> for "Game Pad S".
    /// </summary>
    [DataMember]
    public GamePadIconSetReference GamePadSReference { get; } = new();

    /// <summary>
    /// Gets the <see cref="GamePadIconSetReference" /> for "Game Pad X".
    /// </summary>
    [DataMember]
    public GamePadIconSetReference GamePadXReference { get; } = new();

    /// <summary>
    /// Gets the icon set reference for keyboards.
    /// </summary>
    [DataMember]
    public KeyboardIconSetReference KeyboardReference { get; } = new();

    /// <summary>
    /// Gets or sets the color of the game background when there is no scene opened.
    /// </summary>
    [DataMember]
    public Color BackgroundColor { get; set; } = Color.Black;

    /// <summary>
    /// Gets or sets the color that sprites will be filled in with if their content cannot be loaded.
    /// </summary>
    [DataMember]
    public Color ErrorSpritesColor { get; set; } = Color.HotPink;
}