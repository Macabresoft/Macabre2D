namespace Macabre2D.UI.CommonLibrary.Controls {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public partial class CollectionFilterBox : UserControl, INotifyPropertyChanged {

        public static readonly DependencyProperty FilterFuncProperty = DependencyProperty.Register(
            nameof(FilterFunc),
            typeof(Func<object, string, bool>),
            typeof(CollectionFilterBox),
            new PropertyMetadata());

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(ICollection),
            typeof(CollectionFilterBox),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private readonly CollectionViewSource _collectionViewSource = new CollectionViewSource();

        public CollectionFilterBox() {
            this._collectionViewSource.Filter += this.FilterItems;
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable FilteredItems {
            get {
                return this._collectionViewSource.View;
            }
        }

        public Func<object, string, bool> FilterFunc {
            get { return (Func<object, string, bool>)this.GetValue(FilterFuncProperty); }
            set { this.SetValue(FilterFuncProperty, value); }
        }

        public ICollection ItemsSource {
            get { return (ICollection)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is CollectionFilterBox control) {
                if (e.NewValue != null) {
                    control._collectionViewSource.Source = e.NewValue;
                }
                else {
                    control._collectionViewSource.Source = new List<object>();
                }

                control.RaiseCollectionChanged();
            }
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e) {
            this._collectionViewSource.View?.Refresh();
            this.RaiseCollectionChanged();
        }

        private void FilterItems(object sender, FilterEventArgs e) {
            e.Accepted = this.FilterFunc == null || string.IsNullOrEmpty(this._filterBox.Text) || this.FilterFunc(e.Item, this._filterBox.Text);
        }

        private void RaiseCollectionChanged() {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.FilteredItems)));
        }
    }
}