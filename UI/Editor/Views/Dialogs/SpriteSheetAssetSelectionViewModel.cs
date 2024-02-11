namespace Macabresoft.Macabre2D.UI.Editor;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for the sprite sheet asset selection dialog.
/// </summary>
public sealed class SpriteSheetAssetSelectionViewModel<TAsset> : FilterableViewModel<FilteredContentWrapper> where TAsset : SpriteSheetMember {
    private readonly ObservableCollectionExtended<SpriteSheetAssetDisplayCollection<TAsset>> _spriteSheets = new();
    private bool _isFinished;
    private TAsset _selectedAsset;
    private FilteredContentWrapper _selectedContentNode;
    private ThumbnailSize _selectedThumbnailSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetAssetSelectionViewModel{T}" /> class.
    /// </summary>
    public SpriteSheetAssetSelectionViewModel() : base() {
        this.IsOkEnabled = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetAssetSelectionViewModel{T}" /> class.
    /// </summary>
    /// <param name="contentService">The content service.</param>
    [InjectionConstructor]
    public SpriteSheetAssetSelectionViewModel(IContentService contentService) : this() {
        this.RootContentDirectory = new FilteredContentWrapper(contentService.RootContentDirectory, typeof(SpriteSheet), false, file => ShouldDisplay(file, out _));
        this.SelectedContentNode = this.RootContentDirectory;
    }

    /// <summary>
    /// Gets the root content directory as a <see cref="FilteredContentWrapper" />.
    /// </summary>
    public FilteredContentWrapper RootContentDirectory { get; }

    /// <summary>
    /// Gets the sprite sheets via <see cref="SpriteSheetAssetDisplayCollection{TAsset}" />.
    /// </summary>
    public IReadOnlyCollection<SpriteSheetAssetDisplayCollection<TAsset>> SpriteSheets => this._spriteSheets;

    /// <summary>
    /// Gets or sets the selected asset.
    /// </summary>
    public TAsset SelectedAsset {
        get => this._selectedAsset;
        set {
            this.RaiseAndSetIfChanged(ref this._selectedAsset, value);
            this.IsOkEnabled = this.SelectedAsset != null;
        }
    }

    /// <summary>
    /// Gets the selected content node as a <see cref="FilteredContentWrapper" />.
    /// </summary>
    public FilteredContentWrapper SelectedContentNode {
        get => this._selectedContentNode;
        set {
            if (this._selectedContentNode != value) {
                this.RaiseAndSetIfChanged(ref this._selectedContentNode, value);
                this.ResetSpriteSheets();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected thumbnail size.
    /// </summary>
    public ThumbnailSize SelectedThumbnailSize {
        get => this._selectedThumbnailSize;
        set => this.RaiseAndSetIfChanged(ref this._selectedThumbnailSize, value);
    }

    /// <inheritdoc />
    protected override FilteredContentWrapper GetActualSelected() => this.SelectedContentNode;

    /// <inheritdoc />
    protected override IEnumerable<FilteredContentWrapper> GetNodesAvailableToFilter() => this.RootContentDirectory.GetAllFiles();

    /// <inheritdoc />
    protected override void OnOk() {
        this._isFinished = true;
        base.OnOk();
    }

    /// <inheritdoc />
    protected override void SetActualSelected(FilteredContentWrapper selected) {
        this.SelectedContentNode = selected;
    }

    private void ResetSpriteSheets() {
        if (this._isFinished) {
            return;
        }

        this._spriteSheets.Clear();

        if (this.SelectedContentNode != null) {
            switch (this.SelectedContentNode.Node) {
                case ContentDirectory directory: {
                    var spriteCollections = new List<SpriteSheetAssetDisplayCollection<TAsset>>();
                    foreach (var file in directory.GetAllContentFiles()) {
                        if (ShouldDisplay(file, out var spriteSheet)) {
                            spriteCollections.Add(new SpriteSheetAssetDisplayCollection<TAsset>(spriteSheet, file));
                        }
                    }

                    this._spriteSheets.Reset(spriteCollections);
                    break;
                }
                case ContentFile file: {
                    if (ShouldDisplay(file, out var spriteSheet)) {
                        var spriteCollection = new SpriteSheetAssetDisplayCollection<TAsset>(spriteSheet, file);
                        this._spriteSheets.Add(spriteCollection);
                    }

                    break;
                }
            }
        }
    }

    private static bool ShouldDisplay(ContentFile file, [NotNullWhen(true)] out SpriteSheet spriteSheet) {
        spriteSheet = file.Asset as SpriteSheet;
        return spriteSheet != null && spriteSheet.GetAssets<TAsset>().Any();
    }
}