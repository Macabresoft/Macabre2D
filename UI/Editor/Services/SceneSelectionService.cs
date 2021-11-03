namespace Macabresoft.Macabre2D.UI.Editor {
    using System.Collections.Generic;
    using Avalonia.Controls;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;

    /// <summary>
    /// Interface for the selection service for the scene tree.
    /// </summary>
    public interface ISceneSelectionService {
        /// <summary>
        /// Gets the editors.
        /// </summary>
        IReadOnlyCollection<ValueControlCollection> Editors { get; }

        /// <summary>
        /// Gets the implied selected object.
        /// </summary>
        object ImpliedSelected { get; }

        /// <summary>
        /// Gets a value indicating whether or not the state of the program is in an entity context.
        /// </summary>
        bool IsEntityContext { get; }

        /// <summary>
        /// Gets or sets the selected object in the scene tree.
        /// </summary>
        object Selected { get; set; }
    }

    /// <summary>
    /// The selection service for the scene tree.
    /// </summary>
    public class SceneSelectionService : ReactiveObject, ISceneSelectionService {
        private readonly IEntityService _entityService;
        private readonly ISceneService _sceneService;
        private readonly ISystemService _systemService;
        private object _impliedSelected;
        private bool _isEntityContext;
        private object _selected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneTreeViewModel" /> class.
        /// </summary>
        /// <param name="entityService">The entity service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="systemService">The system service.</param>
        public SceneSelectionService(IEntityService entityService, ISceneService sceneService, ISystemService systemService) {
            this._entityService = entityService;
            this._sceneService = sceneService;
            this._systemService = systemService;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ValueControlCollection> Editors {
            get {
                return this._selected switch {
                    IEntity => this._entityService.Editors,
                    IUpdateableSystem => this._systemService.Editors,
                    _ => null
                };
            }
        }

        /// <inheritdoc />
        public object ImpliedSelected {
            get => this._impliedSelected;
            private set => this.RaiseAndSetIfChanged(ref this._impliedSelected, value);
        }

        /// <inheritdoc />
        public bool IsEntityContext {
            get => this._isEntityContext;
            private set => this.RaiseAndSetIfChanged(ref this._isEntityContext, value);
        }
        
        /// <inheritdoc />
        public object Selected {
            get => this._selected;
            set {
                this.RaiseAndSetIfChanged(ref this._selected, value);
                this._entityService.Selected = null;
                this._systemService.Selected = null;
                this.IsEntityContext = false;

                switch (this._selected) {
                    case IScene scene:
                        this._entityService.Selected = scene;
                        this.ImpliedSelected = this._selected;
                        break;
                    case IUpdateableSystem system:
                        this._systemService.Selected = system;
                        this.ImpliedSelected = this._selected;
                        break;
                    case IEntity entity:
                        this._entityService.Selected = entity;
                        this.ImpliedSelected = this._selected;
                        this.IsEntityContext = true;
                        break;
                    case SystemCollection:
                        this._entityService.Selected = this._sceneService.CurrentScene;
                        this.ImpliedSelected = this._sceneService.CurrentScene;
                        break;
                    case EntityCollection:
                        this.IsEntityContext = true;
                        this._entityService.Selected = this._sceneService.CurrentScene;
                        this.ImpliedSelected = this._sceneService.CurrentScene;
                        break;
                }

                this.RaisePropertyChanged(nameof(this.Editors));
            }
        }
    }
}