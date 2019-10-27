namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the frequency.
    /// </summary>
    [DataContract]
    public struct Frequency {

        /// <summary>
        /// The note.
        /// </summary>
        [DataMember]
        public readonly MusicalScale Note;

        /// <summary>
        /// The pitch.
        /// </summary>
        [DataMember]
        public readonly MusicalPitch Pitch;

        /// <summary>
        /// The frequency value.
        /// </summary>
        [DataMember]
        public readonly float Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Frequency"/> struct.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <param name="pitch">The pitch.</param>
        public Frequency(MusicalScale note, MusicalPitch pitch) {
            this.Note = note;
            this.Pitch = pitch;
            this.Value = this.Note.ToFrequency(this.Pitch);
        }

        /// <inheritdoc/>
        public static bool operator !=(Frequency left, Frequency right) {
            return !(left == right);
        }

        /// <inheritdoc/>
        public static bool operator ==(Frequency left, Frequency right) {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) {
            return obj is Frequency frequency && this.Note == frequency.Note && this.Pitch == frequency.Pitch;
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            return (int)Math.Round(this.Value * 10f);
        }
    }
}