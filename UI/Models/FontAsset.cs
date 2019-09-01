namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Resources.Properties;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    public enum FontStyle {
        Regular,

        Bold,

        Italic
    }

    public sealed class FontAsset : AddableAsset<Font> {
        private string _fontName = "Arial";
        private float _size = 12;
        private float _spacing = 0;
        private FontStyle _style = FontStyle.Regular;
        private bool _useKerning = true;

        public override string FileExtension {
            get {
                return FileHelper.SpriteFontExtension;
            }
        }

        [DataMember]
        public Guid FontId { get; private set; } = new Guid();

        public string FontName {
            get {
                return this._fontName;
            }

            set {
                this.Set(ref this._fontName, value);
            }
        }

        public float Size {
            get {
                return this._size;
            }

            set {
                this.Set(ref this._size, value);
            }
        }

        public float Spacing {
            get {
                return this._spacing;
            }

            set {
                if (this.Set(ref this._spacing, value) && this.SavableValue?.SpriteFont != null) {
                    this.SavableValue.SpriteFont.Spacing = this.Spacing;
                }
            }
        }

        public FontStyle Style {
            get {
                return this._style;
            }

            set {
                this.Set(ref this._style, value);
            }
        }

        public override AssetType Type {
            get {
                return AssetType.Font;
            }
        }

        public bool UseKerning {
            get {
                return this._useKerning;
            }

            set {
                this.Set(ref this._useKerning, value);
            }
        }

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder, string projectDirectoryPath) {
            var path = Path.Combine(projectDirectoryPath, this.GetContentPath());
            contentStringBuilder.AppendLine($"#begin {path}");
            contentStringBuilder.AppendLine(@"/importer:FontDescriptionImporter");
            contentStringBuilder.AppendLine(@"/processor:FontDescriptionProcessor");
            contentStringBuilder.AppendLine(@"/processorParam:PremultiplyAlpha = True");
            contentStringBuilder.AppendLine(@"/processorParam:TextureFormat = Compressed");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public override void Delete() {
            this.RemoveIdentifiableContentFromScenes(this.SavableValue.Id);
            base.Delete();
        }

        public override void Refresh(AssetManager assetManager) {
            this.SavableValue.AssetId = this.Id;
            this.SavableValue.Id = this.FontId;
            assetManager.SetMapping(this.Id, this.GetContentPathWithoutExtension());

            const string assetXmlPath = @"/XnaContent/Asset/";
            var spriteFontXml = new XmlDocument();
            spriteFontXml.Load(this.GetPath());
            this._fontName = spriteFontXml.SelectSingleNode($"{assetXmlPath}FontName").InnerText;

            if (float.TryParse(spriteFontXml.SelectSingleNode($"{assetXmlPath}Size").InnerText, out var size)) {
                this._size = size;
            }

            if (float.TryParse(spriteFontXml.SelectSingleNode($"{assetXmlPath}Spacing").InnerText, out var spacing)) {
                this._spacing = spacing;
            }

            if (Enum.TryParse<FontStyle>(spriteFontXml.SelectSingleNode($"{assetXmlPath}Style").InnerText, out var style)) {
                this._style = style;
            }

            if (bool.TryParse(spriteFontXml.SelectSingleNode($"{assetXmlPath}UseKerning").InnerText, out var useKerning)) {
                this._useKerning = useKerning;
            }

            this.RaiseOnRefreshed();
        }

        protected override Font DeserializeSavableValue() {
            return new Font();
        }

        protected override void SaveChanges(Serializer serializer) {
            var defaultSpriteFont = Resources.DefaultSpriteFont;
            defaultSpriteFont = defaultSpriteFont.Replace("<FontName>Arial</FontName>", $"<FontName>{this.FontName}</FontName>");
            defaultSpriteFont = defaultSpriteFont.Replace("<Size>12</Size>", $"<Size>{this.Size}</Size>");
            defaultSpriteFont = defaultSpriteFont.Replace("<Spacing>0</Spacing>", $"<Spacing>{this.Spacing}</Spacing>");
            defaultSpriteFont = defaultSpriteFont.Replace("<UseKerning>true</UseKerning>", $"<UseKerning>{this.UseKerning.ToString().ToLower()}</UseKerning>");
            defaultSpriteFont = defaultSpriteFont.Replace("<Style>Regular</Style>", $"<Style>{this.Style}</Style>");
            File.WriteAllText(this.GetPath(), defaultSpriteFont);
        }
    }
}