namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;
using Unity.Resolution;

/// <summary>
/// An interface for a common dialog service.
/// </summary>
public interface ICommonDialogService : IBaseDialogService {
    /// <summary>
    /// Opens a dialog that allows the user to pick a <see cref="IContentNode" /> whose asset inherits from the specified base type.
    /// </summary>
    /// <param name="baseAssetType">The base asset type.</param>
    /// <param name="allowDirectorySelection">
    /// A value indicating whether or not the user can select a directory. Usually used
    /// when creating a new asset as opposed to loading one.
    /// </param>
    /// <returns>The selected content node.</returns>
    Task<IContentNode> OpenAssetSelectionDialog(Type baseAssetType, bool allowDirectorySelection);

    /// <summary>
    /// Opens a dialog that allows the user to pick an <see cref="IEntity" /> which inherits from the specified base type.
    /// </summary>
    /// <param name="baseEntityType">The base entity type.</param>
    /// <returns>The selected entity.</returns>
    Task<IEntity> OpenEntitySelectionDialog(Type baseEntityType);

    /// <summary>
    /// Opens a dialog to show the licenses.
    /// </summary>
    /// <returns>A task.</returns>
    Task OpenLicenseDialog();

    /// <summary>
    /// Opens a dialog that allows the user to pick an <see cref="ILoop" /> which inherits from the specified base type.
    /// </summary>
    /// <param name="baseLoopType">The base loop type.</param>
    /// <returns>The selected entity.</returns>
    Task<ILoop> OpenLoopSelectionDialog(Type baseLoopType);

    /// <summary>
    /// Opens a dialog that allows the user to pick a <see cref="Type" />.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    /// <returns>The selected type.</returns>
    Task<Type> OpenTypeSelectionDialog(IEnumerable<Type> types);

    /// <summary>
    /// Shows a dialog to lay out a font.
    /// </summary>
    /// <param name="currentLayout">The current layout.</param>
    /// <returns>The new layout.</returns>
    Task<SpriteFontLayoutResult> ShowFontLayoutDialog(string currentLayout);

    /// <summary>
    /// Shows a dialog to select a file.
    /// </summary>
    /// <param name="title">The title of the window.</param>
    /// <returns>The path of the selected file.</returns>
    Task<string> ShowSingleFileSelectionDialog(string title);
}

/// <summary>
/// A common dialog service.
/// </summary>
public abstract class CommonDialogService : BaseDialogService, ICommonDialogService {
    private readonly FilePickerOpenOptions _contentSelectionOptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonDialogService" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="mainWindow">The main window.</param>
    protected CommonDialogService(IUnityContainer container, Window mainWindow) : base(container, mainWindow) {
        this._contentSelectionOptions.AllowMultiple = false;
        this._contentSelectionOptions.FileTypeFilter = new[] {
            new FilePickerFileType("All") {
                Patterns = new[] { "*" }
            },
            new FilePickerFileType("Audio") {
                Patterns = AudioClipAsset.ValidFileExtensions.Select(x => $"*{x}").ToArray()
            },
            new FilePickerFileType("Images") {
                Patterns = SpriteSheet.ValidFileExtensions.Select(x => $"*{x}").ToArray()
            },
            new FilePickerFileType("Prefabs") {
                Patterns = new[] { $"*{PrefabAsset.FileExtension}" }
            },
            new FilePickerFileType("Scenes") {
                Patterns = new[] { $"*{SceneAsset.FileExtension}" }
            },
            new FilePickerFileType("Shaders") {
                Patterns = new[] { $"*{ShaderAsset.FileExtension}" }
            }
        };
    }

    /// <inheritdoc />
    public async Task<IContentNode> OpenAssetSelectionDialog(Type baseAssetType, bool allowDirectorySelection) {
        IContentNode selectedNode = null;
        var window = Resolver.Resolve<ContentSelectionDialog>(new ParameterOverride(typeof(Type), baseAssetType), new ParameterOverride(typeof(bool), allowDirectorySelection));
        var result = await window.ShowDialog<bool>(this.MainWindow);

        if (result && window.ViewModel != null) {
            selectedNode = window.ViewModel.SelectedContentNode?.Node;
        }

        return selectedNode;
    }

    /// <inheritdoc />
    public async Task<IEntity> OpenEntitySelectionDialog(Type baseEntityType) {
        IEntity selectedEntity = null;
        var window = Resolver.Resolve<EntitySelectionDialog>(new ParameterOverride(typeof(Type), baseEntityType));
        var result = await window.ShowDialog<bool>(this.MainWindow);

        if (result && window.ViewModel is { SelectedEntity.IsSelectable: true }) {
            selectedEntity = window.ViewModel.SelectedEntity.Entity;
        }

        return selectedEntity;
    }

    /// <inheritdoc />
    public async Task OpenLicenseDialog() {
        var window = Resolver.Resolve<LicenseDialog>();
        await window.ShowDialog(this.MainWindow);
    }

    /// <inheritdoc />
    public async Task<ILoop> OpenLoopSelectionDialog(Type baseLoopType) {
        ILoop selectedLoop = null;
        var window = Resolver.Resolve<LoopSelectionDialog>(new ParameterOverride(typeof(Type), baseLoopType));
        var result = await window.ShowDialog<bool>(this.MainWindow);

        if (result) {
            selectedLoop = window.ViewModel.SelectedLoop;
        }

        return selectedLoop;
    }

    /// <inheritdoc />
    public async Task<Type> OpenTypeSelectionDialog(IEnumerable<Type> types) {
        Type selectedType = null;
        var window = Resolver.Resolve<TypeSelectionDialog>(new ParameterOverride(typeof(IEnumerable<Type>), types));
        var result = await window.ShowDialog<bool>(this.MainWindow);

        if (result && window.ViewModel != null) {
            selectedType = window.ViewModel.SelectedType;
        }

        return selectedType;
    }

    /// <inheritdoc />
    public async Task<SpriteFontLayoutResult> ShowFontLayoutDialog(string currentLayout) {
        var window = Resolver.Resolve<SpriteFontLayoutDialog>(new ParameterOverride(typeof(string), currentLayout));
        var result = await window.ShowDialog<bool>(this.MainWindow);
        return result ? window.ViewModel.Result : null;
    }

    /// <inheritdoc />
    public async Task<string> ShowSingleFileSelectionDialog(string title) {
        var result = await this.MainWindow.StorageProvider.OpenFilePickerAsync(this._contentSelectionOptions);

        if (result.FirstOrDefault()?.TryGetLocalPath() is { } path) {
            return path;
        }

        return null;
    }
}