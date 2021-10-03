namespace Macabresoft.Macabre2D.UI.Editor.Controls.ValueEditors.Framework {
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    public class AssetGuidEditor : ValueEditorControl<Guid> {
        public static readonly DirectProperty<AssetGuidEditor, ICommand> ClearCommandProperty =
            AvaloniaProperty.RegisterDirect<AssetGuidEditor, ICommand>(
                nameof(ClearCommand),
                editor => editor.ClearCommand);

        public static readonly DirectProperty<AssetGuidEditor, string> PathTextProperty =
            AvaloniaProperty.RegisterDirect<AssetGuidEditor, string>(
                nameof(PathText),
                editor => editor.PathText);

        public static readonly DirectProperty<AssetGuidEditor, ICommand> SelectCommandProperty =
            AvaloniaProperty.RegisterDirect<AssetGuidEditor, ICommand>(
                nameof(SelectCommand),
                editor => editor.SelectCommand);

        private readonly IAssetManager _assetManager;
        private readonly IDialogService _dialogService;
        private readonly IUndoService _undoService;
        private Type _assetType;

        private ICommand _clearCommand;
        private string _pathText;

        public AssetGuidEditor() : this(
            Resolver.Resolve<IAssetManager>(),
            Resolver.Resolve<IDialogService>(),
            Resolver.Resolve<IUndoService>()) {
        }

        [InjectionConstructor]
        public AssetGuidEditor(
            IAssetManager assetManager,
            IDialogService dialogService,
            IUndoService undoService) {
            this._assetManager = assetManager;
            this._dialogService = dialogService;
            this._undoService = undoService;

            this.ClearCommand = ReactiveCommand.Create(
                this.Clear,
                this.WhenAny(x => x.Value, y => y.Value != Guid.Empty));
            this.SelectCommand = ReactiveCommand.CreateFromTask(this.Select);
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

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            if (owner?.GetType() is Type ownerType) {
                var members = ownerType.GetMember(valuePropertyName);
                if (members.FirstOrDefault() is MemberInfo info && info.GetCustomAttribute<AssetGuidAttribute>() is AssetGuidAttribute attribute) {
                    this._assetType = attribute.AssetType;
                }
            }

            base.Initialize(value, valueType, valuePropertyName, title, owner);
        }

        protected override void OnValueChanged() {
            base.OnValueChanged();

            if (this.Value != Guid.Empty) {
                this.ResetPath();
            }
        }

        private void Clear() {
            var originalValue = this.Value;

            if (originalValue != Guid.Empty) {
                this._undoService.Do(
                    () => this.Value = Guid.Empty,
                    () => { this.Value = originalValue; });
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void ResetPath() {
            this.PathText = null;

            if (this.Value != Guid.Empty &&
                this.Value != Guid.Empty &&
                this._assetManager.TryGetMetadata(this.Value, out var metadata) &&
                metadata != null) {
                this.PathText = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
            }
        }

        private async Task Select() {
            var contentNode = await this._dialogService.OpenAssetSelectionDialog(this._assetType, false);
            if (contentNode is ContentFile { Metadata: ContentMetadata metadata }) {
                var originalId = this.Value;
                var contentId = metadata.ContentId;
                this._undoService.Do(
                    () => { this.Value = contentId; },
                    () => { this.Value = originalId; });
            }
        }
    }
}