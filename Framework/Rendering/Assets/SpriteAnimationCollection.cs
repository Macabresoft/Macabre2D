namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// An observable collection of <see cref="SpriteAnimation" />.
/// </summary>
[DataContract]
[Category("Animations")]
public class SpriteAnimationCollection : SpriteSheetMemberCollection<SpriteAnimation> {
    /// <inheritdoc />
    public override string Name => "Sprite Animations";
}