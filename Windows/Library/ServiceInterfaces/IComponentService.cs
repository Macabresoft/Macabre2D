namespace Macabre2D.Engine.Windows.ServiceInterfaces {

    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.Models.FrameworkWrappers;
    using System;
    using System.ComponentModel;

    public interface IComponentService : INotifyPropertyChanged {

        event EventHandler<ValueChangedEventArgs<ComponentWrapper>> SelectionChanged;

        ComponentWrapper SelectedItem { get; set; }

        void ResetSelectedItemBoundingArea();

        void SelectComponent(BaseComponent component);
    }
}