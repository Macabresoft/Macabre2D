﻿namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;

public abstract class BaseAssetReferenceEditor<TAssetReference, TAsset> : ValueEditorControl<TAssetReference> where TAssetReference : class, IAssetReference<TAsset> where TAsset : class, IAsset {
    public static readonly DirectProperty<BaseAssetReferenceEditor<TAssetReference, TAsset>, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<BaseAssetReferenceEditor<TAssetReference, TAsset>, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<BaseAssetReferenceEditor<TAssetReference, TAsset>, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<BaseAssetReferenceEditor<TAssetReference, TAsset>, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<BaseAssetReferenceEditor<TAssetReference, TAsset>, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<BaseAssetReferenceEditor<TAssetReference, TAsset>, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);


    private ICommand _clearCommand;
    private string _pathText;

    protected BaseAssetReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IUndoService undoService) : base(dependencies) {
        this.AssetManager = assetManager;
        this.DialogService = dialogService;
        this.UndoService = undoService;

        this.SelectCommand = ReactiveCommand.CreateFromTask(this.Select);
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

    protected IAssetManager AssetManager { get; }

    protected ICommonDialogService DialogService { get; }

    protected IUndoService UndoService { get; }

    protected virtual void Clear() {
        var asset = this.Value.Asset;

        if (asset != null) {
            this.UndoService.Do(
                () => this.Value.Clear(),
                () => { this.Value.LoadAsset(asset); });
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTree(e);
        this.ResetPath();
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<TAssetReference> args) {
        base.OnValueChanged(args);

        if (args.OldValue is { HasValue: true, Value: { } reference }) {
            reference.AssetChanged -= this.Value_AssetChanged;
        }

        if (this.Value != null) {
            this.ClearCommand = ReactiveCommand.Create(
                this.Clear,
                this.Value.WhenAny(x => x.ContentId, y => y.Value != Guid.Empty));

            this.ResetPath();
            this.Value.AssetChanged += this.Value_AssetChanged;
        }
    }

    protected virtual void ResetPath() {
        this.PathText = null;

        if (this.AssetManager != null &&
            this.Value?.Asset != null &&
            this.Value.ContentId != Guid.Empty &&
            this.AssetManager.TryGetMetadata(this.Value.ContentId, out var metadata)) {
            this.PathText = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
        }
    }

    protected virtual async Task Select() {
        var contentNode = await this.DialogService.OpenAssetSelectionDialog(typeof(TAsset), false);
        if (contentNode is ContentFile { Asset: TAsset newAsset }) {
            var originalAsset = this.Value.Asset;
            this.UndoService.Do(
                () => { this.Value.LoadAsset(newAsset); },
                () =>
                {
                    if (originalAsset != null) {
                        this.Value.LoadAsset(originalAsset);
                    }
                    else {
                        this.Value.Clear();
                    }
                });
        }
    }

    private void Value_AssetChanged(object sender, bool hasAsset) {
        this.ResetPath();
    }
}