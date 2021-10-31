namespace Macabresoft.Macabre2D.UI.Editor {
    using System.Collections.Generic;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;

    /// <summary>
    /// Interface for the selection service for the scene tree.
    /// </summary>
    public interface IProjectSelectionService {
        /// <summary>
        /// Gets the editors.
        /// </summary>
        IReadOnlyCollection<ValueControlCollection> Editors { get; }

        /// <summary>
        /// Gets a value indicating whether or not editors should be shown.
        /// </summary>
        bool ShowEditors { get; }

        /// <summary>
        /// Gets or sets the selected object in the project tree.
        /// </summary>
        object Selected { get; set; }
    }

    /// <summary>
    /// The selection service for the scene tree.
    /// </summary>
    public class ProjectSelectionService : ReactiveObject, IProjectSelectionService {
        private readonly IContentService _contentService;
        private readonly IProjectService _projectService;
        private object _selected;

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
        public object Selected {
            get => this._selected;
            set {
                this._selected = value;
                this.ShowEditors = false;

                switch (this._selected) {
                    case IContentNode content:
                        this.ShowEditors = true;
                        this._contentService.Selected = content;
                        break;
                    case INameableCollection:
                        this._contentService.Selected = null;
                        break;
                    default:
                        this._contentService.Selected = null;
                        break;
                }

                this.RaisePropertyChanged(nameof(this.ShowEditors));
                this.RaisePropertyChanged(nameof(this.Editors));
                this.RaisePropertyChanged(nameof(this.Selected));
            }
        }

        /// <inheritdoc />
        public bool ShowEditors { get; private set; }
    }
}