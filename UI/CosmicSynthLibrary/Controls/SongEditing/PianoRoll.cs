namespace Macabre2D.UI.CosmicSynthLibrary.Controls.SongEditing {

    using Macabre2D.Framework;
    using System.Linq;

    public interface IPianoRoll {
        Song Song { get; }

        Track Track { get; }

        float GetRowPosition(Note note, Pitch pitch);
    }

    public sealed class PianoRoll : IPianoRoll {
        public const string SpriteSheetPath = "PianoRollSpriteSheet";
        private Song _song;
        private Track _track;

        public Song Song {
            get {
                if (this._song == null) {
                    this._song = new Song();
                }

                return this._song;
            }

            set {
                this._song = value;
            }
        }

        public Track Track {
            get {
                if (this._track == null) {
                    this._track = this.Song.Tracks.First();
                }

                return this._track;
            }

            set {
                this._track = value;
            }
        }

        public float GetRowPosition(Note note, Pitch pitch) {
            return 12f * GetPitchMultiplier(pitch) + (byte)note;
        }

        private static float GetPitchMultiplier(Pitch pitch) {
            var result = 0f;
            if (pitch == Pitch.Normal) {
                result = 1f;
            }
            else if (pitch == Pitch.High) {
                result = 2f;
            }

            return result;
        }
    }
}