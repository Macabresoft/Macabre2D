namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using System;
    using System.ComponentModel;

    public interface IComponentService : INotifyPropertyChanged {

        event EventHandler<ValueChangedEventArgs<ComponentWrapper>> SelectionChanged;

        ComponentWrapper SelectedItem { get; set; }

        void ResetSelectedItemBoundingArea();

        void SelectComponent(BaseComponent component);
    }
}