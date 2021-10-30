namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Macabresoft.Core;

    /// <summary>
    /// An observable collection of <see cref="SpriteAnimation" />.
    /// </summary>
    [DataContract]
    [Category("Animations")]
    public class SpriteAnimationCollection : ObservableCollectionExtended<SpriteAnimation>, INameableCollection {
        /// <inheritdoc />
        public string Name => "Sprite Animations";

        /// <inheritdoc />
        IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() {
            return this.Items.GetEnumerator();
        }
    }
}