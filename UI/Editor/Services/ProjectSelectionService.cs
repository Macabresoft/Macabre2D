namespace Macabresoft.Macabre2D.UI.Editor {
    using System.Collections.Generic;
    using Avalonia.Controls;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;

    /// <summary>
    /// Selection types for content.
    /// </summary>
    public enum ProjectSelectionType {
        None,
        File,
        Directory,
        Asset
    }

    /// <summary>
    /// Interface for the selection service for the scene tree.
    /// </summary>
    public interface IProjectSelectionService : ISelectionService<object> {
        /// <summary>
        /// Gets the asset editor.
        /// </summary>
        IControl AssetEditor { get; }

        /// <summary>
        /// Gets the selection type.
        /// </summary>
        ProjectSelectionType SelectionType { get; }
    }

    /// <summary>
    /// The selection service for the scene tree.
    /// </summary>
    public class ProjectSelectionService : ReactiveObject, IProjectSelectionService {
        private readonly IContentService _contentService;
        private readonly IProjectService _projectService;
        private IControl _assetEditor;
        private object _selected;
        private ProjectSelectionType _selectionType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSelectionService" /> class.
        /// </summary>
        /// <param name="contentService">The content service.</param>
        /// <param name="projectService">The project service.</param>
        public ProjectSelectionService(IContentService contentService, IProjectService projectService) {
            this._contentService = contentService;
            this._projectService = projectService;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ValueControlCollection> Editors {
            get {
                return this._selected switch {
                    RootContentDirectory => this._projectService.GetEditors(),
                    IContentNode => this._contentService.Editors,
                    _ => null
                };
            }
        }

        /// <inheritdoc />
        public IControl AssetEditor {
            get => this._assetEditor;
            private set => this.RaiseAndSetIfChanged(ref this._assetEditor, value);
        }

        /// <inheritdoc />
        public object Selected {
            get => this._selected;
            set {
                this.RaiseAndSetIfChanged(ref this._selected, value);

                if (this._selected is IContentNode node) {
                    this._contentService.Selected = node;
                }
                else {
                    this._contentService.Selected = null;
                }

                this.SelectionType = this._selected switch {
                    IContentDirectory => ProjectSelectionType.Directory,
                    IContentNode => ProjectSelectionType.File,
                    SpriteSheetAsset => ProjectSelectionType.Asset,
                    _ => ProjectSelectionType.None
                };

                this.ResetAssetEditor();
                this.RaisePropertyChanged(nameof(this.Editors));
            }
        }

        /// <inheritdoc />
        public ProjectSelectionType SelectionType {
            get => this._selectionType;
            private set => this.RaiseAndSetIfChanged(ref this._selectionType, value);
        }

        private void ResetAssetEditor() {
            if (this.SelectionType == ProjectSelectionType.Asset) {
            }
            else {
                this.AssetEditor = null;
            }
        }
    }
}