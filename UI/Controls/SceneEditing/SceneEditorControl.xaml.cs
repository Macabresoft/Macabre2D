namespace Macabre2D.UI.Controls.SceneEditing {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class SceneEditorControl : UserControl {

        public SceneEditorControl() {
            this.MonoGameService = ViewContainer.Resolve<IMonoGameService>();
            this.SceneService = ViewContainer.Resolve<ISceneService>();
            this.CenterCameraCommand = new RelayCommand(this.MonoGameService.CenterCamera);
            this.InitializeComponent();
        }

        public ICommand CenterCameraCommand { get; }

        public IMonoGameService MonoGameService { get; }

        public ISceneService SceneService { get; }
    }
}