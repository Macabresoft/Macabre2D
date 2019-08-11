namespace Macabre2D.UI.Controls.AssetEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class SpriteAnimationAssetEditor : UserControl, INotifyPropertyChanged {

        public static readonly DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(SpriteAnimationAsset),
            typeof(SpriteAnimationAssetEditor),
            new PropertyMetadata());

        private readonly RelayCommand _addStepCommand;
        private readonly IDialogService _dialogService;
        private readonly RelayCommand _moveDownCommand;
        private readonly RelayCommand _moveUpCommand;
        private readonly IProjectService _projectService;
        private readonly RelayCommand _removeStepCommand;
        private readonly IUndoService _undoService;
        private SpriteAnimationStepWrapper _selectedStep;

        public SpriteAnimationAssetEditor() {
            this._dialogService = ViewContainer.Resolve<IDialogService>();
            this._projectService = ViewContainer.Resolve<IProjectService>();
            this._undoService = ViewContainer.Resolve<IUndoService>();
            this._addStepCommand = new RelayCommand(this.AddStep, () => this.Asset != null);
            this._removeStepCommand = new RelayCommand(this.RemoveStep, () => this.Asset != null && this.SelectedStep != null);
            this._moveDownCommand = new RelayCommand(this.MoveDown, this.CanMoveDown);
            this._moveUpCommand = new RelayCommand(this.MoveUp, this.CanMoveUp);
            this.ClearSpriteCommand = new RelayCommand<SpriteAnimationStepWrapper>(this.ClearSprite);
            this.SelectSpriteCommand = new RelayCommand<SpriteAnimationStepWrapper>(this.SelectSprite);
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddStepCommand {
            get {
                return this._addStepCommand;
            }
        }

        public SpriteAnimationAsset Asset {
            get { return (SpriteAnimationAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public ICommand ClearSpriteCommand { get; }

        public ICommand MoveDownCommand {
            get {
                return this._moveDownCommand;
            }
        }

        public ICommand MoveUpCommand {
            get {
                return this._moveUpCommand;
            }
        }

        public ICommand RemoveStepCommand {
            get {
                return this._removeStepCommand;
            }
        }

        public SpriteAnimationStepWrapper SelectedStep {
            get {
                return this._selectedStep;
            }

            set {
                this._selectedStep = value;
                this._removeStepCommand.RaiseCanExecuteChanged();
                this._moveDownCommand.RaiseCanExecuteChanged();
                this._moveUpCommand.RaiseCanExecuteChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedStep)));
            }
        }

        public ICommand SelectSpriteCommand { get; }

        private void AddStep() {
            SpriteAnimationStepWrapper newValue = null;
            var index = 0;
            var undoCommand = new UndoCommand(() => {
                if (newValue == null) {
                    newValue = this.Asset.AddStep();
                    index = this.Asset.SavableValue.Steps.Count;
                }
                else {
                    if (index > 0) {
                        this.Asset.AddStep(newValue, index);
                    }
                    else {
                        this.Asset.AddStep(newValue, this.Asset.SavableValue.Steps.Count);
                    }
                }
            }, () => {
                this.Asset.RemoveStep(newValue);
                this.SelectedStep = this.Asset.Steps.FirstOrDefault();
            });

            this._undoService.Do(undoCommand);
        }

        private bool CanMoveDown() {
            var result = false;
            if (this.SelectedStep != null) {
                result = this.Asset.Steps.IndexOf(this.SelectedStep) < this.Asset.Steps.Count - 1;
            }

            return result;
        }

        private bool CanMoveUp() {
            var result = false;
            if (this.SelectedStep != null) {
                result = this.Asset.Steps.IndexOf(this.SelectedStep) > 0;
            }

            return result;
        }

        private void ClearSprite(SpriteAnimationStepWrapper step) {
            step.Sprite = null;
        }

        private void MoveDown() {
            var step = this.SelectedStep;
            var undoCommand = new UndoCommand(() => {
                this.MoveDown(step);
            }, () => {
                this.MoveUp(step);
            });

            this._undoService.Do(undoCommand);
        }

        private void MoveDown(SpriteAnimationStepWrapper step) {
            this.SelectedStep = null;
            var index = this.Asset.Steps.IndexOf(step);
            this.Asset.RemoveStep(step);
            this.Asset.AddStep(step, index + 1);
            this.SelectedStep = step;
        }

        private void MoveUp() {
            var step = this.SelectedStep;
            var undoCommand = new UndoCommand(() => {
                this.MoveUp(step);
            }, () => {
                this.MoveDown(step);
            });

            this._undoService.Do(undoCommand);
        }

        private void MoveUp(SpriteAnimationStepWrapper step) {
            this.SelectedStep = null;
            var index = this.Asset.Steps.IndexOf(step);
            this.Asset.RemoveStep(step);
            this.Asset.AddStep(step, index - 1);
            this.SelectedStep = step;
        }

        private void RemoveStep() {
            var originalValue = this.SelectedStep;
            var index = 0;
            var undoCommand = new UndoCommand(() => {
                var originalWrapper = this.Asset.Steps.FirstOrDefault(x => x == originalValue);
                index = this.Asset.Steps.IndexOf(originalWrapper);
                this.Asset.RemoveStep(originalWrapper);
                this.SelectedStep = this.Asset.Steps.FirstOrDefault();
            }, () => {
                this.Asset.AddStep(originalValue, index);
                this.SelectedStep = originalValue;
            });

            this._undoService.Do(undoCommand);
        }

        private void SelectSprite(SpriteAnimationStepWrapper step) {
            if (this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, AssetType.Image | AssetType.Sprite, AssetType.Sprite, true, out var asset)) {
                step.Sprite = asset as SpriteWrapper;
            }
        }
    }
}