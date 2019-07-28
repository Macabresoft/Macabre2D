namespace Macabre2D.UI.Common {

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public class ObservableRangeCollection<T> : ObservableCollection<T> {

        public ObservableRangeCollection() : base() {
        }

        public ObservableRangeCollection(IEnumerable<T> items) : base(items) {
        }

        public ObservableRangeCollection(List<T> items) : base(items) {
        }

        public void AddRange(IEnumerable<T> items) {
            foreach (var item in items) {
                this.Items.Add(item);
            }

            this.RaiseEvents();
        }

        public void Replace(T item, int index) {
            if (index < this.Items.Count) {
                var originalItem = this.Items[index];
                this.Items.RemoveAt(index);
                this.Items.Insert(index, item);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, originalItem));
            }
        }

        public void Reset(IEnumerable<T> items) {
            this.Items.Clear();
            this.AddRange(items);
            this.RaiseEvents();
        }

        private void RaiseEvents() {
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Count)));
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}