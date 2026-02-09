namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Macabresoft.AvaloniaEx;
using Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;
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
    /// A value indicating whether the user can select a directory. Usually used
    /// when creating a new asset as opposed to loading one.
    /// </param>
    /// <param name="title">The title of the window.</param>
    /// <returns>The selected content node.</returns>
    Task<IContentNode> OpenContentSelectionDialog(Type baseAssetType, bool allowDirectorySelection, string title);

    /// <summary>
    /// Opens a dialog that allows the user to pick an <see cref="IEntity" /> which inherits from the specified base type.
    /// </summary>
    /// <param name="baseEntityType">The base entity type.</param>
    /// <param name="title">The title of the window.</param>
    /// <returns>The selected entity.</returns>
    Task<IEntity> OpenEntitySelectionDialog(Type baseEntityType, string title);

    /// <summary>
    /// Opens a dialog that allows the user to pick an <see cref="IEntity" /> which contains the specified content.
    /// </summary>
    /// <param name="contentId">The content type.</param>
    /// <param name="title">The title of the window.</param>
    /// <returns>The selected entity.</returns>
    Task<IEntity> OpenEntitySelectionDialog(Guid contentId, string title);

    /// <summary>
    /// Opens a dialog to show the licenses.
    /// </summary>
    /// <returns>A task.</returns>
    Task OpenLicenseDialog();

    /// <summary>
    /// Opens a dialog that allows the user to pick a sprite.
    /// </summary>
    /// <param name="title">The title of the window.</param>
    /// <returns>A sprite sheet and the sprite index on the sprite sheet.</returns>
    Task<(SpriteSheet SpriteSheet, byte SpriteIndex)> OpenSpriteSelectionDialog(string title);

    /// <summary>
    /// Opens a dialog that allows the user to pick an <see cref="SpriteSheetMember" />.
    /// </summary>
    /// <param name="title">The title of the window.</param>
    /// <returns>A sprite sheet and the packaged asset identifier of the selected <see cref="SpriteSheetMember" />.</returns>
    Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog<TAsset>(string title) where TAsset : SpriteSheetMember;

    /// <summary>
    /// Opens a dialog that allows the user to pick an <see cref="SpriteSheetMember" />.
    /// </summary>
    /// <param name="assetType">The type of asset.</param>
    /// <param name="title">The title of the window.</param>
    /// <returns>A sprite sheet and the packaged asset identifier of the selected <see cref="SpriteSheetMember" />.</returns>
    Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog(Type assetType, string title);

    /// <summary>
    /// Opens a dialog that allows the user to pick a <see cref="IGameSystem" /> which inherits from the specified base type.
    /// </summary>
    /// <param name="baseSystemType">The base system type.</param>
    /// <returns>The selected entity.</returns>
    Task<IGameSystem> OpenSystemSelectionDialog(Type baseSystemType);

    /// <summary>
    /// Opens a dialog that allows the user to pick a <see cref="Type" />.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    /// <param name="defaultType">The default type to have selected upon opening the window.</param>
    /// <param name="title">The window title.</param>
    /// <returns>The selected type.</returns>
    Task<Type> OpenTypeSelectionDialog(IEnumerable<Type> types, Type defaultType, string title);

    /// <summary>
    /// Shows a dialog to lay out a font.
    /// </summary>
    /// <param name="currentLayout">The current layout.</param>
    /// <returns>The new layout.</returns>
    Task<SpriteFontLayoutResult> ShowFontLayoutDialog(string currentLayout);

    /// <summary>
    /// Shows a dialog to select a key.
    /// </summary>
    /// <returns>Selects a key.</returns>
    Task<Keys?> ShowKeySelectDialog();

    /// <summary>
    /// Shows a dialog to filter and select a resource entry.
    /// </summary>
    /// <param name="resources">The resources to filter through.</param>
    /// <param name="title">The title of the window.</param>
    /// <returns>The selected resource.</returns>
    Task<ResourceEntry?> ShowSearchResourceDialog(IEnumerable<ResourceEntry> resources, string title);

    /// <summary>
    /// Shows a dialog to select a file.
    /// </summary>
    /// <param name="title">The title of the window.</param>
    /// <returns>The path of the selected file.</returns>
    Task<string> ShowSingleFileSelectionDialog(string title);

    /// <summary>
    /// Shows a dialog with the provided list of items.
    /// </summary>
    /// <param name="title">The title of the dialog/</param>
    /// <param name="items">The items to display.</param>
    /// <returns>A task.</returns>
    Task ShowTextList(string title, IEnumerable<string> items);
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
                Patterns = AudioClip.ValidFileExtensions.Select(x => $"*{x}").ToArray()
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
    public async Task<IContentNode> OpenContentSelectionDialog(Type baseAssetType, bool allowDirectorySelection, string title) {
        IContentNode selectedNode = null;
        var window = Resolver.Resolve<ContentSelectionDialog>(
            new ParameterOverride(typeof(Type), baseAssetType),
            new ParameterOverride(typeof(bool), allowDirectorySelection),
            new ParameterOverride(typeof(string), title));

        var result = await window.ShowDialog<bool>(this.MainWindow);
        if (result && window.ViewModel != null) {
            selectedNode = window.ViewModel.SelectedContentNode?.Node;
        }

        return selectedNode;
    }

    /// <inheritdoc />
    public async Task<IEntity> OpenEntitySelectionDialog(Type baseEntityType, string title) {
        IEntity selectedEntity = null;
        var viewModel = new EntitySelectionViewModel(Resolver.Resolve<ISceneService>(), baseEntityType, title);
        var window = Resolver.Resolve<EntitySelectionDialog>(new ParameterOverride(typeof(EntitySelectionViewModel), viewModel));
        var result = await window.ShowDialog<bool>(this.MainWindow);

        if (result && window.ViewModel is { SelectedEntity.IsSelectable: true }) {
            selectedEntity = window.ViewModel.SelectedEntity.Entity;
        }

        return selectedEntity;
    }

    /// <inheritdoc />
    public async Task<IEntity> OpenEntitySelectionDialog(Guid contentId, string title) {
        IEntity selectedEntity = null;
        var viewModel = new EntitySelectionViewModel(Resolver.Resolve<ISceneService>(), contentId, title);
        var window = Resolver.Resolve<EntitySelectionDialog>(new ParameterOverride(typeof(EntitySelectionViewModel), viewModel));
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
    public abstract Task<(SpriteSheet SpriteSheet, byte SpriteIndex)> OpenSpriteSelectionDialog(string title);

    /// <inheritdoc />
    public abstract Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog<TAsset>(string title) where TAsset : SpriteSheetMember;

    /// <inheritdoc />
    public abstract Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog(Type assetType, string title);

    /// <inheritdoc />
    public async Task<IGameSystem> OpenSystemSelectionDialog(Type baseSystemType) {
        IGameSystem selectedGameSystem = null;
        var window = Resolver.Resolve<SystemSelectionDialog>(new ParameterOverride(typeof(Type), baseSystemType));
        var result = await window.ShowDialog<bool>(this.MainWindow);

        if (result) {
            selectedGameSystem = window.ViewModel.SelectedGameSystem;
        }

        return selectedGameSystem;
    }

    /// <inheritdoc />
    public async Task<Type> OpenTypeSelectionDialog(IEnumerable<Type> types, Type defaultType, string title) {
        Type selectedType = null;
        var window = Resolver.Resolve<TypeSelectionDialog>(
            new ParameterOverride(typeof(IEnumerable<Type>), types),
            new ParameterOverride(typeof(Type), defaultType),
            new ParameterOverride(typeof(string), title));

        var result = await window.ShowDialog<bool>(this.MainWindow);
        if (result && window.ViewModel != null) {
            selectedType = window.ViewModel.SelectedItem;
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
    public async Task<Keys?> ShowKeySelectDialog() {
        var window = Resolver.Resolve<KeySelectDialog>();
        var result = await window.ShowDialog<bool>(this.MainWindow);
        return result ? window.SelectedKey : null;
    }

    /// <inheritdoc />
    public async Task<ResourceEntry?> ShowSearchResourceDialog(IEnumerable<ResourceEntry> resources, string title) {
        ResourceEntry? selectedResource = null;
        var window = Resolver.Resolve<ResourceSelectionDialog>(
            new ParameterOverride(typeof(IEnumerable<ResourceEntry>), resources),
            new ParameterOverride(typeof(string), title));

        var result = await window.ShowDialog<bool>(this.MainWindow);
        if (result && window.ViewModel != null) {
            selectedResource = window.ViewModel.SelectedItem;
        }

        return selectedResource;
    }

    /// <inheritdoc />
    public async Task<string> ShowSingleFileSelectionDialog(string title) {
        var result = await this.MainWindow.StorageProvider.OpenFilePickerAsync(this._contentSelectionOptions);

        if (result.FirstOrDefault()?.TryGetLocalPath() is { } path) {
            return path;
        }

        return null;
    }

    /// <inheritdoc />
    public async Task ShowTextList(string title, IEnumerable<string> items) {
        var window = Resolver.Resolve<TextListDialog>(new ParameterOverride(typeof(IEnumerable<string>), items));
        window.Title = title;
        await window.ShowDialog(this.MainWindow);
    }
}