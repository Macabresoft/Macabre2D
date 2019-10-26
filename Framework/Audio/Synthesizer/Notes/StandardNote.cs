namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// A standard note.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.BaseNote"/>
    public sealed class StandardNote : BaseNote {

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardNote"/> class.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        public StandardNote(Frequency frequency) {
            this.Frequency = frequency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardNote"/> class.
        /// </summary>
        internal StandardNote() {
        }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        [DataMember]
        public Frequency Frequency { get; set; }

        // <inheritdoc/>
        public override float GetFrequency(float percentage) {
            return this.Frequency.Value;
        }
    }
}