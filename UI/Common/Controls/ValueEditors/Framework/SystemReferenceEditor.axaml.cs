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

public partial class SystemReferenceEditor : ValueEditorControl<SystemReference> {
    public static readonly DirectProperty<SystemReferenceEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<SystemReferenceEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<SystemReferenceEditor, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<SystemReferenceEditor, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<SystemReferenceEditor, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<SystemReferenceEditor, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);

    private readonly ICommonDialogService _dialogService;
    private readonly IUndoService _undoService;

    private ICommand _clearCommand;
    private string _pathText;

    public SystemReferenceEditor() : this(
        null,
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public SystemReferenceEditor(
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

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<SystemReference> args) {
        base.OnValueChanged(args);
        
        if (args.OldValue is { HasValue: true, Value: { } reference }) {
            reference.PropertyChanged -= this.Value_PropertyChanged;
        }

        if (this.Value != null) {
            this.ClearCommand = ReactiveCommand.Create(
                this.Clear,
                this.Value.WhenAny(x => x.SystemId, y => y.Value != Guid.Empty));

            this.ResetPath();
            this.Value.PropertyChanged += this.Value_PropertyChanged;
        }
    }

    private void Clear() {
        var identifier = this.Value.SystemId;

        if (identifier != Guid.Empty) {
            var previousId = this.Value.SystemId;
            this._undoService.Do(
                () => this.Value.SystemId = Guid.Empty,
                () => this.Value.SystemId = previousId);
        }
    }

    private void ResetPath() {
        this.PathText = null;

        if (this.Value != null && this.Value.SystemId != Guid.Empty && this.Value.UntypedSystem != null) {
            this.PathText = this.Value.UntypedSystem.Name;
        }
    }

    private async Task Select() {
        var system = await this._dialogService.OpenSystemSelectionDialog(this.Value.Type);
        if (system != null) {
            var originalId = this.Value.SystemId;
            var newId = system.Id;
            this._undoService.Do(
                () => this.Value.SystemId = newId,
                () => this.Value.SystemId = originalId);
        }
    }

    private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(SystemReference.SystemId)) {
            this.ResetPath();
        }
    }
}