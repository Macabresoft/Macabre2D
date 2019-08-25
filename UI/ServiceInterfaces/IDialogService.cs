namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using System;
    using System.Windows;

    public interface IDialogService {

        bool ShowAssetNameChangeDialog(string name, Asset asset, FolderAsset parent, out string newName);

        bool ShowCreateProjectDialog(out Project project, string initialDirectory = null);

        bool ShowFileBrowser(string filter, out string path, string initialDirectory = null);

        bool ShowFolderBrowser(out string path, string initialDirectory = null);

        bool ShowSaveAssetAsDialog(AddableAsset asset);

        SaveDiscardCancelResult ShowSaveDiscardCancelDialog();

        SceneAsset ShowSaveSceneWindow(Project project, Scene scene);

        bool ShowSelectAssetDialog(Project project, AssetType assetMask, AssetType selectableAssetMask, bool allowNull, out Asset asset);

        bool ShowSelectSpriteDialog(out SpriteWrapper spriteWrapper);

        (Type Type, string Name) ShowSelectTypeAndNameDialog(Type type, string title);

        Type ShowSelectTypeDialog(Type type, string title);

        void ShowWarningMessageBox(string title, string message);

        MessageBoxResult ShowYesNoCancelMessageBox(string title, string message);

        bool ShowYesNoMessageBox(string title, string message);
    }
}