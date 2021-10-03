namespace Macabresoft.Macabre2D.UI.Editor.Controls.ValueEditors.Framework {
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;
    using Unity;

    public class AutoTileSetCollectionEditor : ValueEditorControl<AutoTileSetCollection> {
        public static readonly DirectProperty<AutoTileSetCollectionEditor, ICommand> AddCommandProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetCollectionEditor, ICommand>(
                nameof(AddCommand),
                editor => editor.AddCommand);

        public static readonly DirectProperty<AutoTileSetCollectionEditor, ICommand> EditCommandProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetCollectionEditor, ICommand>(
                nameof(EditCommand),
                editor => editor.EditCommand);

        public static readonly DirectProperty<AutoTileSetCollectionEditor, ICommand> RemoveCommandProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetCollectionEditor, ICommand>(
                nameof(RemoveCommand),
                editor => editor.RemoveCommand);

        public static readonly DirectProperty<AutoTileSetCollectionEditor, ICommand> RenameCommandProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetCollectionEditor, ICommand>(
                nameof(RenameCommand),
                editor => editor.RenameCommand);

        public static readonly DirectProperty<AutoTileSetCollectionEditor, AutoTileSet> SelectedTileSetProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetCollectionEditor, AutoTileSet>(
                nameof(SelectedTileSet),
                editor => editor.SelectedTileSet,
                (editor, value) => editor.SelectedTileSet = value);

        private readonly IDialogService _dialogService;

        private readonly IUndoService _undoService;
        private AutoTileSet _selectedTileSet;

        public AutoTileSetCollectionEditor() {
        }

        [InjectionConstructor]
        public AutoTileSetCollectionEditor(IDialogService dialogService, IUndoService undoService) {
            this._dialogService = dialogService;
            this._undoService = undoService;

            var canExecute = this.WhenAny(x => x.SelectedTileSet, y => y.Value != null);
            this.AddCommand = ReactiveCommand.Create(this.AddTileSet);
            this.EditCommand = ReactiveCommand.CreateFromTask<AutoTileSet>(async x => await this.EditTileSet(x), canExecute);
            this.RemoveCommand = ReactiveCommand.Create<AutoTileSet>(this.RemoveTileSet, canExecute);
            this.RenameCommand = ReactiveCommand.Create<string>(this.RenameTileSet, canExecute);

            this.InitializeComponent();
        }

        public ICommand AddCommand { get; }

        public ICommand EditCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand RenameCommand { get; }

        public AutoTileSet SelectedTileSet {
            get => this._selectedTileSet;
            set => this.SetAndRaise(SelectedTileSetProperty, ref this._selectedTileSet, value);
        }

        private void AddTileSet() {
            if (this.Value is AutoTileSetCollection collection) {
                var tileSet = new AutoTileSet {
                    Name = AutoTileSet.DefaultName
                };

                this._undoService.Do(
                    () => { Dispatcher.UIThread.Post(() => { collection.Add(tileSet); }); },
                    () => { Dispatcher.UIThread.Post(() => { collection.Remove(tileSet); }); });
            }
        }

        private async Task EditTileSet(AutoTileSet tileSet) {
            if (tileSet != null && this.Owner is ContentFile { Asset: SpriteSheet spriteSheet } file) {
                await this._dialogService.OpenAutoTileSetEditor(tileSet, spriteSheet, file);
            }

            await Task.CompletedTask;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void RemoveTileSet(AutoTileSet tileSet) {
            if (tileSet != null && this.Value is AutoTileSetCollection collection) {
                var index = collection.IndexOf(tileSet);
                this._undoService.Do(
                    () => { Dispatcher.UIThread.Post(() => { collection.Remove(tileSet); }); },
                    () => {
                        if (index < 0 || index >= collection.Count) {
                            Dispatcher.UIThread.Post(() => { collection.Add(tileSet); });
                        }
                        else {
                            Dispatcher.UIThread.Post(() => { collection.Insert(index, tileSet); });
                        }
                    });
            }
        }

        private void RenameTileSet(string updatedName) {
            if (this.SelectedTileSet is AutoTileSet tileSet && tileSet.Name != updatedName) {
                var originalName = tileSet.Name;
                this._undoService.Do(
                    () => { Dispatcher.UIThread.Post(() => { tileSet.Name = updatedName; }); },
                    () => { Dispatcher.UIThread.Post(() => { tileSet.Name = originalName; }); });
            }
        }
    }
}