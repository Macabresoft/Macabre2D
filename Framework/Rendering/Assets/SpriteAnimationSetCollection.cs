namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

/// <summary>
/// An observable collection of <see cref="SpriteAnimationSet" />.
/// </summary>
[DataContract]
[Category("Animation Sets")]
public class SpriteAnimationSetCollection : SpriteSheetMemberCollection<SpriteAnimationSet> {
    /// <inheritdoc />
    public override string Name => "Animation Sets";

    /// <inheritdoc />
    public override bool TryCreateNewMember([NotNullWhen(true)] out SpriteSheetMember? member) {
        member = null;
        return false;
    }
}