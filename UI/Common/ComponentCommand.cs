namespace Macabre2D.UI.Common {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using System;
    using System.Windows.Input;

    public sealed class ComponentCommand<T> where T : BaseComponent {
        private Action<T> _action;

        public ComponentCommand(string name, Action<T> action) : this(name, action, null) {
        }

        public ComponentCommand(string name, Action<T> action, Func<bool> canExecuteFunc) {
            this.Name = name;
            this._action = action;
            this.Command = new RelayCommand<T>(action, x => canExecuteFunc?.Invoke() ?? true);
        }

        public ICommand Command { get; }
        public string Name { get; }
    }
}