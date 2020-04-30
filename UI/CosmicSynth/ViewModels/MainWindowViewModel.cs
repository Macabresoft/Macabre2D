namespace Macabre2D.UI.CosmicSynth.ViewModels {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Services;
    using System.Windows.Input;

    public sealed class MainWindowViewModel : NotifyPropertyChanged {
        private readonly RelayCommand _redoCommand;
        private readonly RelayCommand _undoCommand;
        private readonly IUndoService _undoService;

        public MainWindowViewModel(
            IBusyService busyService,
            IUndoService undoService) {
            this.BusyService = busyService;
            this._undoService = undoService;
            this._undoCommand = new RelayCommand(() => this._undoService.Undo(), () => this._undoService.CanUndo);
            this._redoCommand = new RelayCommand(() => this._undoService.Redo(), () => this._undoService.CanRedo);
            this._undoService.PropertyChanged += this.UndoService_PropertyChanged;
        }

        public IBusyService BusyService { get; }

        public ICommand RedoCommand {
            get {
                return this._redoCommand;
            }
        }

        public ICommand UndoCommand {
            get {
                return this._undoCommand;
            }
        }

        private void UndoService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._undoService.CanRedo)) {
                this._redoCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(this._undoService.CanUndo)) {
                this._undoCommand.RaiseCanExecuteChanged();
            }
        }
    }
}