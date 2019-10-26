namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// A note that slides between two values at a constant rate.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.BaseNote"/>
    public sealed class SlideNote : BaseNote {

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideNote"/> class.
        /// </summary>
        /// <param name="initialFrequency">The initial frequency.</param>
        /// <param name="finalFrequency">The final frequency.</param>
        public SlideNote(Frequency initialFrequency, Frequency finalFrequency) {
            this.InitialFrequency = initialFrequency;
            this.FinalFrequency = finalFrequency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideNote"/> class.
        /// </summary>
        internal SlideNote() {
        }

        /// <summary>
        /// Gets or sets the final frequency.
        /// </summary>
        /// <value>The final frequency.</value>
        [DataMember]
        public Frequency FinalFrequency { get; set; }

        /// <summary>
        /// Gets or sets the initial frequency.
        /// </summary>
        /// <value>The initial frequency.</value>
        [DataMember]
        public Frequency InitialFrequency { get; set; }

        // <inheritdoc/>
        public override float GetFrequency(float percentage) {
            return this.InitialFrequency.Value * (1f - percentage) + this.FinalFrequency.Value * percentage;
        }
    }
}