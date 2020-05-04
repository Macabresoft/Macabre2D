namespace Macabre2D.UI.GameEditorLibrary.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Common.Extensions;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Services;
    using Macabre2D.UI.GameEditorLibrary.Dialogs;
    using Macabre2D.UI.GameEditorLibrary.Models;
    using Macabre2D.UI.GameEditorLibrary.Models.FrameworkWrappers;
    using System;
    using System.IO;
    using System.Linq;
    using Unity;
    using Unity.Injection;
    using Unity.Resolution;

    public interface IGameDialogService : ICommonDialogService {

        bool ShowAssetNameChangeDialog(string name, Asset asset, FolderAsset parent, out string newName);

        bool ShowGenerateSpritesDialog(ImageAsset imageAsset, out (int Columns, int Rows, bool ReplaceExistingSprites) generateSpritesParameters);

        bool ShowSaveAssetAsDialog(Project project, AddableAsset asset);

        SaveDiscardCancelResult ShowSaveDiscardCancelDialog();

        SceneAsset ShowSaveSceneWindow(Project project, Scene scene);

        bool ShowSelectAssetDialog(Project project, Type assetType, bool allowNull, out Asset asset);

        bool ShowSelectComponentDialog(SceneAsset sceneAsset, Type requestedType, out BaseComponent component);

        bool ShowSelectModuleDialog(Scene scene, Type moduleType, out BaseModule module);

        bool ShowSelectProjectDialog(out FileInfo projectFileInfo);

        bool ShowSelectSpriteDialog(SpriteWrapper currentlySelected, out SpriteWrapper spriteWrapper);
    }

    public sealed class GameDialogService : CommonDialogService, IGameDialogService {

        public GameDialogService(IUnityContainer container) : base(container) {
        }

        public bool ShowAssetNameChangeDialog(string name, Asset asset, FolderAsset parent, out string newName) {
            var wasNameChanged = false;
            newName = name;

            var extension = Path.GetExtension(name) ?? string.Empty;
            var nameWithoutExtension = name;
            if (!string.IsNullOrWhiteSpace(extension)) {
                nameWithoutExtension = Path.GetFileNameWithoutExtension(name);
            }

            var window = this.Container.Resolve<AssetNameChangeDialog>(
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

        public bool ShowGenerateSpritesDialog(ImageAsset imageAsset, out (int Columns, int Rows, bool ReplaceExistingSprites) generateSpritesParameter) {
            if (imageAsset == null) {
                throw new ArgumentNullException(nameof(imageAsset));
            }

            var window = this.Container.Resolve<GenerateSpritesDialog>(new ParameterOverride("imageAsset", imageAsset));
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

            var window = this.Container.Resolve<SaveAssetAsDialog>(new ParameterOverride("project", project));
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

                asset.Save();
            }

            return result;
        }

        public SaveDiscardCancelResult ShowSaveDiscardCancelDialog() {
            var projectService = this.Container.Resolve<IProjectService>();
            var sceneService = this.Container.Resolve<ISceneService>();
            var result = SaveDiscardCancelResult.Save;

            if (projectService.HasChanges || sceneService.CurrentScene.HasChanges) {
                var window = this.Container.Resolve<SaveDiscardCancelDialog>();
                window.ShowDialog();
                result = window.Result;
            }

            return result;
        }

        public SceneAsset ShowSaveSceneWindow(Project project, Scene scene) {
            var window = this.Container.Resolve<SaveSceneDialog>(new ParameterOverride("project", project));
            if (window.SimpleShowDialog()) {
                var fileName = window.ViewModel.FileName + FileHelper.SceneExtension;
                FolderAsset parent;

                if (typeof(FolderAsset).IsAssignableFrom(window.ViewModel.SelectedAsset.GetType()) && window.ViewModel.SelectedAsset is FolderAsset folder) {
                    parent = folder;
                }
                else {
                    parent = window.ViewModel.SelectedAsset.Parent;
                }

                var sceneAsset = new SceneAsset(fileName) {
                    Parent = parent
                };

                scene.SaveToFile(sceneAsset.GetPath());

                if (project.StartUpSceneAsset == null) {
                    project.StartUpSceneAsset = sceneAsset;
                }

                return sceneAsset;
            }

            return null;
        }

        public bool ShowSelectAssetDialog(Project project, Type assetType, bool allowNull, out Asset asset) {
            var window = this.Container.Resolve<SelectAssetDialog>(
                new DependencyOverride(typeof(Project), new InjectionParameter(project)),
                new ParameterOverride("assetType", new InjectionParameter(assetType)),
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

        public bool ShowSelectComponentDialog(SceneAsset sceneAsset, Type requestedType, out BaseComponent component) {
            var window = this.Container.Resolve<SelectComponentDialog>(
                new ParameterOverride("sceneAsset", new InjectionParameter(sceneAsset)),
                new ParameterOverride("requestedType", new InjectionParameter(requestedType)));

            var result = window.SimpleShowDialog();
            component = result ? window.ViewModel.SelectedComponent : null;
            return result;
        }

        public bool ShowSelectModuleDialog(Scene scene, Type moduleType, out BaseModule module) {
            var modules = scene.GetAllModules().Where(x => moduleType.IsAssignableFrom(x.GetType()));
            var window = this.Container.Resolve<SelectModuleDialog>(new ParameterOverride("modules", new InjectionParameter(modules)));

            var result = window.SimpleShowDialog();
            module = result ? window.ViewModel.SelectedModule : null;
            return result;
        }

        public bool ShowSelectProjectDialog(out FileInfo projectFileInfo) {
            var window = this.Container.Resolve<SelectProjectDialog>();

            var result = window.SimpleShowDialog();

            if (result) {
                projectFileInfo = window.ViewModel.SelectedProjectFile;
            }
            else {
                projectFileInfo = null;
            }

            return result;
        }

        public bool ShowSelectSpriteDialog(SpriteWrapper currentlySelected, out SpriteWrapper spriteWrapper) {
            var window = this.Container.Resolve<SelectSpriteDialog>();
            window.ViewModel.SelectedSprite = currentlySelected;
            var result = window.SimpleShowDialog();
            if (result && !(window.ViewModel.SelectedSprite is NullSpriteWrapper)) {
                spriteWrapper = window.ViewModel.SelectedSprite;
            }
            else {
                spriteWrapper = null;
            }

            return result;
        }
    }
}