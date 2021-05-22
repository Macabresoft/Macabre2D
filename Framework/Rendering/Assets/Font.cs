namespace Macabresoft.Macabre2D.Framework {
    using System.Runtime.Serialization;
    using System.Text;
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
    /// A font to be used by the <see cref="TextRenderer" />.
    /// </summary>
    public sealed class Font : Asset<SpriteFont> {
        private bool _premultiplyAlpha = true;
        private float _size = 12;
        private float _spacing;
        private FontStyle _style = FontStyle.Regular;
        private TextureProcessorOutputFormat _textureFormat = TextureProcessorOutputFormat.Compressed;
        private bool _useKerning = true;

        /// <inheritdoc />
        public override bool IncludeFileExtensionInContentPath => false;
        
        /// <summary>
        /// Gets or sets a value indicating whether or not to premultiply the alpha.
        /// </summary>
        [DataMember]
        public bool PremultiplyAlpha {
            get => this._premultiplyAlpha;

            set => this.Set(ref this._premultiplyAlpha, value);
        }

        /// <summary>
        /// Gets or sets the size of the sprite font.
        /// </summary>
        public float Size {
            get => this._size;

            set => this.Set(ref this._size, value);
        }

        /// <summary>
        /// Gets or sets the spacing for the sprite font.
        /// </summary>
        public float Spacing {
            get => this._spacing;

            set {
                if (this.Set(ref this._spacing, value) && this.Content != null) {
                    this.Content.Spacing = this.Spacing;
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of the sprite font.
        /// </summary>
        public FontStyle Style {
            get => this._style;

            set => this.Set(ref this._style, value);
        }

        /// <summary>
        /// Gets or sets the texture format of the sprite font.
        /// </summary>
        [DataMember]
        public TextureProcessorOutputFormat TextureFormat {
            get => this._textureFormat;

            set => this.Set(ref this._textureFormat, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use kerning for this sprite font.
        /// </summary>
        public bool UseKerning {
            get => this._useKerning;

            set => this.Set(ref this._useKerning, value);
        }

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
    }
}