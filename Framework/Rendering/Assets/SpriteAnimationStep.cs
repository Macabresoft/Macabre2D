namespace Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A step in a sprite animation.
/// </summary>
[DataContract]
public sealed class SpriteAnimationStep : PropertyChangedNotifier {

    /// <summary>
    /// Gets or sets the number of frames this sprite will be seen.
    /// </summary>
    /// <value>The number of frames.</value>
    [DataMember]
    public int Frames {
        get;
        set {
            if (value > 0) {
                this.Set(ref field, value);
            }
        }
    } = 1;

    /// <summary>
    /// Gets or sets the sprite.
    /// </summary>
    /// <value>The sprite.</value>
    [DataMember]
    public byte? SpriteIndex {
        get;
        set => this.Set(ref field, value);
    }
}