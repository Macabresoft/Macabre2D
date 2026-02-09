namespace Macabre2D.Framework;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

/// <summary>
/// An observable collection of <see cref="SpriteSheetIconSet" />.
/// </summary>
[DataContract]
[Category("Icon Sets")]
public class SpriteSheetIconSetCollection : SpriteSheetMemberCollection<SpriteSheetIconSet> {
    /// <inheritdoc />
    public override string Name => "Icon Sets";

    /// <inheritdoc />
    public override bool TryCreateNewMember([NotNullWhen(true)] out SpriteSheetMember? member) {
        member = null;
        return false;
    }
}