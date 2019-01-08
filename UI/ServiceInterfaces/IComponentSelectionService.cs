namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using System;
    using System.ComponentModel;

    public interface IComponentSelectionService : INotifyPropertyChanged {

        event EventHandler<ValueChangedEventArgs<ComponentWrapper>> SelectionChanged;

        ComponentWrapper SelectedItem { get; set; }

        void SelectComponent(BaseComponent component);
    }
}