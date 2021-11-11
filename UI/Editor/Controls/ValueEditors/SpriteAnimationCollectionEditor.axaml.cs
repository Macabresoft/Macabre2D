namespace Macabresoft.Macabre2D.UI.Editor {
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;
    using Unity;

    public class SpriteAnimationCollectionEditor : ValueEditorControl<SpriteAnimationCollection> {
        public static readonly DirectProperty<SpriteAnimationCollectionEditor, ICommand> AddCommandProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationCollectionEditor, ICommand>(
                nameof(AddCommand),
                editor => editor.AddCommand);

        public static readonly DirectProperty<SpriteAnimationCollectionEditor, ICommand> EditCommandProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationCollectionEditor, ICommand>(
                nameof(EditCommand),
                editor => editor.EditCommand);

        public static readonly DirectProperty<SpriteAnimationCollectionEditor, ICommand> RemoveCommandProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationCollectionEditor, ICommand>(
                nameof(RemoveCommand),
                editor => editor.RemoveCommand);

        public static readonly DirectProperty<SpriteAnimationCollectionEditor, ICommand> RenameCommandProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationCollectionEditor, ICommand>(
                nameof(RenameCommand),
                editor => editor.RenameCommand);

        public static readonly DirectProperty<SpriteAnimationCollectionEditor, SpriteAnimation> SelectedAnimationProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationCollectionEditor, SpriteAnimation>(
                nameof(SelectedAnimation),
                editor => editor.SelectedAnimation,
                (editor, value) => editor.SelectedAnimation = value);

        private readonly IContentService _contentService;
        private readonly ILocalDialogService _dialogService;
        private readonly IUndoService _undoService;
        private SpriteAnimation _selectedAnimation;

        public SpriteAnimationCollectionEditor() {
        }

        [InjectionConstructor]
        public SpriteAnimationCollectionEditor(
            ValueControlDependencies dependencies,
            IContentService contentService,
            ILocalDialogService dialogService,
            IUndoService undoService) : base(dependencies) {
            this._contentService = contentService;
            this._dialogService = dialogService;
            this._undoService = undoService;

            var canExecute = this.WhenAny(x => x.SelectedAnimation, y => y.Value != null);
            this.AddCommand = ReactiveCommand.Create(this.AddAnimation);
            this.EditCommand = ReactiveCommand.CreateFromTask<SpriteAnimation>(async x => await this.EditAnimation(x), canExecute);
            this.RemoveCommand = ReactiveCommand.Create<SpriteAnimation>(this.RemoveAnimation, canExecute);
            this.RenameCommand = ReactiveCommand.Create<string>(this.RenameAnimation, canExecute);

            this.InitializeComponent();
        }

        public ICommand AddCommand { get; }

        public ICommand EditCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand RenameCommand { get; }

        public SpriteAnimation SelectedAnimation {
            get => this._selectedAnimation;
            set => this.SetAndRaise(SelectedAnimationProperty, ref this._selectedAnimation, value);
        }

        private void AddAnimation() {
            if (this.Value is SpriteAnimationCollection collection) {
                var animation = new SpriteAnimation {
                    Name = SpriteAnimation.DefaultName
                };

                this._undoService.Do(
                    () => { Dispatcher.UIThread.Post(() => { collection.Add(animation); }); },
                    () => { Dispatcher.UIThread.Post(() => { collection.Remove(animation); }); });
            }
        }

        private async Task EditAnimation(SpriteAnimation animation) {
            if (animation != null && this.Owner is SpriteSheet spriteSheet && this._contentService.RootContentDirectory.TryFindNode(spriteSheet.ContentId, out var file)) {
                await this._dialogService.OpenSpriteAnimationEditor(animation, spriteSheet, file);
            }

            await Task.CompletedTask;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void RemoveAnimation(SpriteAnimation animation) {
            if (animation != null && this.Value is SpriteAnimationCollection collection) {
                var index = collection.IndexOf(animation);
                this._undoService.Do(
                    () => { Dispatcher.UIThread.Post(() => { collection.Remove(animation); }); },
                    () => {
                        if (index < 0 || index >= collection.Count) {
                            Dispatcher.UIThread.Post(() => { collection.Add(animation); });
                        }
                        else {
                            Dispatcher.UIThread.Post(() => { collection.Insert(index, animation); });
                        }
                    });
            }
        }

        private void RenameAnimation(string updatedName) {
            if (this.SelectedAnimation is SpriteAnimation animation && animation.Name != updatedName) {
                var originalName = animation.Name;
                this._undoService.Do(
                    () => { Dispatcher.UIThread.Post(() => { animation.Name = updatedName; }); },
                    () => { Dispatcher.UIThread.Post(() => { animation.Name = originalName; }); });
            }
        }
    }
}