namespace Macabresoft.MonoGame.Core2D {

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
            var result = type switch
            {
                SamplerStateType.AnisotropicClamp => SamplerState.AnisotropicClamp,
                SamplerStateType.AnisotropicWrap => SamplerState.AnisotropicWrap,
                SamplerStateType.LinearClamp => SamplerState.LinearClamp,
                SamplerStateType.LinearWrap => SamplerState.LinearWrap,
                SamplerStateType.PointWrap => SamplerState.PointWrap,
                _ => SamplerState.PointClamp,
            };
            return result;
        }
    }
}