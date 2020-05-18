namespace Macabre2D.UI.CosmicSynthLibrary.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Services;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ISongService : INotifyPropertyChanged, IChangeDetectionService {
        Song CurrentSong { get; }

        Track CurrentTrack { get; set; }

        Task<Song> CreateSong();

        Task<Song> LoadSong();
    }

    public sealed class SongService : NotifyPropertyChanged, ISongService {
        private readonly ICommonDialogService _dialogService;
        private Song _currentSong = new Song();
        private Track _currentTrack;
        private bool _hasChanges;

        public SongService(ICommonDialogService dialogService) {
            this._dialogService = dialogService;
            this._currentSong = new Song();
            this._currentTrack = this._currentSong.Tracks.First();
        }

        public IReadOnlyCollection<Track> AvailableTracks {
            get {
                return this.CurrentSong.Tracks;
            }
        }

        public Song CurrentSong {
            get {
                return this._currentSong;
            }

            private set {
                if (value != null) {
                    this.Set(ref this._currentSong, value);
                }
            }
        }

        public Track CurrentTrack {
            get {
                if (this._currentTrack == null) {
                    this._currentTrack = this.CurrentSong.Tracks.First();
                }

                return this._currentTrack;
            }

            set {
                if (value != null) {
                    this.Set(ref this._currentTrack, value);
                }
            }
        }

        public bool HasChanges {
            get {
                return this._hasChanges;
            }

            set {
                this.Set(ref this._hasChanges, value);
            }
        }

        public async Task<Song> CreateSong() {
            // TODO: check if current song needs to be saved.
            this.CurrentSong = new Song();
            await Task.CompletedTask;
            this.HasChanges = true;
            return this.CurrentSong;
        }

        public Task<Song> LoadSong() {
            // TODO: use dialog service to load a song.
            throw new System.NotImplementedException();
        }
    }
}