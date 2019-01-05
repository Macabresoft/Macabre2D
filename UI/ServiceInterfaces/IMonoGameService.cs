namespace Macabre2D.UI.ServiceInterfaces {

    using System.Windows;

    public interface IMonoGameService {
        DependencyObject EditorGame { get; }

        void ResetCamera();
    }
}