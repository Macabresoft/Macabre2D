namespace Macabre2D.UI.ServiceInterfaces {

    using Macabre2D.UI.Common;
    using System.Windows;

    public interface IMonoGameService {
        DependencyObject EditorGame { get; }

        GizmoType SelectedGizmo { get; set; }

        bool ShowGrid { get; set; }

        bool ShowSelection { get; set; }

        void CenterCamera();

        void ResetCamera();
    }
}