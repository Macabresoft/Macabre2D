namespace Macabre2D.UI.Controls {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Unity;

    public partial class ModuleList : UserControl {

        public static readonly DependencyProperty ModulesProperty = DependencyProperty.Register(
            nameof(Modules),
            typeof(ObservableCollection<ModuleWrapper>),
            typeof(ModuleList),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectedModuleProperty = DependencyProperty.Register(
            nameof(SelectedModule),
            typeof(ModuleWrapper),
            typeof(ModuleList),
            new PropertyMetadata());

        private readonly RelayCommand _addModuleCommand;
        private readonly IDialogService _dialogService;
        private readonly RelayCommand _removeModuleCommand;
        private readonly IUndoService _undoService;

        public ModuleList() {
            this._dialogService = ViewContainer.Instance.Resolve<IDialogService>();
            this._undoService = ViewContainer.Instance.Resolve<IUndoService>();
            this._addModuleCommand = new RelayCommand(this.AddModule);
            this._removeModuleCommand = new RelayCommand(this.RemoveModule, () => this.SelectedModule != null);
            InitializeComponent();
        }

        public ICommand AddModuleCommand {
            get {
                return this._addModuleCommand;
            }
        }

        public ObservableCollection<ModuleWrapper> Modules {
            get { return (ObservableCollection<ModuleWrapper>)GetValue(ModulesProperty); }
            set { SetValue(ModulesProperty, value); }
        }

        public ICommand RemoveModuleCommand {
            get {
                return this._removeModuleCommand;
            }
        }

        public ModuleWrapper SelectedModule {
            get { return (ModuleWrapper)GetValue(SelectedModuleProperty); }
            set { SetValue(SelectedModuleProperty, value); }
        }

        private void AddModule() {
            var type = this._dialogService.ShowSelectTypeDialog(typeof(BaseModule));

            if (type != null) {
                var baseModule = Activator.CreateInstance(type) as BaseModule;
                var moduleWrapper = new ModuleWrapper(baseModule);

                var undoCommand = new UndoCommand(
                    () => this.Modules.Add(moduleWrapper),
                    () => this.Modules.Remove(moduleWrapper));

                this._undoService.Do(undoCommand);
            }
        }

        private void RemoveModule() {
            var module = this.SelectedModule;
            var undoCommand = new UndoCommand(
                () => this.Modules.Remove(module),
                () => this.Modules.Add(module));

            this._undoService.Do(undoCommand);
        }
    }
}