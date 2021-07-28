namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs {
    using System;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for the asset selection dialog.
    /// </summary>
    public class ContentSelectionViewModel : BaseDialogViewModel {
        private readonly bool _allowDirectorySelection;
        private readonly Type _desiredAssetType;
        private FilteredContentWrapper _selectedContentNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSelectionViewModel" /> class.
        /// </summary>
        public ContentSelectionViewModel() : base() {
            this.IsOkEnabled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSelectionViewModel" /> class.
        /// </summary>
        /// <param name="contentService">The content service.</param>
        /// <param name="desiredAssetType">The desired asset type.</param>
        /// <param name="allowDirectorySelection">
        /// A value indicating whether or not the user can press 'Ok' after selecting a
        /// directory node.
        /// </param>
        [InjectionConstructor]
        public ContentSelectionViewModel(IContentService contentService, Type desiredAssetType, bool allowDirectorySelection) : this() {
            this._desiredAssetType = desiredAssetType;
            this._allowDirectorySelection = allowDirectorySelection;
            this.RootContentDirectory = new FilteredContentWrapper(contentService.RootContentDirectory, desiredAssetType);
        }

        /// <summary>
        /// Gets the root content directory as a <see cref="FilteredContentWrapper" />.
        /// </summary>
        public FilteredContentWrapper RootContentDirectory { get; }

        /// <summary>
        /// Gets the selected content node as a <see cref="FilteredContentWrapper" />.
        /// </summary>
        public FilteredContentWrapper SelectedContentNode {
            get => this._selectedContentNode;
            set {
                this.RaiseAndSetIfChanged(ref this._selectedContentNode, value);
                this.IsOkEnabled = (this._selectedContentNode?.Node is ContentFile file && this._desiredAssetType.IsInstanceOfType(file.Asset)) ||
                                   (this._allowDirectorySelection && this.SelectedContentNode?.Node is ContentDirectory);
            }
        }
    }
}