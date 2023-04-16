namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public class LoopReferenceEditor : ValueEditorControl<LoopReference> {
    public static readonly DirectProperty<LoopReferenceEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<LoopReferenceEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<LoopReferenceEditor, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<LoopReferenceEditor, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<LoopReferenceEditor, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<LoopReferenceEditor, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);

    private readonly ICommonDialogService _dialogService;
    private readonly IUndoService _undoService;

    private ICommand _clearCommand;
    private string _pathText;

    public LoopReferenceEditor() : this(
        null,
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public LoopReferenceEditor(
        ValueControlDependencies dependencies,
        ICommonDialogService dialogService,
        IUndoService undoService) : base(dependencies) {
        this._dialogService = dialogService;
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
                this.Value.WhenAny(x => x.LoopId, y => y.Value != Guid.Empty));

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
        var identifier = this.Value.LoopId;

        if (identifier != Guid.Empty) {
            var previousId = this.Value.LoopId;
            this._undoService.Do(
                () => this.Value.LoopId = Guid.Empty,
                () => this.Value.LoopId = previousId);
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetPath() {
        this.PathText = null;

        if (this.Value != null && this.Value.LoopId != Guid.Empty && this.Value.UntypedLoop != null) {
            this.PathText = this.Value.UntypedLoop.Name;
        }
    }

    private async Task Select() {
        var loop = await this._dialogService.OpenLoopSelectionDialog(this.Value.Type);
        if (loop != null) {
            var originalId = this.Value.LoopId;
            var newId = loop.Id;
            this._undoService.Do(
                () => this.Value.LoopId = newId,
                () => this.Value.LoopId = originalId);
        }
    }

    private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(LoopReference.LoopId)) {
            this.ResetPath();
        }
    }
}