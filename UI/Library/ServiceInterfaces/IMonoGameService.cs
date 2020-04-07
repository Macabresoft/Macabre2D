namespace Macabre2D.UI.Library.ServiceInterfaces {

    using Macabre2D.UI.Library.Models.FrameworkWrappers;
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