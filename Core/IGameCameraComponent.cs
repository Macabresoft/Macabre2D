namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.ComponentModel;

    /// <summary>
    /// Interface for a camera component which tells the engine where to render any <see
    /// cref="IGameRenderableComponent" />.
    /// </summary>
    public interface IGameCameraComponent : IEnableable, IBoundable, INotifyPropertyChanged {

        /// <summary>
        /// Gets the layers to render.
        /// </summary>
        /// <value>The layers to render.</value>
        Layers LayersToRender { get; }

        /// <summary>
        /// Gets the render order.
        /// </summary>
        /// <value>The render order.</value>
        int RenderOrder { get; }

        /// <summary>
        /// Gets the state of the sampler.
        /// </summary>
        /// <value>The state of the sampler.</value>
        SamplerState SamplerState { get; }

        /// <summary>
        /// Gets the shader.
        /// </summary>
        /// <value>The shader.</value>
        Shader? Shader { get; }

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <value>The view matrix.</value>
        Matrix ViewMatrix { get; }
    }
}