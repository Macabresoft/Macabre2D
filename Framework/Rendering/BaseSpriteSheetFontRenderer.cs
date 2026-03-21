namespace Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabre2D.Project.Common;

/// <summary>
/// A base renderer for <see cref="SpriteSheetFont" />.
/// </summary>
public abstract class BaseSpriteSheetFontRenderer : RenderableEntity, ITextRenderer {

    private string _resourceText = string.Empty;
    private string _stringFormat = string.Empty;

    /// <inheritdoc />
    [DataMember]
    public FontCategory FontCategory {
        get;
        set {
            if (value != field) {
                field = value;
                this.ReloadFontFromCategory();
                this.RequestRefresh();
            }
        }
    } = FontCategory.None;

    /// <summary>
    /// Gets the font asset reference.
    /// </summary>
    [DataMember]
    public SpriteSheetFontReference FontReference { get; } = new();

    /// <inheritdoc />
    [DataMember]
    public string Format {
        get => this._stringFormat;
        set {
            if (value != this._stringFormat) {
                this._stringFormat = value;
                this.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether color should be ignored when rendering icons.
    /// </summary>
    [DataMember]
    public bool IgnoreColorForIcons { get; set; } = true;

    /// <summary>
    /// Gets or sets the kerning. This is the space between letters in pixels. Positive numbers will increase the space, negative numbers will decrease it.
    /// </summary>
    [DataMember]
    public int Kerning {
        get;
        set {
            if (value != field) {
                field = value;
                this.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the render options.
    /// </summary>
    /// <value>The render options.</value>
    [DataMember(Order = 4)]
    public RenderOptions RenderOptions { get; private set; } = new();

    /// <inheritdoc />
    public override RenderPriority RenderPriority {
        get {
            if (this.RenderPriorityOverride.IsEnabled) {
                return this.RenderPriorityOverride.Value;
            }

            return this.SpriteSheet?.DefaultRenderPriority ?? default;
        }

        set {
            this.RenderPriorityOverride.IsEnabled = true;
            this.RenderPriorityOverride.Value = value;
        }
    }

    /// <summary>
    /// Gets a render priority override.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public RenderPriorityOverride RenderPriorityOverride { get; } = new();

    /// <inheritdoc />
    [ResourceName]
    [DataMember]
    public string ResourceName {
        get;
        set {
            field = value;
            this.ResetResource();
            this.RequestRefresh();
        }
    } = string.Empty;

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    [DataMember]
    public string Text {
        get;
        set {
            if (value != field) {
                field = value;
                this.RequestRefresh();
            }
        }
    } = string.Empty;

    /// <inheritdoc />
    public bool UsesSpriteSheetFont { get; private set; }

    /// <summary>
    /// Gets the font.
    /// </summary>
    protected SpriteSheetFont? Font { get; private set; }

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    protected SpriteSheet? SpriteSheet { get; private set; }

    /// <inheritdoc />
    public virtual string GetFullText() {
        var actualText = string.IsNullOrEmpty(this.ResourceName) ? this.Text : this._resourceText;
        return !string.IsNullOrEmpty(this._stringFormat) ? string.Format(actualText, this._stringFormat) : actualText;
    }

    /// <summary>
    /// Called when the font changes.
    /// </summary>
    protected virtual void OnFontChanged() {
        this.RequestRefresh();
    }

    /// <summary>
    /// Reloads the font from the category.
    /// </summary>
    protected void ReloadFontFromCategory() {
        this.UsesSpriteSheetFont = this.Project.Fonts.TryGetFont(this.FontCategory, this.Game.DisplaySettings.Culture, out var fontDefinition);
        if (this.UsesSpriteSheetFont) {
            this.FontReference.LoadAsset(fontDefinition.SpriteSheetId, fontDefinition.FontId);
        }
    }

    /// <summary>
    /// Requests a refresh of this renderer.
    /// </summary>
    protected abstract void RequestRefresh();

    /// <summary>
    /// Resets the <see cref="Font" /> and <see cref="SpriteSheet" /> properties.
    /// </summary>
    protected void ResetFontAndSpriteSheet() {
        if (this.FontReference is { PackagedAsset: not null, Asset: not null }) {
            this.Font = this.FontReference.PackagedAsset;
            this.SpriteSheet = this.FontReference.Asset;
        }
        else if (this.Project.Fallbacks.Font is { PackagedAsset: not null, Asset: not null }) {
            this.Font = this.Project.Fallbacks.Font.PackagedAsset;
            this.SpriteSheet = this.Project.Fallbacks.Font.Asset;
        }
    }

    /// <summary>
    /// Resets the resource text.
    /// </summary>
    protected void ResetResource() {
        if (!string.IsNullOrEmpty(this.ResourceName)) {
            if (Resources.ResourceManager.TryGetString(this.ResourceName, out var resource)) {
                this._resourceText = resource;
            }
        }
    }
}