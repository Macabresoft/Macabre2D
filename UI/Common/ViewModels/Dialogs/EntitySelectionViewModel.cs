namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Macabresoft.AvaloniaEx;
using ReactiveUI;
using Unity;

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
    [InjectionConstructor]
    public EntitySelectionViewModel(ISceneService sceneService, Type desiredAssetType) : this() {
        this.Scene = new FilteredEntityWrapper(sceneService.CurrentScene, desiredAssetType);
    }

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