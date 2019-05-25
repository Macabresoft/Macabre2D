namespace Macabre2D.UI.ServiceInterfaces {

    using System.Windows;

    public interface IMonoGameService {
        DependencyObject EditorGame { get; }

        bool ShowGrid { get; set; }

        bool ShowRotationGizmo { get; set; }

        bool ShowScaleGizmo { get; set; }

        bool ShowSelection { get; set; }

        bool ShowTranslationGizmo { get; set; }

        void CenterCamera();

        void ResetCamera();
    }
}