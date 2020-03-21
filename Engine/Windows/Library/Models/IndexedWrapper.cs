namespace Macabre2D.Engine.Windows.Models {

    using Macabre2D.Framework;

    public sealed class IndexedWrapper<T> : NotifyPropertyChanged {
        private T _wrappedObject;

        public IndexedWrapper(T wrappedObject, int index) : this(index) {
            this._wrappedObject = wrappedObject;
        }

        public IndexedWrapper(int index) {
            this.Index = index;
        }

        public int Index { get; }

        public T WrappedObject {
            get {
                return this._wrappedObject;
            }

            set {
                this.Set(ref this._wrappedObject, value);
            }
        }
    }
}