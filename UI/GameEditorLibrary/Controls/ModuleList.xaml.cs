namespace Macabre2D.UI.GameEditorLibrary.Controls {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Services;
    using Macabre2D.UI.GameEditorLibrary.Services;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Unity;

    public partial class ModuleList : UserControl {

        public static readonly DependencyProperty ModulesProperty = DependencyProperty.Register(
            nameof(Modules),
            typeof(ObservableCollection<BaseModule>),
            typeof(ModuleList),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectedModuleProperty = DependencyProperty.Register(
            nameof(SelectedModule),
            typeof(BaseModule),
            typeof(ModuleList),
            new PropertyMetadata(null, new PropertyChangedCallback(OnModuleChanged)));

        private readonly RelayCommand _addModuleCommand;
        private readonly IGameDialogService _dialogService = ViewContainer.Instance.Resolve<IGameDialogService>();
        private readonly RelayCommand _removeModuleCommand;
        private readonly ISceneService _sceneService = ViewContainer.Instance.Resolve<ISceneService>();
        private readonly IUndoService _undoService = ViewContainer.Instance.Resolve<IUndoService>();

        public ModuleList() {
            this._addModuleCommand = new RelayCommand(this.AddModule);
            this._removeModuleCommand = new RelayCommand(this.RemoveModule, () => this.SelectedModule != null);
            this.InitializeComponent();
        }

        public ICommand AddModuleCommand {
            get {
                return this._addModuleCommand;
            }
        }

        public ObservableCollection<BaseModule> Modules {
            get { return (ObservableCollection<BaseModule>)this.GetValue(ModulesProperty); }
            set { this.SetValue(ModulesProperty, value); }
        }

        public ICommand RemoveModuleCommand {
            get {
                return this._removeModuleCommand;
            }
        }

        public BaseModule SelectedModule {
            get { return (BaseModule)this.GetValue(SelectedModuleProperty); }
            set { this.SetValue(SelectedModuleProperty, value); }
        }

        private static void OnModuleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is ModuleList control) {
                if (e.NewValue is BaseModule newModule) {
                    newModule.PropertyChanged += control.NewModule_PropertyChanged;
                }

                if (e.OldValue is BaseModule oldModule) {
                    oldModule.PropertyChanged -= control.NewModule_PropertyChanged;
                }
            }
        }

        private void AddModule() {
            var type = this._dialogService.ShowSelectTypeDialog(typeof(BaseModule), "Select a Module");

            if (type != null) {
                var module = Activator.CreateInstance(type) as BaseModule;
                module.Name = type.Name;

                var hasChanges = this._sceneService.CurrentScene.HasChanges;
                var undoCommand = new UndoCommand(
                    () => {
                        this.Modules.Add(module);
                        this._sceneService.CurrentScene.HasChanges = true;
                    },
                    () => {
                        this.Modules.Remove(module);
                        this._sceneService.CurrentScene.HasChanges = hasChanges;
                    });

                this._undoService.Do(undoCommand);
            }
        }

        private void NewModule_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this._sceneService.CurrentScene.HasChanges = true;
        }

        private void RemoveModule() {
            var module = this.SelectedModule;

            var hasChanges = this._sceneService.CurrentScene.HasChanges;
            var undoCommand = new UndoCommand(
                () => {
                    this.Modules.Remove(module);
                    this._sceneService.CurrentScene.HasChanges = true;
                },
                () => {
                    this.Modules.Add(module);
                    this._sceneService.CurrentScene.HasChanges = hasChanges;
                });

            this._undoService.Do(undoCommand);
        }
    }
}