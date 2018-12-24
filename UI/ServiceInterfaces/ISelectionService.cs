namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.Framework;
    using System;
    using System.ComponentModel;

    public interface ISelectionService : INotifyPropertyChanged {

        event EventHandler<ValueChangedEventArgs<object>> SelectionChanged;

        object SelectedItem { get; set; }
    }
}