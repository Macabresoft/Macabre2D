namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A shader that wraps around <see cref="Effect" />.
/// </summary>
public sealed class ShaderAsset : Asset<Effect> {
    /// <summary>
    /// The file extension for <see cref="ShaderAsset" />.
    /// </summary>
    public const string FileExtension = ".fx";

    private Type _configurationType = typeof(ShaderConfiguration);

    /// <inheritdoc />
    public override bool IncludeFileExtensionInContentPath => false;

    /// <summary>
    /// Gets or sets the configuration type for this shader, which determines shader parameters.
    /// </summary>
    [DataMember]
    [TypeRestriction(typeof(IShaderConfiguration))]
    public Type ConfigurationType {
        get => this._configurationType;
        set {
            if (value != this._configurationType && value is { IsAbstract: false, IsInterface: false, IsClass: true } && value.GetConstructor(Type.EmptyTypes) != null) {
                this.Set(ref this._configurationType, value);
            }
        }
    }

    /// <inheritdoc />
    public override string GetContentBuildCommands(string contentPath, string fileExtension) {
        var contentStringBuilder = new StringBuilder();
        contentStringBuilder.AppendLine($"#begin {contentPath}");
        contentStringBuilder.AppendLine($@"/importer:{nameof(EffectImporter)}");
        contentStringBuilder.AppendLine($@"/processor:{nameof(EffectProcessor)}");
        contentStringBuilder.AppendLine(@"/processorParam:DebugMode = Auto");
        contentStringBuilder.AppendLine($@"/build:{contentPath}{fileExtension}");
        contentStringBuilder.AppendLine($"#end {contentPath}");
        return contentStringBuilder.ToString();
    }
}