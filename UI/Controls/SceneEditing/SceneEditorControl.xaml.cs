namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Windows.Controls;

    public partial class SceneEditorControl : UserControl {

        public SceneEditorControl() {
            this.MonoGameService = ViewContainer.Resolve<IMonoGameService>();
            this.InitializeComponent();
        }

        public IMonoGameService MonoGameService { get; }
    }
}