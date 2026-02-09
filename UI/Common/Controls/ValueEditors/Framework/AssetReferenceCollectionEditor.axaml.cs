namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class AssetReferenceCollectionEditor : ValueEditorControl<IAssetReferenceCollection> {

    public static readonly DirectProperty<AssetReferenceCollectionEditor, ICommand> AddCommandProperty =
        AvaloniaProperty.RegisterDirect<AssetReferenceCollectionEditor, ICommand>(
            nameof(AddCommand),
            editor => editor.AddCommand);

    public static readonly DirectProperty<AssetReferenceCollectionEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<AssetReferenceCollectionEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<AssetReferenceCollectionEditor, IReadOnlyCollection<ContentMetadata>> MetadataProperty =
        AvaloniaProperty.RegisterDirect<AssetReferenceCollectionEditor, IReadOnlyCollection<ContentMetadata>>(
            nameof(Metadata),
            editor => editor.Metadata);

    public static readonly DirectProperty<AssetReferenceCollectionEditor, ICommand> RemoveCommandProperty =
        AvaloniaProperty.RegisterDirect<AssetReferenceCollectionEditor, ICommand>(
            nameof(RemoveCommand),
            editor => editor.RemoveCommand);

    public static readonly DirectProperty<AssetReferenceCollectionEditor, ContentMetadata> SelectedMetadataProperty =
        AvaloniaProperty.RegisterDirect<AssetReferenceCollectionEditor, ContentMetadata>(
            nameof(SelectedMetadata),
            editor => editor.SelectedMetadata,
            (editor, value) => editor.SelectedMetadata = value);

    private readonly IAssetManager _assetManager;
    private readonly ICommonDialogService _dialogService;
    private readonly ObservableCollectionExtended<ContentMetadata> _metadata = new();
    private readonly IUndoService _undoService;
    private ContentMetadata _selectedMetadata;

    public AssetReferenceCollectionEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public AssetReferenceCollectionEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IUndoService undoService) : base(dependencies) {
        this._assetManager = assetManager;
        this._dialogService = dialogService;
        this._undoService = undoService;

        this.ClearCommand = ReactiveCommand.Create(this.Clear);
        this.AddCommand = ReactiveCommand.CreateFromTask(this.Add);
        this.RemoveCommand = ReactiveCommand.Create<ContentMetadata>(this.Remove);
        
        this.ResetMetadata();
        this.InitializeComponent();
    }

    public ICommand AddCommand { get; }

    public ICommand ClearCommand { get; }

    public IReadOnlyCollection<ContentMetadata> Metadata => this._metadata;

    public ICommand RemoveCommand { get; }

    public ContentMetadata SelectedMetadata {
        get => this._selectedMetadata;
        set => this.SetAndRaise(SelectedMetadataProperty, ref this._selectedMetadata, value);
    }


    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<IAssetReferenceCollection> args) {
        base.OnValueChanged(args);
        this.ResetMetadata();
    }

    private void ResetMetadata() {
        if (this.Value != null && this._assetManager != null) {
            if (this.Value.ContentIds.Any()) {
                this._metadata.AddRange(this.GetMetadata(this.Value.ContentIds));
            }
            else {
                this._metadata.Clear();
            }
        }
    }

    private async Task Add() {
        var contentNode = await this._dialogService.OpenContentSelectionDialog(this.Value.AssetType, false, this.Title);
        if (contentNode is ContentFile { Metadata: { } metadata }) {
            if (!this.Value.ContentIds.Contains(metadata.ContentId)) {
                this._undoService.Do(
                    () =>
                    {
                        this.Value.AddAsset(metadata.ContentId);
                        this._metadata.Add(metadata);
                    },
                    () =>
                    {
                        this.Value.RemoveAsset(metadata.ContentId);
                        this._metadata.Remove(metadata);
                    });
            }
        }
    }

    private void Clear() {
        var ids = this.Value.ContentIds.ToList();

        if (ids.Any()) {
            this._undoService.Do(
                () =>
                {
                    this.Value.Clear();
                    this._metadata.Clear();
                },
                () =>
                {
                    var metadata = this.GetMetadata(this.Value.ContentIds);
                    this._metadata.Reset(metadata);
                    foreach (var contentId in this._metadata.Select(x => x.ContentId)) {
                        this.Value.AddAsset(contentId);
                    }
                });
        }
    }

    private IEnumerable<ContentMetadata> GetMetadata(IEnumerable<Guid> contentIds) {
        var metadata = new List<ContentMetadata>();

        foreach (var contentId in contentIds) {
            if (this._assetManager.TryGetMetadata(contentId, out var contentMetadata)) {
                metadata.Add(contentMetadata);
            }
        }

        return metadata;
    }

    private void Remove(ContentMetadata selectedMetadata) {
        if (selectedMetadata != null && this.Value.ContentIds.Contains(selectedMetadata.ContentId)) {
            this._undoService.Do(
                () =>
                {
                    this.Value.RemoveAsset(selectedMetadata.ContentId);
                    this._metadata.Remove(selectedMetadata);
                },
                () =>
                {
                    this.Value.AddAsset(selectedMetadata.ContentId);
                    this._metadata.Add(selectedMetadata);
                });
        }
    }
}