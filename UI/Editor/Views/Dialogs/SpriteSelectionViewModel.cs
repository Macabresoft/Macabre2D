namespace Macabre2D.UI.Editor;

using System.Collections.Generic;
using Macabre2D.Framework;
using Macabre2D.UI.Common;
using Macabresoft.Core;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for the sprite selection dialog.
/// </summary>
public sealed class SpriteSelectionViewModel : FilterableViewModel<FilteredContentWrapper> {
    private readonly ObservableCollectionExtended<SpriteDisplayCollection> _spriteSheets = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSelectionViewModel" /> class.
    /// </summary>
    public SpriteSelectionViewModel() : base() {
        this.IsOkEnabled = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSelectionViewModel" /> class.
    /// </summary>
    /// <param name="contentService">The content service.</param>
    /// <param name="title">The title of the window.</param>
    [InjectionConstructor]
    public SpriteSelectionViewModel(IContentService contentService, string title) : this() {
        this.Title = title;
        this.RootContentDirectory = new FilteredContentWrapper(contentService.RootContentDirectory, typeof(SpriteSheet), false);
        this.SelectedContentNode = this.RootContentDirectory;
    }

    /// <summary>
    /// Gets the root content directory as a <see cref="FilteredContentWrapper" />.
    /// </summary>
    public FilteredContentWrapper RootContentDirectory { get; }

    /// <summary>
    /// Gets the selected content node as a <see cref="FilteredContentWrapper" />.
    /// </summary>
    public FilteredContentWrapper SelectedContentNode {
        get;
        set {
            if (field != value && !this.IsClosing) {
                this.RaiseAndSetIfChanged(ref field, value);
                this.ResetSpriteSheets();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected sprite.
    /// </summary>
    public SpriteDisplayModel SelectedSprite {
        get;
        set {
            if (!this.IsClosing) {
                this.RaiseAndSetIfChanged(ref field, value);
                this.IsOkEnabled = this.SelectedSprite != null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected thumbnail size.
    /// </summary>
    public ThumbnailSize SelectedThumbnailSize {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets the sprite sheets via <see cref="SpriteDisplayCollection" />.
    /// </summary>
    public IReadOnlyCollection<SpriteDisplayCollection> SpriteSheets => this._spriteSheets;

    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; }

    /// <inheritdoc />
    protected override FilteredContentWrapper GetActualSelected() => this.SelectedContentNode;

    /// <inheritdoc />
    protected override IEnumerable<FilteredContentWrapper> GetNodesAvailableToFilter() => this.RootContentDirectory.GetAllFiles();

    /// <inheritdoc />
    protected override void SetActualSelected(FilteredContentWrapper selected) {
        this.SelectedContentNode = selected;
    }

    private void ResetSpriteSheets() {
        this._spriteSheets.Clear();

        if (this.SelectedContentNode != null) {
            if (this.SelectedContentNode.Node is ContentDirectory directory) {
                var spriteCollections = new List<SpriteDisplayCollection>();
                foreach (var file in directory.GetAllContentFiles()) {
                    if (file.Asset is SpriteSheet spriteSheet) {
                        spriteCollections.Add(new SpriteDisplayCollection(spriteSheet, file));
                    }
                }

                this._spriteSheets.Reset(spriteCollections);
            }
            else if (this.SelectedContentNode.Node is ContentFile { Asset: SpriteSheet spriteSheet } file) {
                var spriteCollection = new SpriteDisplayCollection(spriteSheet, file);
                this._spriteSheets.Add(spriteCollection);
            }
        }
    }
}