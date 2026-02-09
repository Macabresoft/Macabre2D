namespace Macabre2D.Framework;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

/// <summary>
/// An observable collection of <see cref="SpriteSheetFont"/>.
/// </summary>
[DataContract]
[Category("Fonts")]
public class SpriteSheetFontCollection : SpriteSheetMemberCollection<SpriteSheetFont> {
    /// <inheritdoc />
    public override string Name => "Fonts";

    /// <inheritdoc />
    public override bool TryCreateNewMember([NotNullWhen(true)] out SpriteSheetMember? member) {
        member = new SpriteSheetFont();
        return true;
    }
}