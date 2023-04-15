namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;
using Unity;

public class EntityReferenceEditor : ValueEditorControl<EntityReference> {
    public static readonly DirectProperty<EntityReferenceEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<EntityReferenceEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<EntityReferenceEditor, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<EntityReferenceEditor, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<EntityReferenceEditor, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<EntityReferenceEditor, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);

    private readonly ILocalDialogService _dialogService;

    private readonly ISceneService _sceneService;
    private readonly IUndoService _undoService;

    private ICommand _clearCommand;
    private string _pathText;

    public EntityReferenceEditor() : this(
        null,
        Resolver.Resolve<ILocalDialogService>(),
        Resolver.Resolve<ISceneService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public EntityReferenceEditor(
        ValueControlDependencies dependencies,
        ILocalDialogService dialogService,
        ISceneService sceneService,
        IUndoService undoService) : base(dependencies) {
        this._dialogService = dialogService;
        this._sceneService = sceneService;
        this._undoService = undoService;

        this.SelectCommand = ReactiveCommand.CreateFromTask(this.Select);
        this.ResetPath();
        this.InitializeComponent();
    }

    public ICommand SelectCommand { get; }

    public ICommand ClearCommand {
        get => this._clearCommand;
        private set => this.SetAndRaise(ClearCommandProperty, ref this._clearCommand, value);
    }

    public string PathText {
        get => this._pathText;
        private set => this.SetAndRaise(PathTextProperty, ref this._pathText, value);
    }

    protected override void OnValueChanged() {
        base.OnValueChanged();

        if (this.Value != null) {
            this.ClearCommand = ReactiveCommand.Create(
                this.Clear,
                this.Value.WhenAny(x => x.EntityId, y => y.Value != Guid.Empty));

            this.ResetPath();
            this.Value.PropertyChanged += this.Value_PropertyChanged;
        }
    }

    protected override void OnValueChanging() {
        base.OnValueChanging();

        if (this.Value != null) {
            this.Value.PropertyChanged -= this.Value_PropertyChanged;
        }
    }

    private void Clear() {
        var identifier = this.Value.EntityId;

        if (identifier != Guid.Empty) {
            var previousId = this.Value.EntityId;
            this._undoService.Do(
                () => this.Value.EntityId = Guid.Empty,
                () => this.Value.EntityId = previousId);
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetPath() {
        this.PathText = null;

        if (this.Value != null && this.Value.EntityId != Guid.Empty && this.Value.UntypedEntity != null) {
            this.PathText = this.Value.UntypedEntity.Name;
        }
    }

    private async Task Select() {
        throw new NotImplementedException();
        /*var contentNode = await this._dialogService.OpenAssetSelectionDialog(typeof(PrefabAsset), false);
        if (contentNode is ContentFile file) {
            var originalId = this.Value.Asset;
            var newAsset = file.Asset as PrefabAsset;
            this._undoService.Do(
                () =>
                {
                    if (newAsset != null) {
                        this.Value.LoadAsset(newAsset);
                    }
                    else {
                        this.Value.Clear();
                    }
                },
                () =>
                {
                    if (originalId != null) {
                        this.Value.LoadAsset(originalId);
                    }
                    else {
                        this.Value.Clear();
                    }
                });
        }*/
    }

    private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(EntityReference.EntityId)) {
            this.ResetPath();
        }
    }
}