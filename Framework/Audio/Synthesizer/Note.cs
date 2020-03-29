namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// Notes as letters on the musical scale.
    /// </summary>
    public enum Note : byte {
        C = 0,
        CSharp = 1,
        DFlat = CSharp,
        D = 2,
        DSharp = 3,
        EFlat = DSharp,
        E = 4,
        F = 5,
        FSharp = 6,
        GFlat = FSharp,
        G = 7,
        GSharp = 8,
        AFlat = GSharp,
        A = 9,
        ASharp = 10,
        BFlat = ASharp,
        B = 11
    }

    /// <summary>
    /// Extensions for <see cref="Note"/>.
    /// </summary>
    public static class NoteExtensions {

        private static readonly float[] _notesToFrequency = {
            16.35f,
            17.32f,
            18.35f,
            19.45f,
            20.60f,
            21.83f,
            23.12f,
            24.50f,
            25.96f,
            27.50f,
            29.14f,
            30.87f
        };

        /// <summary>
        /// Determines whether this note is natural (not sharp or flat).
        /// </summary>
        /// <param name="note">The note.</param>
        /// <returns><c>true</c> if the specified note is natural; otherwise, <c>false</c>.</returns>
        public static bool IsNatural(this Note note) {
            var result = false;
            switch (note) {
                case Note.A:
                case Note.B:
                case Note.C:
                case Note.D:
                case Note.E:
                case Note.F:
                case Note.G:
                    result = true;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Converts to frequency.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <param name="pitch">The pitch.</param>
        /// <returns>The frequency at the suggested pitch.</returns>
        public static float ToFrequency(this Note note, Pitch pitch) {
            return (float)Math.Pow(2D, (int)pitch) * _notesToFrequency[(int)note];
        }
    }
}