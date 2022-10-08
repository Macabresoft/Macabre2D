namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using Avalonia.Controls;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;
using Unity.Resolution;

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
/// An interface for a service which handling assets and their editors.
/// </summary>
public interface IAssetSelectionService : ISelectionService<object> {
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
/// A service for handling assets and their editors.
/// </summary>
public sealed class AssetSelectionService : ReactiveObject, IAssetSelectionService {
    private readonly IUnityContainer _container;
    private readonly IContentService _contentService;
    private readonly List<ValueControlCollection> _editors = new();
    private readonly IProjectService _projectService;
    private readonly IUndoService _undoService;
    private readonly IValueControlService _valueControlService;
    private IControl _assetEditor;
    private object _selected;
    private ProjectSelectionType _selectionType;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetSelectionService" /> class.
    /// </summary>
    /// <param name="container">The unity container.</param>
    /// <param name="contentService">The content service.</param>
    /// <param name="projectService">The project service.</param>
    /// <param name="undoService">The undo service.</param>
    /// <param name="valueControlService">The value control service.</param>
    public AssetSelectionService(
        IUnityContainer container,
        IContentService contentService,
        IProjectService projectService,
        IUndoService undoService,
        IValueControlService valueControlService) {
        this._container = container;
        this._contentService = contentService;
        this._projectService = projectService;
        this._undoService = undoService;
        this._valueControlService = valueControlService;
    }

    /// <inheritdoc />
    public IControl AssetEditor {
        get => this._assetEditor;
        private set => this.RaiseAndSetIfChanged(ref this._assetEditor, value);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<ValueControlCollection> Editors {
        get {
            return this._selected switch {
                RootContentDirectory => this._editors,
                IContentNode => this._contentService.Editors,
                _ => null
            };
        }
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
                SpriteSheetMember => ProjectSelectionType.Asset,
                _ => ProjectSelectionType.None
            };

            this.ResetAssetEditor();
            this.ResetEditors();
            this.RaisePropertyChanged(nameof(this.Editors));
        }
    }

    /// <inheritdoc />
    public ProjectSelectionType SelectionType {
        get => this._selectionType;
        private set => this.RaiseAndSetIfChanged(ref this._selectionType, value);
    }

    private void EditorCollection_OwnedValueChanged(object sender, ValueChangedEventArgs<object> e) {
        if (sender is IValueEditor { Owner: { } } valueEditor && !string.IsNullOrEmpty(valueEditor.ValuePropertyName)) {
            var originalValue = valueEditor.Owner.GetPropertyValue(valueEditor.ValuePropertyName);
            var newValue = e.UpdatedValue;

            this._undoService.Do(() =>
            {
                valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, newValue);
                valueEditor.SetValue(newValue, false);
            }, () =>
            {
                valueEditor.Owner.SetProperty(valueEditor.ValuePropertyName, originalValue);
                valueEditor.SetValue(originalValue, false);
            });
        }
    }

    private void ResetAssetEditor() {
        if (this.SelectionType == ProjectSelectionType.Asset) {
            if (this.Selected is SpriteSheetMember { SpriteSheet: { } spriteSheet } spriteSheetAsset &&
                this._contentService.RootContentDirectory.TryFindNode(spriteSheetAsset.SpriteSheet.ContentId, out var contentFile)) {
                this.AssetEditor = spriteSheetAsset switch {
                    AutoTileSet tileSet => this._container.Resolve<AutoTileSetEditorView>(
                        new ParameterOverride(typeof(AutoTileSet), tileSet),
                        new ParameterOverride(typeof(SpriteSheetAsset), spriteSheet),
                        new ParameterOverride(typeof(ContentFile), contentFile)),
                    SpriteAnimation animation => this._container.Resolve<SpriteAnimationEditorView>(
                        new ParameterOverride(typeof(SpriteAnimation), animation),
                        new ParameterOverride(typeof(SpriteSheetAsset), spriteSheet),
                        new ParameterOverride(typeof(ContentFile), contentFile)),
                    _ => null
                };
            }
            else {
                this.AssetEditor = null;
            }
        }
        else {
            this.AssetEditor = null;
        }
    }

    private void ResetEditors() {
        foreach (var editorCollection in this._editors) {
            editorCollection.OwnedValueChanged -= this.EditorCollection_OwnedValueChanged;
        }

        this._editors.Clear();

        if (this._selected is RootContentDirectory) {
            var editors = this._valueControlService.CreateControls(this._projectService.CurrentProject);
            this._editors.AddRange(editors);

            foreach (var editorCollection in this._editors) {
                editorCollection.OwnedValueChanged += this.EditorCollection_OwnedValueChanged;
            }
        }
    }
}