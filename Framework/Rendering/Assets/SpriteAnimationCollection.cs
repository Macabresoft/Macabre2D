namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Macabresoft.Core;

    /// <summary>
    /// An observable collection of <see cref="SpriteAnimation"/>.
    /// </summary>
    [DataContract]
    [Category("Animations")]
    public class SpriteAnimationCollection : ObservableCollectionExtended<SpriteAnimation> {
    }
}