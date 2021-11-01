namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// An explicit observable collection of <see cref="IUpdateableSystem"/>.
    /// </summary>
    public class SystemCollection : ObservableCollection<IUpdateableSystem>, INameableCollection {
        /// <inheritdoc />
        IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() {
            return this.Items.GetEnumerator();
        }

        /// <inheritdoc />
        public string Name => "Systems";
    }
}