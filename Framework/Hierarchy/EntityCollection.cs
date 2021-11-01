namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// An explicit observable collection of <see cref="IEntity"/>.
    /// </summary>
    public class EntityCollection : ObservableCollection<IEntity>, INameableCollection {
        /// <inheritdoc />
        IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() {
            return this.Items.GetEnumerator();
        }

        /// <inheritdoc />
        public string Name => "Entities";
    }
}