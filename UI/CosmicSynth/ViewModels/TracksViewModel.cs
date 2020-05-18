namespace Macabre2D.UI.CosmicSynth.ViewModels {

    using GalaSoft.MvvmLight.Command;
    using Macabre2D.Framework;
    using Macabre2D.UI.CosmicSynthLibrary.Services;
    using System.Linq;
    using System.Windows.Input;

    public sealed class TracksViewModel : NotifyPropertyChanged {

        public TracksViewModel(ISongService songService) {
            this.SongService = songService;
            this.AddTrackCommand = new RelayCommand(this.AddTrack);
            this.RemoveTrackCommand = new RelayCommand<Track>(
                this.RemoveTrack,
                x => x != null && this.SongService.CurrentSong.Tracks.Count > 1 && this.SongService.CurrentSong.Tracks.Contains(x));
        }

        public ICommand AddTrackCommand { get; }

        public ICommand RemoveTrackCommand { get; }

        public ISongService SongService { get; }

        private void AddTrack() {
            var track = this.SongService.CurrentSong.AddTrack();
            this.SongService.CurrentTrack = track;
        }

        private void RemoveTrack(Track track) {
            this.SongService.CurrentSong.RemoveTrack(track);
            this.SongService.CurrentTrack = this.SongService.CurrentSong.Tracks.First();
        }
    }
}