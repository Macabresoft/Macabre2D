namespace Macabre2D.Framework {

    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// A song to be played by a synthesizer.
    /// </summary>
    [DataContract]
    public sealed class Song {
        public const ushort MaximumBeatsPerMinute = 1000;
        public const ushort MaximumSampleRate = 44100 / 2;
        public const ushort MinimumBeatsPerMinute = 10;
        public const ushort MinimumSampleRate = 8000;

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        private readonly ObservableCollection<Track> _tracks = new ObservableCollection<Track>();

        private ushort _beatsPerMinute = 120;
        private ushort _sampleRate = MaximumSampleRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Song"/> class.
        /// </summary>
        public Song() {
            this.AddTrack();
        }

        /// <summary>
        /// Gets or sets the beats per minute.
        /// </summary>
        /// <value>The beats per minute.</value>
        [DataMember]
        public ushort BeatsPerMinute {
            get {
                return this._beatsPerMinute;
            }

            set {
                this._beatsPerMinute = value.Clamp(MinimumBeatsPerMinute, MaximumBeatsPerMinute);
                this.BeatsPerSecond = this._beatsPerMinute / 60f;
            }
        }

        /// <summary>
        /// Gets the beats per second.
        /// </summary>
        /// <value>The beats per second.</value>
        public float BeatsPerSecond { get; private set; } = 120f / 60f;

        /// <summary>
        /// Gets the inverse sample rate.
        /// </summary>
        /// <value>The inverse sample rate.</value>
        public float InverseSampleRate { get; private set; } = 1f / MaximumSampleRate;

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public float Length {
            get {
                return this._tracks.Any() ? this._tracks.Max(x => x.Length) : 0f;
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
                this.InverseSampleRate = 1f / this._sampleRate;
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
        /// Converts a length of beats into the equivilant amount of samples.
        /// </summary>
        /// <param name="beats">The beats.</param>
        /// <returns>The number of samples within the beat length.</returns>
        public ulong ConvertBeatsToSamples(float beats) {
            return (ulong)Math.Floor((this.SampleRate * beats) / this.BeatsPerSecond);
        }

        /// <summary>
        /// Converts a set of samples to the length in beats they represent.
        /// </summary>
        /// <param name="numberOfSamples">The number of samples.</param>
        /// <returns>The length of the beat that the number of samples encompass.</returns>
        public float ConvertSamplesToBeats(ushort numberOfSamples) {
            return this.BeatsPerSecond * numberOfSamples * this.InverseSampleRate;
        }

        /// <summary>
        /// Removes the track.
        /// </summary>
        /// <param name="track">The track.</param>
        /// <returns>A value indicating whether or not the track was removed.</returns>
        public bool RemoveTrack(Track track) {
            return this._tracks.Count > 1 && this._tracks.Remove(track);
        }
    }
}