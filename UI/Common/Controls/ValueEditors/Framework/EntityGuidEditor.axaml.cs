namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Common.Attributes;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class EntityGuidEditor : ValueEditorControl<Guid> {
    public static readonly DirectProperty<EntityGuidEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<EntityGuidEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<EntityGuidEditor, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<EntityGuidEditor, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<EntityGuidEditor, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<EntityGuidEditor, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);

    private readonly ICommonDialogService _dialogService;
    private readonly Type _entityType = typeof(Entity);
    private readonly ISceneService _sceneService;
    private readonly IUndoService _undoService;

    private ICommand _clearCommand;
    private string _pathText;

    public EntityGuidEditor() : this(
        null,
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<ISceneService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public EntityGuidEditor(
        ValueControlDependencies dependencies,
        ICommonDialogService dialogService,
        ISceneService sceneService,
        IUndoService undoService) : base(dependencies) {
        this._dialogService = dialogService;
        this._sceneService = sceneService;
        this._undoService = undoService;

        if (dependencies is { Owner: AssetReference assetReference, ValuePropertyName: nameof(SpriteSheet.ContentId) }) {
            this._entityType = assetReference.AssetType;
        }
        else if (dependencies?.Owner?.GetType() is { } ownerType) {
            var members = ownerType.GetMember(dependencies.ValuePropertyName);
            if (members.FirstOrDefault() is { } info && info.GetCustomAttribute<AssetGuidAttribute>() is { } attribute) {
                this._entityType = attribute.AssetType;
            }
        }

        this.SelectCommand = ReactiveCommand.CreateFromTask(this.Select);
        this.InitializeComponent();

        this.ClearCommand = ReactiveCommand.Create(
            this.Clear,
            this.Value.WhenAny(x => x, y => y.Value != Guid.Empty));

        this.ResetPath();
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

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<Guid> args) {
        base.OnValueChanged(args);

        if (this.Owner != null) {
            this.ResetPath();
        }
    }

    private void Clear() {
        if (this.Value != Guid.Empty) {
            var previousId = this.Value;
            var originalPathText = this.PathText;
            this._undoService.Do(
                () =>
                {
                    this.Value = Guid.Empty;
                    this.PathText = string.Empty;
                },
                () =>
                {
                    this.Value = previousId;
                    this.PathText = originalPathText;
                });
        }
    }

    private void ResetPath() {
        this.PathText = null;

        if (this.Value != Guid.Empty && !Entity.IsNullOrEmpty(this._sceneService?.CurrentScene, out var scene)) {
            this.PathText = scene.FindChild(this.Value).Name;
        }
    }

    private async Task Select() {
        var entity = await this._dialogService.OpenEntitySelectionDialog(this._entityType);
        if (entity != null) {
            var originalId = this.Value;
            var newId = entity.Id;
            var originalPathText = this.PathText;
            this._undoService.Do(
                () =>
                {
                    this.Value = newId;
                    this.PathText = entity.Name;
                },
                () =>
                {
                    this.Value = originalId;
                    this.PathText = originalPathText;
                });
        }
    }
}