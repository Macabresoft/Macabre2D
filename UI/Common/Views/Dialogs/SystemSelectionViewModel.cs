namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabre2D.Framework;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for the system selection dialog.
/// </summary>
public class SystemSelectionViewModel : BaseDialogViewModel {
    private ISceneSystem _selectedSceneSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemSelectionViewModel" /> class.
    /// </summary>
    public SystemSelectionViewModel() : base() {
        this.IsOkEnabled = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemSelectionViewModel" /> class.
    /// </summary>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="desiredAssetType">The desired asset type.</param>
    /// <param name="title">The title.</param>
    [InjectionConstructor]
    public SystemSelectionViewModel(ISceneService sceneService, Type desiredAssetType, string title) : this() {
        this.Title = title;
        if (sceneService.CurrentScene is { } scene) {
            this.Systems = scene.Systems.Where(x => x.GetType().IsAssignableTo(desiredAssetType)).ToList();
        }
        else {
            this.Systems = [];
        }
    }

    /// <summary>
    /// Gets the selected system.
    /// </summary>
    public ISceneSystem SelectedSceneSystem {
        get => this._selectedSceneSystem;
        set {
            this.RaiseAndSetIfChanged(ref this._selectedSceneSystem, value);
            this.IsOkEnabled = this._selectedSceneSystem != null;
        }
    }

    /// <summary>
    /// Gets the systems.
    /// </summary>
    public IEnumerable<ISceneSystem> Systems { get; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; }
}