namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Asset reference for <see cref="ShaderAsset" />.
/// </summary>
public class ShaderReference : AssetReference<ShaderAsset, Effect> {
    // TODO: support non-color parameters and multiple parameters
    private string _parameterName = string.Empty;
    private Color _parameterValue;

    /// <summary>
    /// Gets a value indicating whether or not this has a parameter.
    /// </summary>
    public bool HasParameter => !string.IsNullOrEmpty(this.ParameterName);

    /// <summary>
    /// Gets or sets the parameter name.
    /// </summary>
    [DataMember]
    public string ParameterName {
        get => this._parameterName;
        set => this.Set(ref this._parameterName, value);
    }

    /// <summary>
    /// Gets or sets the parameter value.
    /// </summary>
    [DataMember]
    public Color ParameterValue {
        get => this._parameterValue;
        set => this.Set(ref this._parameterValue, value);
    }
}