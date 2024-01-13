namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Framework;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

/// <summary>
/// Registers services and instances to a <see cref="IUnityContainer" />.
/// </summary>
public sealed class CommonContainerExtension : UnityContainerExtension {
    /// <inheritdoc />
    protected override void Initialize() {
        this.RegisterFrameworkTypes();
        this.RegisterLibraryServices();
        this.RegisterLibraryTypes();
    }

    private void RegisterFrameworkTypes() {
        this.Container.RegisterType<IAssetManager, AssetManager>(new SingletonLifetimeManager())
            .RegisterType<ISerializer, Serializer>(new SingletonLifetimeManager());
    }

    private void RegisterLibraryServices() {
        this.Container.RegisterType<IAssemblyService, AssemblyService>(new SingletonLifetimeManager())
            .RegisterType<IAssetSelectionService, AssetSelectionService>(new SingletonLifetimeManager())
            .RegisterType<IBuildService, BuildService>(new SingletonLifetimeManager())
            .RegisterType<IBusyService, BusyService>(new SingletonLifetimeManager())
            .RegisterType<IContentService, ContentService>(new SingletonLifetimeManager())
            .RegisterType<IEditorService, EditorService>(new SingletonLifetimeManager())
            .RegisterType<IEditorSettingsService, EditorSettingsService>(new SingletonLifetimeManager())
            .RegisterType<IEntityService, EntityService>(new SingletonLifetimeManager())
            .RegisterType<IPathService, PathService>(new SingletonLifetimeManager())
            .RegisterType<IProjectService, ProjectService>(new SingletonLifetimeManager())
            .RegisterType<ISaveService, SaveService>(new SingletonLifetimeManager())
            .RegisterType<ISceneService, SceneService>(new SingletonLifetimeManager())
            .RegisterType<ILoopService, LoopService>(new SingletonLifetimeManager())
            .RegisterType<IValueControlService, ValueControlService>(new SingletonLifetimeManager());
    }

    private void RegisterLibraryTypes() {
        this.Container.RegisterType<IEditorGame, EditorGame>(new SingletonLifetimeManager());
    }
}