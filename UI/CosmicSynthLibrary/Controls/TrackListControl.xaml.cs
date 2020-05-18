namespace Macabre2D.UI.CosmicSynthLibrary.Controls {

    using Macabre2D.Framework;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class TrackListControl : UserControl {

        public static readonly DependencyProperty AddCommandProperty = DependencyProperty.Register(
            nameof(AddCommand),
            typeof(ICommand),
            typeof(TrackListControl),
            new PropertyMetadata());

        public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register(
            nameof(RemoveCommand),
            typeof(ICommand),
            typeof(TrackListControl),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectedTrackProperty = DependencyProperty.Register(
            nameof(SelectedTrack),
            typeof(Track),
            typeof(TrackListControl),
            new PropertyMetadata());

        public static readonly DependencyProperty TracksProperty = DependencyProperty.Register(
            nameof(Tracks),
            typeof(IReadOnlyCollection<Track>),
            typeof(TrackListControl),
            new PropertyMetadata(new List<Track>()));

        public TrackListControl() {
            this.InitializeComponent();
        }

        public ICommand AddCommand {
            get { return (ICommand)this.GetValue(AddCommandProperty); }
            set { this.SetValue(AddCommandProperty, value); }
        }

        public ICommand RemoveCommand {
            get { return (ICommand)this.GetValue(RemoveCommandProperty); }
            set { this.SetValue(RemoveCommandProperty, value); }
        }

        public Track SelectedTrack {
            get { return (Track)this.GetValue(SelectedTrackProperty); }
            set { this.SetValue(SelectedTrackProperty, value); }
        }

        public IReadOnlyCollection<Track> Tracks {
            get { return (IReadOnlyCollection<Track>)this.GetValue(TracksProperty); }
            set { this.SetValue(TracksProperty, value); }
        }
    }
}