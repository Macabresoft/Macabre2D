namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// View model for the systems view.
    /// </summary>
    public sealed class SystemsViewModel : ViewModelBase {
        private readonly IDialogService _dialogService;
        private readonly ISceneService _sceneService;
        private readonly IUndoService _undoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemsViewModel" /> class.
        /// </summary>
        /// <remarks>This constructor only exists for design time XAML.</remarks>
        public SystemsViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemsViewModel" /> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="systemService">The system service.</param>
        /// <param name="undoService">The undo service.</param>
        [InjectionConstructor]
        public SystemsViewModel(
            IDialogService dialogService,
            ISceneService sceneService,
            ISystemService systemService,
            IUndoService undoService) {
            this._dialogService = dialogService;
            this._sceneService = sceneService;
            this.SystemService = systemService;
            this._undoService = undoService;

            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;

            this.AddSystemCommand = ReactiveCommand.CreateFromTask<Type>(
                async x => await this.AddSystem(x),
                this._sceneService.WhenAny(x => x.CurrentScene, y => y.Value != null));

            this.RemoveSystemCommand = ReactiveCommand.Create<IUpdateableSystem>(
                this.RemoveSystem,
                this.SystemService.WhenAny(x => x.Selected, y => y.Value != null));

            this.RenameCommand = ReactiveCommand.Create<string>(
                this.RenameEntity,
                this.SystemService.WhenAny(x => x.Selected, y => y.Value != null));
        }

        /// <summary>
        /// Gets a command to add a system.
        /// </summary>
        public ICommand AddSystemCommand { get; }

        /// <summary>
        /// Gets a command to remove a system.
        /// </summary>
        public ICommand RemoveSystemCommand { get; }

        /// <summary>
        /// Gets a command for renaming a system.
        /// </summary>
        public ICommand RenameCommand { get; }

        /// <summary>
        /// Gets the systems.
        /// </summary>
        public IReadOnlyCollection<IUpdateableSystem> Systems => this._sceneService.CurrentScene?.Systems;

        /// <summary>
        /// Gets the system service.
        /// </summary>
        public ISystemService SystemService { get; }

        private async Task AddSystem(Type type) {
            if (this._sceneService.CurrentScene is IScene scene) {
                type ??= await this._dialogService.OpenTypeSelectionDialog(this.SystemService.AvailableTypes);

                if (type != null && Activator.CreateInstance(type) is IUpdateableSystem system) {
                    var originallySelected = this.SystemService.Selected;
                    this._undoService.Do(() => {
                        Dispatcher.UIThread.Post(() => {
                            scene.AddSystem(system);
                            this.SystemService.Selected = system;
                        });
                    }, () => {
                        Dispatcher.UIThread.Post(() => {
                            scene.RemoveSystem(system);
                            this.SystemService.Selected = originallySelected;
                        });
                    });
                }
            }
        }

        private void RemoveSystem(IUpdateableSystem system) {
            if (this._sceneService.CurrentScene is IScene scene) {
                this._undoService.Do(() => {
                    Dispatcher.UIThread.Post(() => {
                        scene.RemoveSystem(system);
                        this.SystemService.Selected = null;
                    });
                }, () => {
                    Dispatcher.UIThread.Post(() => {
                        scene.AddSystem(system);
                        this.SystemService.Selected = system;
                    });
                });
            }
        }

        private void RenameEntity(string updatedName) {
            if (this.SystemService.Selected is IUpdateableSystem system && system.Name != updatedName) {
                var originalName = system.Name;
                this._undoService.Do(
                    () => { Dispatcher.UIThread.Post(() => { system.Name = updatedName; }); },
                    () => { Dispatcher.UIThread.Post(() => { system.Name = originalName; }); });
            }
        }

        private void SceneService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ISceneService.CurrentScene)) {
                this.RaisePropertyChanged(nameof(this.Systems));
            }
        }
    }
}