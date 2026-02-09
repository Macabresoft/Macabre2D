namespace Macabre2D.UI.Common;

using System;
using Macabresoft.AvaloniaEx;
using ReactiveUI;

/// <summary>
/// A view model for the entity selection dialog.
/// </summary>
public class EntitySelectionViewModel : BaseDialogViewModel {
    private FilteredEntityWrapper _selectedEntity;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySelectionViewModel" /> class.
    /// </summary>
    public EntitySelectionViewModel() : base() {
        this.IsOkEnabled = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySelectionViewModel" /> class.
    /// </summary>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="desiredAssetType">The desired asset type.</param>
    /// <param name="title">The title of the window.</param>
    public EntitySelectionViewModel(ISceneService sceneService, Type desiredAssetType, string title) : this() {
        this.Title = title;
        this.Scene = new FilteredEntityWrapper(sceneService.CurrentlyEditing, desiredAssetType);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySelectionViewModel" /> class.
    /// </summary>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="contentId">The content identifier.</param>
    /// <param name="title">The title of the window.</param>
    public EntitySelectionViewModel(ISceneService sceneService, Guid contentId, string title) : this() {
        this.Title = title;
        this.Scene = new FilteredEntityWrapper(sceneService.CurrentlyEditing, contentId);
    }
    
    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the scene as a <see cref="FilteredEntityWrapper" />.
    /// </summary>
    public FilteredEntityWrapper Scene { get; }

    /// <summary>
    /// Gets the selected entity as a <see cref="FilteredEntityWrapper" />.
    /// </summary>
    public FilteredEntityWrapper SelectedEntity {
        get => this._selectedEntity;
        set {
            this.RaiseAndSetIfChanged(ref this._selectedEntity, value);
            this.IsOkEnabled = this._selectedEntity?.IsSelectable == true;
        }
    }
}