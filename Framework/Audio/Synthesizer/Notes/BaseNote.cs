namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base note.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.ISynthNote"/>
    [DataContract]
    public abstract class BaseNote : ISynthNote {
        private ulong _length = 1;

        // <inheritdoc/>
        [DataMember]
        public ulong Length {
            get {
                return this._length;
            }

            private set {
                this._length = Math.Max(1, value);
            }
        }

        // <inheritdoc/>
        public abstract float GetFrequency(float percentage);
    }
}