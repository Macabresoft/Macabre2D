namespace Macabre2D.Framework;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

/// <summary>
/// An observable collection of <see cref="SpriteAnimation" />.
/// </summary>
[DataContract]
[Category("Animations")]
public class SpriteAnimationCollection : SpriteSheetMemberCollection<SpriteAnimation> {
    /// <inheritdoc />
    public override string Name => "Animations";

    /// <inheritdoc />
    public override bool TryCreateNewMember([NotNullWhen(true)] out SpriteSheetMember? member) {
        member = new SpriteAnimation();
        return true;
    }
}