namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A shader reference with a single parameter.
/// </summary>
public class SingleColorShaderReference : ShaderReference {
    private string _parameterName = string.Empty;
    private Color _parameterValue = Color.Black;

    [DataMember]
    public string ParameterName {
        get => this._parameterName;
        set => this.Set(ref this._parameterName, value);
    }

    /// <summary>
    /// Gets or sets the shadow color for the shader.
    /// </summary>
    [DataMember]
    public Color ParameterValue {
        get => this._parameterValue;
        set => this.Set(ref this._parameterValue, value);
    }

    /// <summary>
    /// Gets the effect with its parameter filled.
    /// </summary>
    /// <returns>The effect.</returns>
    public Effect? GetEffect() {
        var effect = this.Asset?.Content;
        if (effect != null && !string.IsNullOrEmpty(this.ParameterName)) {
            effect.Parameters[this.ParameterName].SetValue(this.ParameterValue.ToVector4());
        }

        return effect;
    }
}