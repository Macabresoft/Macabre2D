namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;
using Unity.Resolution;

/// <summary>
/// An interface for a common dialog service.
/// </summary>
public interface ICommonDialogService : IBaseDialogService {
    /// <summary>
    /// Opens a dialog that allows the user to pick a <see cref="IContentNode" /> whose asset inherits from the specified base
    /// type.
    /// </summary>
    /// <param name="baseAssetType">The base asset type.</param>
    /// <param name="allowDirectorySelection">
    /// A value indicating whether or not the user can select a directory. Usually used
    /// when creating a new asset as opposed to loading one.
    /// </param>
    /// <returns>The selected content node.</returns>
    Task<IContentNode> OpenAssetSelectionDialog(Type baseAssetType, bool allowDirectorySelection);

    /// <summary>
    /// Opens a dialog to show the licenses.
    /// </summary>
    /// <returns>A task.</returns>
    Task OpenLicenseDialog();

    /// <summary>
    /// Opens a dialog that allows the user to pick a <see cref="Type" />.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    /// <returns>The selected type.</returns>
    Task<Type> OpenTypeSelectionDialog(IEnumerable<Type> types);

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
    private readonly List<FileDialogFilter> _fileFilters = new() {
        new FileDialogFilter {
            Name = "All",
            Extensions = new List<string> { "*" }
        },
        new FileDialogFilter {
            Name = @"Audio",
            Extensions = AudioClip.ValidFileExtensions.Select(x => x.TrimStart('.')).ToList()
        },
        new FileDialogFilter {
            Name = "Images",
            Extensions = SpriteSheet.ValidFileExtensions.Select(x => x.TrimStart('.')).ToList()
        },
        new FileDialogFilter {
            Name = "Prefabs",
            Extensions =new List<string> { PrefabAsset.FileExtension.TrimStart('.') }
        },
        new FileDialogFilter {
            Name = "Scenes",
            Extensions = new List<string> { SceneAsset.FileExtension.TrimStart('.') }
        },
        new FileDialogFilter {
            Name = "Shaders",
            Extensions = new List<string> { Shader.FileExtension.TrimStart('.') }
        }
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonDialogService" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="mainWindow">The main window.</param>
    protected CommonDialogService(IUnityContainer container, Window mainWindow) : base(container, mainWindow) {
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
    public async Task OpenLicenseDialog() {
        var window = Resolver.Resolve<LicenseDialog>();
        await window.ShowDialog(this.MainWindow);
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
    public async Task<string> ShowSingleFileSelectionDialog(string title) {
        var dialog = new OpenFileDialog {
            AllowMultiple = false,
            Filters = this._fileFilters
        };

        var result = await dialog.ShowAsync(this.MainWindow);
        return result?.FirstOrDefault();
    }
}