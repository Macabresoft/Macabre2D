namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ReactiveUI;

/// <summary>
/// Interface for a service which handles saving.
/// </summary>
public interface ISaveService : INotifyPropertyChanged {
    /// <summary>
    /// Gets or sets a value indicating whether or not this has changes.
    /// </summary>
    bool HasChanges { get; }

    /// <summary>
    /// Requests a save with a dialog.
    /// </summary>
    /// <returns>The dialog result.</returns>
    Task<YesNoCancelResult> RequestSave();

    /// <summary>
    /// Saves the scene and project.
    /// </summary>
    void Save();
}

/// <summary>
/// A service which handles saving.
/// </summary>
public class SaveService : ReactiveObject, ISaveService {
    private readonly IContentService _contentService;
    private readonly ICommonDialogService _dialogService;
    private readonly IProjectService _projectService;
    private readonly ISceneService _sceneService;
    private readonly IUndoService _undoService;
    private Guid _savedChangeId;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveService" /> class.
    /// </summary>
    /// <param name="contentService"></param>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="projectService">The project service.</param>
    /// <param name="sceneService">The scene service.</param>
    /// <param name="undoService">The undo service.</param>
    public SaveService(
        IContentService contentService,
        ICommonDialogService dialogService,
        IProjectService projectService,
        ISceneService sceneService,
        IUndoService undoService) {
        this._contentService = contentService;
        this._dialogService = dialogService;
        this._projectService = projectService;
        this._sceneService = sceneService;
        this._undoService = undoService;
        this._undoService.PropertyChanged += this.UndoService_PropertyChanged;
    }

    /// <inheritdoc />
    public bool HasChanges => this._undoService.LatestChangeId != this._savedChangeId;

    /// <inheritdoc />
    public async Task<YesNoCancelResult> RequestSave() {
        var result = YesNoCancelResult.No;
        if (this.HasChanges) {
            result = await this._dialogService.ShowYesNoDialog("Unsaved Changes", "Save changes before closing?", true);

            if (result == YesNoCancelResult.Yes) {
                this.Save();
            }
        }

        return result;
    }

    /// <inheritdoc />
    public void Save() {
        if (this.HasChanges) {
            this._sceneService.SaveScene();
            this._projectService.SaveProject();
            this._contentService.Save();
            this._savedChangeId = this._undoService.LatestChangeId;
            this.RaisePropertyChanged(nameof(this.HasChanges));
        }
    }

    private void UndoService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IUndoService.LatestChangeId)) {
            this.RaisePropertyChanged(nameof(this.HasChanges));
        }
    }
}