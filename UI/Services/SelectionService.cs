namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.ServiceInterfaces;
    using System;

    public sealed class SelectionService : NotifyPropertyChanged, ISelectionService {
        private object _selectedItem;

        public event EventHandler<ValueChangedEventArgs<object>> SelectionChanged;

        public object SelectedItem {
            get {
                return this._selectedItem;
            }

            set {
                var oldValue = this._selectedItem;
                if (this.Set(ref this._selectedItem, value)) {
                    this.SelectionChanged.SafeInvoke(this, new ValueChangedEventArgs<object>(oldValue, value));
                }
            }
        }
    }
}