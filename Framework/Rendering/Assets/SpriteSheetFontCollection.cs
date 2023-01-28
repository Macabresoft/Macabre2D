namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// An observable collection of <see cref="SpriteSheetFont"/>.
/// </summary>
[DataContract]
[Category("Fonts")]
public class SpriteSheetFontCollection : NameableCollection<SpriteSheetFont> {
    /// <inheritdoc />
    public override string Name => "Fonts";
}