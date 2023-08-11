namespace Macabresoft.Macabre2D.UI.Editor;

using Macabresoft.Macabre2D.UI.Common;
using Unity;
using Unity.Extension;
using Unity.Lifetime;

/// <summary>
/// Registers required types to the <see cref="IUnityContainer" />.
/// </summary>
public sealed class EditorContainerExtension : UnityContainerExtension {
    /// <inheritdoc />
    protected override void Initialize() {
        this.Container.RegisterType<ICommonDialogService, LocalDialogService>(new SingletonLifetimeManager());
    }
}