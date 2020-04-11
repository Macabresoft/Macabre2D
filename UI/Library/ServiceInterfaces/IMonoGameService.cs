namespace Macabre2D.UI.Library.ServiceInterfaces {

    using Macabre2D.UI.Library.Controls.SceneEditing;
    using Macabre2D.UI.Library.Models.FrameworkWrappers;

    public interface IMonoGameService {
        ComponentEditingStyle EditingStyle { get; set; }

        SceneEditor SceneEditor { get; }

        bool ShowGrid { get; set; }

        bool ShowSelection { get; set; }

        void CenterCamera();

        void ResetCamera();
    }
}