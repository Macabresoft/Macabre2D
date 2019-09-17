namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Models.FrameworkWrappers;
    using System.Windows;

    public interface IMonoGameService {
        ComponentEditingStyle EditingStyle { get; set; }

        DependencyObject EditorGame { get; }

        bool ShowGrid { get; set; }

        bool ShowSelection { get; set; }

        void CenterCamera();

        void ResetCamera();
    }
}