namespace Macabre2D.UI.CosmicSynthLibrary.Controls {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Macabre2D.UI.CommonLibrary.Services;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    public partial class TrackControl : UserControl {

        public static readonly DependencyProperty TrackProperty = DependencyProperty.Register(
            nameof(Track),
            typeof(Track),
            typeof(TrackControl),
            new PropertyMetadata(null, TrackControl.OnTrackChanged));

        private readonly IValueEditorService _valueEditorService = ViewContainer.Resolve<IValueEditorService>();

        public TrackControl() {
            this.InitializeComponent();
        }

        public ObservableRangeCollection<DependencyObject> Editors { get; } = new ObservableRangeCollection<DependencyObject>();

        public Track Track {
            get { return (Track)this.GetValue(TrackProperty); }
            set { this.SetValue(TrackProperty, value); }
        }

        private static async void OnTrackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is TrackControl control) {
                if (e.NewValue is Track) {
                    await control.PopulateEditors();
                }
                else {
                    control.Editors.Clear();
                }
            }
        }

        private async Task PopulateEditors() {
            var editors = await this._valueEditorService.CreateEditors(this.Track, typeof(Track));

            var count = editors.Count;
            for (var i = 0; i < count; i++) {
                if (editors.ElementAtOrDefault(i) is ISeparatedValueEditor currentSeparated) {
                    var previousEditor = editors.ElementAtOrDefault(i);
                    if (previousEditor == null || previousEditor is ISeparatedValueEditor) {
                        currentSeparated.ShowTopSeparator = false;
                    }

                    var nextEditor = editors.ElementAtOrDefault(i + 1);
                    if (nextEditor == null) {
                        currentSeparated.ShowBottomSeparator = false;
                    }
                }
            }

            this.Editors.Reset(editors);
        }
    }
}