namespace Macabre2D.UI.ServiceInterfaces {

    using System.Windows;

    public interface IMonoGameService {
        DependencyObject SceneEditor { get; }

        void ResetCamera();
    }
}