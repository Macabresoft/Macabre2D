namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Asset reference for <see cref="ShaderAsset" />.
/// </summary>
[Category(CommonCategories.Shader)]
public class ShaderReference : AssetReference<ShaderAsset, Effect> {
    /// <summary>
    /// Gets the configuration for this shader.
    /// </summary>
    [DataMember]
    public IShaderConfiguration Configuration { get; private set; } = ShaderConfiguration.Empty;

    /// <inheritdoc />
    public override void LoadAsset(ShaderAsset asset) {
        base.LoadAsset(asset);
        this.ResetConfigurationType();
    }

    /// <summary>
    /// Prepares a shader and returns it.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="scene">The scene.</param>
    /// <returns>The shader.</returns>
    public Effect? PrepareAndGetShader(IGame game, IScene scene) {
        var effect = this.Asset?.Content;
        if (effect != null) {
            this.Configuration.FillParameters(effect, game, scene);
        }

        return effect;
    }

    /// <inheritdoc />
    protected override void OnAssetPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnAssetPropertyChanged(sender, e);

        if (e.PropertyName == nameof(ShaderAsset.ConfigurationType)) {
            this.ResetConfigurationType();
        }
    }

    private void ResetConfigurationType() {
        var newConfiguration = ShaderConfiguration.Empty;
        if (this.Asset != null) {
            if (this.Asset.ConfigurationType == this.Configuration.GetType()) {
                newConfiguration = this.Configuration;
            }
            else if (Activator.CreateInstance(this.Asset.ConfigurationType) is IShaderConfiguration configuration) {
                newConfiguration = configuration;
            }
        }

        this.Configuration = newConfiguration;
    }
}