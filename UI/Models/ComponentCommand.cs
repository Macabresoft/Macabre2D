namespace Macabre2D.UI.Models {

    using GalaSoft.MvvmLight.CommandWpf;
    using System;
    using System.Windows.Input;

    public sealed class ComponentCommand {

        public ComponentCommand(string name, Action action) {
            this.Name = name;
            this.Command = new RelayCommand(action, true);
        }

        public ICommand Command { get; }
        public string Name { get; }
    }
}