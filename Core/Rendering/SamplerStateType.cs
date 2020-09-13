namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An enum to swap between predefined <see cref="SamplerState" />. Exists for serialization.
    /// </summary>
    public enum SamplerStateType {

        /// <summary>
        /// No sampler state.
        /// </summary>
        None,

        /// <summary>
        /// <see cref="SamplerState.AnisotropicClamp" />
        /// </summary>
        AnisotropicClamp,

        /// <summary>
        /// <see cref="SamplerState.AnisotropicWrap" />
        /// </summary>
        AnisotropicWrap,

        /// <summary>
        /// <see cref="SamplerState.LinearClamp" />
        /// </summary>
        LinearClamp,

        /// <summary>
        /// <see cref="SamplerState.LinearWrap" />
        /// </summary>
        LinearWrap,

        /// <summary>
        /// <see cref="SamplerState.PointClamp" />
        /// </summary>
        PointClamp,

        /// <summary>
        /// <see cref="SamplerState.PointWrap" />
        /// </summary>
        PointWrap
    }

    /// <summary>
    /// Extensions for <see cref="SamplerStateType" />.
    /// </summary>
    public static class SamplerStateTypeExtensions {

        /// <summary>
        /// Converts from <see cref="SamplerStateType" /> to <see cref="SamplerState" />.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The sampler state.</returns>
        public static SamplerState ToSamplerState(this SamplerStateType type) {
            SamplerState result;

            switch (type) {
                case SamplerStateType.AnisotropicClamp:
                    result = SamplerState.AnisotropicClamp;
                    break;

                case SamplerStateType.AnisotropicWrap:
                    result = SamplerState.AnisotropicWrap;
                    break;

                case SamplerStateType.LinearClamp:
                    result = SamplerState.LinearClamp;
                    break;

                case SamplerStateType.LinearWrap:
                    result = SamplerState.LinearWrap;
                    break;

                case SamplerStateType.PointWrap:
                    result = SamplerState.PointWrap;
                    break;

                case SamplerStateType.None:
                case SamplerStateType.PointClamp:
                default:
                    result = SamplerState.PointClamp;
                    break;
            }

            return result;
        }
    }
}