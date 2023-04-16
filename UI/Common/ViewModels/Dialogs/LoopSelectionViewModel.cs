namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for the loop selection dialog.
/// </summary>
public class LoopSelectionViewModel : BaseDialogViewModel {
    private ILoop _selectedLoop;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoopSelectionViewModel" /> class.
    /// </summary>
    public LoopSelectionViewModel() : base() {
        this.IsOkEnabled = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoopSelectionViewModel" /> class.
    /// </summary>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="desiredAssetType">The desired asset type.</param>
    [InjectionConstructor]
    public LoopSelectionViewModel(ISceneService sceneService, Type desiredAssetType) : this() {
        this.Loops = sceneService.CurrentScene.Loops.Where(x => x.GetType().IsAssignableTo(desiredAssetType)).ToList();
    }

    /// <summary>
    /// Gets the loops.
    /// </summary>
    public IEnumerable<ILoop> Loops { get; }

    /// <summary>
    /// Gets the selected loop.
    /// </summary>
    public ILoop SelectedLoop {
        get => this._selectedLoop;
        set {
            this.RaiseAndSetIfChanged(ref this._selectedLoop, value);
            this.IsOkEnabled = this._selectedLoop != null;
        }
    }
}