namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Common.Extensions;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Macabre2D.UI.Views.Dialogs;
    using Microsoft.Win32;
    using System;
    using System.IO;
    using System.Windows;
    using Unity;
    using Unity.Injection;
    using Unity.Resolution;

    public sealed class DialogService : IDialogService {
        private readonly IUnityContainer _container;
        private readonly Serializer _serializer;

        public DialogService(IUnityContainer container, Serializer serializer) {
            this._container = container;
            this._serializer = serializer;
        }

        public bool ShowAssetNameChangeDialog(string name, Asset asset, FolderAsset parent, out string newName) {
            var wasNameChanged = false;
            newName = name;

            var extension = Path.GetExtension(name) ?? string.Empty;
            var nameWithoutExtension = name;
            if (!string.IsNullOrWhiteSpace(extension)) {
                nameWithoutExtension = Path.GetFileNameWithoutExtension(name);
            }

            var window = this._container.Resolve<AssetNameChangeDialog>(
                new ParameterOverride("name", nameWithoutExtension),
                new ParameterOverride("extension", extension),
                new ParameterOverride("asset", asset),
                new ParameterOverride("parent", parent));

            if (window.SimpleShowDialog()) {
                newName = $"{window.ViewModel.NewName}{extension}";
                wasNameChanged = true;
            }

            return wasNameChanged;
        }

        public bool ShowFileBrowser(string filter, out string path, string initialDirectory = null) {
            var dialog = new OpenFileDialog() {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = filter,
                InitialDirectory = initialDirectory
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value) {
                path = dialog.FileName;
                return true;
            }

            path = string.Empty;
            return false;
        }

        public bool ShowFolderBrowser(out string path, string initialDirectory = null) {
            var dialog = new System.Windows.Forms.FolderBrowserDialog() {
                SelectedPath = initialDirectory
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                path = dialog.SelectedPath;
                return true;
            }

            path = string.Empty;
            return false;
        }

        public bool ShowGenerateSpritesDialog(ImageAsset imageAsset, out (int Columns, int Rows, bool ReplaceExistingSprites) generateSpritesParameter) {
            if (imageAsset == null) {
                throw new ArgumentNullException(nameof(imageAsset));
            }

            var window = this._container.Resolve<GenerateSpritesDialog>(new ParameterOverride("imageAsset", imageAsset));
            var result = window.SimpleShowDialog();

            if (result) {
                var viewModel = window.ViewModel;
                generateSpritesParameter = (viewModel.NumberOfColumns, viewModel.NumberOfRows, viewModel.ReplaceExistingSprites);
            }
            else {
                generateSpritesParameter = (0, 0, false);
            }

            return result;
        }

        public bool ShowSaveAssetAsDialog(Project project, AddableAsset asset) {
            if (asset == null) {
                throw new ArgumentNullException(nameof(asset));
            }

            if (project == null) {
                throw new ArgumentNullException(nameof(project));
            }

            var window = this._container.Resolve<SaveAssetAsDialog>(new ParameterOverride("project", project));
            window.ViewModel.Name = asset.NameWithoutExtension;

            var result = window.SimpleShowDialog();
            if (result) {
                asset.Name = window.ViewModel.Name.EndsWith(asset.FileExtension) ? window.ViewModel.Name : $"{window.ViewModel.Name}{asset.FileExtension}";
                if (window.ViewModel.SelectedAsset is FolderAsset folder) {
                    asset.Parent = folder;
                }
                else {
                    asset.Parent = project.AssetFolder;
                }

                asset.Save(this._serializer, project.AssetManager);
            }

            return result;
        }

        public SaveDiscardCancelResult ShowSaveDiscardCancelDialog() {
            var projectService = this._container.Resolve<IProjectService>();
            var sceneService = this._container.Resolve<ISceneService>();
            var result = SaveDiscardCancelResult.Save;

            if (projectService.HasChanges || sceneService.HasChanges) {
                var window = this._container.Resolve<SaveDiscardCancelDialog>();
                window.ShowDialog();
                result = window.Result;
            }

            return result;
        }

        public SceneAsset ShowSaveSceneWindow(Project project, Scene scene) {
            var window = this._container.Resolve<SaveSceneDialog>(new ParameterOverride("project", project));
            if (window.SimpleShowDialog()) {
                var fileName = window.ViewModel.FileName + FileHelper.SceneExtension;
                FolderAsset parent;

                if (window.ViewModel.SelectedAsset.Type == AssetType.Folder && window.ViewModel.SelectedAsset is FolderAsset folder) {
                    parent = folder;
                }
                else {
                    parent = window.ViewModel.SelectedAsset.Parent;
                }

                var sceneAsset = new SceneAsset(fileName) {
                    Parent = parent
                };

                scene.SaveToFile(sceneAsset.GetPath(), this._serializer);

                if (project.StartUpSceneAsset == null) {
                    project.StartUpSceneAsset = sceneAsset;
                }

                return sceneAsset;
            }

            return null;
        }

        public bool ShowSelectAssetDialog(Project project, AssetType assetMask, AssetType selectableAssetMask, bool allowNull, out Asset asset) {
            var window = this._container.Resolve<SelectAssetDialog>(
                new DependencyOverride(typeof(Project), new InjectionParameter(project)),
                new ParameterOverride("assetMask", new InjectionParameter(assetMask)),
                new ParameterOverride("selectableAssetMask", new InjectionParameter(selectableAssetMask)),
                new ParameterOverride("allowNull", new InjectionParameter(allowNull)));

            var result = window.SimpleShowDialog();
            if (result) {
                asset = window.ViewModel.SelectedAsset;
            }
            else {
                asset = null;
            }

            return result;
        }

        public bool ShowSelectSpriteDialog(out SpriteWrapper spriteWrapper) {
            var window = this._container.Resolve<SelectSpriteDialog>();
            var result = window.SimpleShowDialog();
            if (result && !(window.ViewModel.SelectedSprite is NullSpriteWrapper)) {
                spriteWrapper = window.ViewModel.SelectedSprite;
            }
            else {
                spriteWrapper = null;
            }

            return result;
        }

        public (Type Type, string Name) ShowSelectTypeAndNameDialog(Type type, string title) {
            var window = this._container.Resolve<SelectTypeDialog>(new DependencyOverride(typeof(Type), new InjectionParameter(type)));
            window.Title = title;
            window.ShowNameTextBox = true;

            if (window.SimpleShowDialog() && window.ViewModel != null) {
                return (window.ViewModel.SelectedType, window.ViewModel.Name);
            }

            return (null, null);
        }

        public Type ShowSelectTypeDialog(Type type, string title) {
            var window = this._container.Resolve<SelectTypeDialog>(new DependencyOverride(typeof(Type), new InjectionParameter(type)));
            window.Title = title;
            return window.SimpleShowDialog() ? window.ViewModel?.SelectedType : null;
        }

        public void ShowWarningMessageBox(string title, string message) {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public MessageBoxResult ShowYesNoCancelMessageBox(string title, string message) {
            return MessageBox.Show(message, title, MessageBoxButton.YesNoCancel);
        }

        public bool ShowYesNoMessageBox(string title, string message) {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
    }
}