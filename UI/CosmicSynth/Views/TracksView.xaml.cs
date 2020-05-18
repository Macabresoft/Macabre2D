namespace Macabre2D.UI.CosmicSynth.Views {

    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CosmicSynth.ViewModels;
    using System.Windows.Controls;

    public partial class TracksView : UserControl {

        public TracksView() {
            this.DataContext = ViewContainer.Resolve<TracksViewModel>();
            this.InitializeComponent();
        }
    }
}