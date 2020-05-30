namespace Macabre2D.Framework {

    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A template to begin a <see cref="Song"/> with.
    /// </summary>
    [DataContract]
    public sealed class Template {
        public const string FileExtension = ".cosmictemplate";

        [DataMember]
        private readonly List<TemplatedTrack> _templatedTracks = new List<TemplatedTrack>();

        [DataMember]
        private ushort _beatsPerMinute;

        [DataMember]
        private ushort _sampleRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class.
        /// </summary>
        /// <param name="song">The song.</param>
        public Template(Song song) {
            this._sampleRate = song.SampleRate;
            this._beatsPerMinute = song.BeatsPerMinute;

            foreach (var track in song.Tracks) {
                this._templatedTracks.Add(new TemplatedTrack(track));
            }
        }

        /// <summary>
        /// Converts this instance to a <see cref="Song"/>.
        /// </summary>
        /// <returns>The song.</returns>
        public Song ToSong() {
            var song = new Song() {
                SampleRate = this._sampleRate,
                BeatsPerMinute = this._beatsPerMinute
            };

            foreach (var templatedTrack in this._templatedTracks) {
                var track = song.AddTrack();
                templatedTrack.ApplyTemplate(track);
            }

            return song;
        }

        [DataContract]
        private sealed class TemplatedTrack {

            [DataMember]
            private Instrument _instrument;

            [DataMember]
            private float _leftChannelVolume;

            [DataMember]
            private float _rightChannelVolume;

            internal TemplatedTrack(Track track) {
                this._instrument = track.Instrument;
                this._leftChannelVolume = track.LeftChannelVolume;
                this._rightChannelVolume = track.RightChannelVolume;
            }

            internal void ApplyTemplate(Track track) {
                track.LeftChannelVolume = this._leftChannelVolume;
                track.RightChannelVolume = this._rightChannelVolume;
                track.Instrument = this._instrument;
            }
        }
    }
}