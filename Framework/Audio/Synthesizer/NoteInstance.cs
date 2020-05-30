namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An instance of a note on the piano roll.
    /// </summary>
    [DataContract]
    public sealed class NoteInstance {
        private float _beat;
        private float _length = 1f;
        private float _velocity = 1f;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteInstance"/> class.
        /// </summary>
        /// <param name="beat">The beat.</param>
        /// <param name="length">The length.</param>
        /// <param name="velocity">The velocity.</param>
        /// <param name="frequency">The frequency.</param>
        public NoteInstance(ushort beat, ushort length, float velocity, Frequency frequency) : this(beat, length, velocity, frequency, frequency) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteInstance"/> class.
        /// </summary>
        /// <param name="beat">The beat.</param>
        /// <param name="length">The length.</param>
        /// <param name="velocity">The velocity.</param>
        /// <param name="startFrequency">The start frequency.</param>
        /// <param name="endFrequency">The end frequency.</param>
        public NoteInstance(float beat, float length, float velocity, Frequency startFrequency, Frequency endFrequency) {
            this.Beat = beat;
            this.Length = length;
            this.Velocity = velocity;
            this.StartFrequency = startFrequency;
            this.EndFrequency = endFrequency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteInstance"/> class.
        /// </summary>
        internal NoteInstance() {
        }

        /// <summary>
        /// Gets or sets the beat.
        /// </summary>
        /// <value>The beat.</value>
        [DataMember]
        public float Beat {
            get {
                return this._beat;
            }

            set {
                this._beat = Math.Max(0f, value);
            }
        }

        /// <summary>
        /// Gets or sets the end frequency.
        /// </summary>
        /// <value>The end frequency.</value>
        [DataMember]
        public Frequency EndFrequency { get; set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        [DataMember]
        public float Length {
            get {
                return this._length;
            }

            private set {
                this._length = Math.Max(0.125f, value);
            }
        }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        [DataMember]
        public Frequency StartFrequency { get; set; }

        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        /// <value>The velocity.</value>
        [DataMember]
        public float Velocity {
            get {
                return this._velocity;
            }

            set {
                this._velocity = value.Clamp(0f, 1f);
            }
        }

        /// <summary>
        /// Gets the frequency.
        /// </summary>
        /// <param name="percentageOfNotePlayed">The percentage of note played.</param>
        /// <returns>The frequency.</returns>
        public float GetFrequency(float percentageOfNotePlayed) {
            var result = this.StartFrequency.Value;
            if (this.StartFrequency != this.EndFrequency) {
                result = (this.StartFrequency.Value * (1f - percentageOfNotePlayed)) + (this.EndFrequency.Value * percentageOfNotePlayed);
            }

            return result;
        }
    }
}