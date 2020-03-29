namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A song to be played by a synthesizer.
    /// </summary>
    [DataContract]
    public sealed class Song {
        public const ushort MaximumSampleRate = 44100 / 2;
        public const ushort MinimumSampleRate = 8000;
        private const ushort MaximumSamplesPerBeat = MaximumSampleRate / 2; // Minimum of 120 beats per minute.
        private const ushort MinimumSamplesPerBeat = (MinimumSampleRate * 60) / 1000; // Maximum of 1000 beats per minute.

        [DataMember]
        private readonly List<Track> _tracks = new List<Track>();

        private ushort _sampleRate = MaximumSampleRate;
        private ushort _samplesPerBeat = MaximumSampleRate / 4;

        /// <summary>
        /// Gets or sets the beats per minute.
        /// </summary>
        /// <value>The beats per minute.</value>
        public double BeatsPerMinute {
            get {
                return (this.SampleRate * 60D) / this.SamplesPerBeat;
            }

            set {
                this.SamplesPerBeat = (ushort)Math.Round((this.SampleRate * 60D) / value);
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length {
            get {
                return this._tracks.Any() ? this._tracks.Max(x => x.Length) : 0;
            }
        }

        /// <summary>
        /// Gets or sets the sample rate.
        /// </summary>
        /// <value>The sample rate.</value>
        [DataMember]
        public ushort SampleRate {
            get {
                return this._sampleRate;
            }

            set {
                this._sampleRate = value.Clamp(MinimumSampleRate, MaximumSampleRate);
            }
        }

        /// <summary>
        /// Gets or sets the samples per beat.
        /// </summary>
        /// <value>The samples per beat.</value>
        [DataMember]
        public ushort SamplesPerBeat {
            get {
                return this._samplesPerBeat;
            }

            set {
                this._samplesPerBeat = value.Clamp(MinimumSamplesPerBeat, MaximumSamplesPerBeat);
            }
        }

        /// <summary>
        /// Gets the total number of samples.
        /// </summary>
        /// <value>The total number of samples.</value>
        public ulong TotalNumberOfSamples {
            get {
                return (ulong)this.Length * this.SamplesPerBeat;
            }
        }

        /// <summary>
        /// Gets the tracks.
        /// </summary>
        /// <value>The tracks.</value>
        public IReadOnlyCollection<Track> Tracks {
            get {
                return this._tracks;
            }
        }

        /// <summary>
        /// Adds the track.
        /// </summary>
        /// <returns>The added track.</returns>
        public Track AddTrack() {
            var track = new Track();
            this._tracks.Add(track);
            return track;
        }

        /// <summary>
        /// Removes the track.
        /// </summary>
        /// <param name="track">The track.</param>
        /// <returns>A value indicating whether or not the track was removed.</returns>
        public bool RemoveTrack(Track track) {
            return this._tracks.Remove(track);
        }
    }
}