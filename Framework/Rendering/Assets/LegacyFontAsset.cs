namespace Macabre2D.Framework;

using System.Runtime.Serialization;
using System.Text;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// The font style.
/// </summary>
public enum FontStyle {
    Regular,
    Bold,
    Italic
}

/// <summary>
/// A font to be used by the <see cref="LegacyTextLineRenderer" />.
/// </summary>
public sealed class LegacyFontAsset : Asset<SpriteFont> {

    /// <summary>
    /// The file extension for a serialized <see cref="SpriteFont" />.
    /// </summary>
    public const string FileExtension = ".spritefont";

    private readonly ResettableLazy<Vector2> _spaceSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyFontAsset" /> class.
    /// </summary>
    public LegacyFontAsset() : base() {
        this._spaceSize = new ResettableLazy<Vector2>(this.GetSpaceSize);
    }

    /// <summary>
    /// Gets the character height. This is represented in screen pixels.
    /// </summary>
    public float CharacterPixelHeight => this._spaceSize.Value.Y;

    /// <inheritdoc />
    public override bool IncludeFileExtensionInContentPath => false;

    /// <summary>
    /// Gets the width of the space character. This is represented in screen pixels.
    /// </summary>
    public float SpacePixelWidth => this._spaceSize.Value.X;

    /// <summary>
    /// Gets or sets a value indicating whether to premultiply the alpha.
    /// </summary>
    [DataMember]
    public bool PremultiplyAlpha {
        get;
        set => this.Set(ref field, value);
    } = true;

    /// <summary>
    /// Gets or sets the size of the sprite font.
    /// </summary>
    public float Size {
        get;
        set => this.Set(ref field, value);
    } = 12;

    /// <summary>
    /// Gets or sets the spacing for the sprite font.
    /// </summary>
    public float Spacing {
        get;
        set {
            if (this.Set(ref field, value) && this.Content != null) {
                this.Content.Spacing = this.Spacing;
            }
        }
    }

    /// <summary>
    /// Gets or sets the style of the sprite font.
    /// </summary>
    public FontStyle Style {
        get;
        set => this.Set(ref field, value);
    } = FontStyle.Regular;

    /// <summary>
    /// Gets or sets the texture format of the sprite font.
    /// </summary>
    [DataMember]
    public TextureProcessorOutputFormat TextureFormat {
        get;
        set => this.Set(ref field, value);
    } = TextureProcessorOutputFormat.Compressed;

    /// <summary>
    /// Gets or sets a value indicating whether to use kerning for this sprite font.
    /// </summary>
    public bool UseKerning {
        get;
        set => this.Set(ref field, value);
    } = true;

    /// <inheritdoc />
    public override string GetContentBuildCommands(string contentPath, string fileExtension) {
        var contentStringBuilder = new StringBuilder();
        contentStringBuilder.AppendLine($"#begin {contentPath}");
        contentStringBuilder.AppendLine(@"/importer:FontDescriptionImporter");
        contentStringBuilder.AppendLine(@"/processor:FontDescriptionProcessor");
        contentStringBuilder.AppendLine($@"/processorParam:PremultiplyAlpha = {this.PremultiplyAlpha}");
        contentStringBuilder.AppendLine($@"/processorParam:TextureFormat = {this.TextureFormat.ToString()}");
        contentStringBuilder.AppendLine($@"/build:{contentPath}{fileExtension}");
        contentStringBuilder.AppendLine($"#end {contentPath}");
        return contentStringBuilder.ToString();
    }

    /// <inheritdoc />
    public override void LoadContent(SpriteFont content) {
        base.LoadContent(content);
        this._spaceSize.Reset();
    }

    private Vector2 GetSpaceSize() {
        var result = Vector2.Zero;
        if (this.Content is { } font) {
            result = font.MeasureString(" ");
        }

        return result;
    }
}