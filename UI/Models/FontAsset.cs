namespace Macabre2D.UI.Models {

    using Macabre2D.Framework.Rendering;
    using Macabre2D.Framework.Serialization;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Resources.Properties;
    using System;
    using System.IO;
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

        public FontAsset() {
            this.PropertyChanged += this.FontAsset_PropertyChanged;
        }

        public override string FileExtension {
            get {
                return FileHelper.SpriteFontExtension;
            }
        }

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
                this.Set(ref this._spacing, value);
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

        public override void BuildProcessorCommands(StringBuilder contentStringBuilder) {
            var path = this.GetContentPath();
            contentStringBuilder.AppendLine($"#begin {path}");
            contentStringBuilder.AppendLine(@"/importer:FontDescriptionImporter");
            contentStringBuilder.AppendLine(@"/processor:FontDescriptionProcessor");
            contentStringBuilder.AppendLine(@"/processorParam:PremultiplyAlpha = True");
            contentStringBuilder.AppendLine(@"/processorParam:TextureFormat = Compressed");
            contentStringBuilder.AppendLine($@"/build:{path}");
        }

        public override void Refresh() {
            this.SavableValue.ContentPath = Path.ChangeExtension(this.GetContentPath(), null);

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

            if (Guid.TryParse(spriteFontXml.SelectSingleNode($"{assetXmlPath}Id")?.InnerText, out var id)) {
                if (id != Guid.Empty) {
                    this.SavableValue.Id = id;
                }
                else {
                    this.SavableValue.Id = Guid.NewGuid();
                }
            }
            else {
                this.SavableValue.Id = Guid.NewGuid();
            }
        }

        protected override void SaveChanges(Serializer serializer) {
            var defaultSpriteFont = Resources.DefaultSpriteFont;
            defaultSpriteFont = defaultSpriteFont.Replace("<FontName>Arial</FontName>", $"<FontName>{this.FontName}</FontName>");
            defaultSpriteFont = defaultSpriteFont.Replace("<Size>12</Size>", $"<Size>{this.Size}</Size>");
            defaultSpriteFont = defaultSpriteFont.Replace("<Spacing>0</Spacing>", $"<Spacing>{this.Spacing}</Spacing>");
            defaultSpriteFont = defaultSpriteFont.Replace("<UseKerning>true</UseKerning>", $"<UseKerning>{this.UseKerning}</UseKerning>");
            defaultSpriteFont = defaultSpriteFont.Replace("<Style>Regular</Style>", $"<Style>{this.Style}</Style>");
            defaultSpriteFont = defaultSpriteFont.Replace("<Id>00000000-0000-0000-0000-000000000000</Id>", $"<Id>{this.SavableValue.Id}</Id>");
            File.WriteAllText(this.GetPath(), defaultSpriteFont);
        }

        private void FontAsset_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Name)) {
                this.SavableValue.ContentPath = Path.ChangeExtension(this.GetContentPath(), null);
            }
        }
    }
}