namespace Macabre2D.Engine.Windows.ServiceInterfaces {

    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.Models;
    using Macabre2D.Engine.Windows.Models.FrameworkWrappers;
    using System;
    using System.IO;
    using System.Windows;

    public interface IDialogService {

        bool ShowAssetNameChangeDialog(string name, Asset asset, FolderAsset parent, out string newName);

        bool ShowFileBrowser(string filter, out string path, string initialDirectory = null);

        bool ShowFolderBrowser(out string path, string initialDirectory = null);

        bool ShowGenerateSpritesDialog(ImageAsset imageAsset, out (int Columns, int Rows, bool ReplaceExistingSprites) generateSpritesParameters);

        bool ShowSaveAssetAsDialog(Project project, AddableAsset asset);

        SaveDiscardCancelResult ShowSaveDiscardCancelDialog();

        SceneAsset ShowSaveSceneWindow(Project project, Scene scene);

        bool ShowSelectAssetDialog(Project project, Type assetType, bool allowNull, out Asset asset);

        bool ShowSelectProjectDialog(out FileInfo projectFileInfo);

        bool ShowSelectSpriteDialog(SpriteWrapper currentlySelected, out SpriteWrapper spriteWrapper);

        (Type Type, string Name) ShowSelectTypeAndNameDialog(Type type, string title);

        Type ShowSelectTypeDialog(Type type, string title);

        void ShowWarningMessageBox(string title, string message);

        MessageBoxResult ShowYesNoCancelMessageBox(string title, string message);

        bool ShowYesNoMessageBox(string title, string message);
    }
}