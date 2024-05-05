﻿namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a shader configuration.
/// </summary>
public interface IShaderConfiguration {
    /// <summary>
    /// Fills the effect with parameters.
    /// </summary>
    /// <param name="effect">The effect.</param>
    /// <param name="game">The game.</param>
    /// <param name="scene">The scene.</param>
    void FillParameters(Effect effect, IGame game, IScene scene);

    /// <summary>
    /// Resets this configuration to defaults.
    /// </summary>
    void ResetToDefault();
}

/// <summary>
/// A base implementation of <see cref="IShaderConfiguration" />.
/// </summary>
[DataContract]
public class ShaderConfiguration : IShaderConfiguration {

    /// <summary>
    /// Gets an empty instance of <see cref="IShaderConfiguration" />.
    /// </summary>
    public static IShaderConfiguration Empty { get; } = new ShaderConfiguration();

    /// <inheritdoc />
    public virtual void FillParameters(Effect effect, IGame game, IScene scene) {
    }

    /// <inheritdoc />
    public virtual void ResetToDefault() {
    }
}