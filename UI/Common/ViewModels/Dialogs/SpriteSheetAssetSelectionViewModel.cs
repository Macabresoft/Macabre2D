namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs {
    using System.Collections.Generic;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Models.Rendering;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for the sprite sheet asset selection dialog.
    /// </summary>
    public sealed class SpriteSheetAssetSelectionViewModel<TAsset> : BaseDialogViewModel where TAsset : SpriteSheetAsset {
        private readonly ObservableCollectionExtended<SpriteSheetAssetDisplayCollection<TAsset>> _spriteSheets = new();
        private FilteredContentWrapper _selectedContentNode;
        private TAsset _selectedAsset;
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
            this.RootContentDirectory = new FilteredContentWrapper(contentService.RootContentDirectory, typeof(SpriteSheet));
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
        /// Gets or sets the selected thumbnail size.
        /// </summary>
        public ThumbnailSize SelectedThumbnailSize {
            get => this._selectedThumbnailSize;
            set => this.RaiseAndSetIfChanged(ref this._selectedThumbnailSize, value);
        }

        private void ResetSpriteSheets() {
            this._spriteSheets.Clear();

            if (this.SelectedContentNode != null) {
                if (this.SelectedContentNode.Node is ContentDirectory directory) {
                    var spriteCollections = new List<SpriteSheetAssetDisplayCollection<TAsset>>();
                    foreach (var file in directory.GetAllContentFiles()) {
                        if (file.Asset is SpriteSheet spriteSheet) {
                            spriteCollections.Add(new SpriteSheetAssetDisplayCollection<TAsset>(spriteSheet, file));
                        }
                    }

                    this._spriteSheets.Reset(spriteCollections);
                }
                else if (this.SelectedContentNode.Node is ContentFile { Asset: SpriteSheet spriteSheet } file) {
                    var spriteCollection = new SpriteSheetAssetDisplayCollection<TAsset>(spriteSheet, file);
                    this._spriteSheets.Add(spriteCollection);
                }
            }
        }
    }
}