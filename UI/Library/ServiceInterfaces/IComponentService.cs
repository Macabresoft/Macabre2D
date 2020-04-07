namespace Macabre2D.UI.Library.ServiceInterfaces {

    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Models.FrameworkWrappers;
    using System;
    using System.ComponentModel;

    public interface IComponentService : INotifyPropertyChanged {

        event EventHandler<ValueChangedEventArgs<ComponentWrapper>> SelectionChanged;

        ComponentWrapper SelectedItem { get; set; }

        void ResetSelectedItemBoundingArea();

        void SelectComponent(BaseComponent component);
    }
}