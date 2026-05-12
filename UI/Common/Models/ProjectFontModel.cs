namespace Macabre2D.UI.Common;

using System;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;

/// <summary>
/// A UI model for project fonts.
/// </summary>
public class ProjectFontModel : PropertyChangedNotifier {
    private readonly IAssetManager _assetManager;
    private readonly ProjectFonts _fonts;

    private readonly ProjectFontKey _key;
    private readonly IUndoService _undoService;
    private ProjectFontDefinition _definition;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontModel" /> class.
    /// </summary>
    /// <param name="key">The project font unique key.</param>
    /// <param name="assetManager">The asset manager.</param>
    /// <param name="undoService">The under service.</param>
    /// <param name="fonts">The project's font configuration.</param>
    public ProjectFontModel(ProjectFontKey key, IAssetManager assetManager, IUndoService undoService, ProjectFonts fonts) {
        this._key = key;
        this._assetManager = assetManager;
        this._undoService = undoService;
        this._fonts = fonts;

        fonts.TryGetFont(this._key, out this._definition);
        this.ResetFontName();
    }

    /// <summary>
    /// Gets the font category.
    /// </summary>
    public FontCategory Category => this._key.Category;

    /// <summary>
    /// Gets the project font definition.
    /// </summary>
    public ProjectFontDefinition Definition {
        get => this._definition;
        set {
            var currentDefinition = this._definition;
            this._undoService.Do(
                () =>
                {
                    this._definition = value;
                    this._fonts.SetFont(this._key, this._definition);
                    this.ResetFontName();
                },
                () =>
                {
                    this._definition = currentDefinition;
                    this._fonts.SetFont(this._key, this._definition);
                    this.ResetFontName();
                });
        }
    }

    /// <summary>
    /// Gets or sets the legacy font scale.
    /// </summary>
    public float LegacyFontScale {
        get => this._definition.LegacyFontScale;
        set {
            var scale = Math.Clamp(value, 0f, 1f);
            this.Definition = this._definition.WithLegacyFontScale(scale);
        }
    }

    /// <summary>
    /// Gets the path to the MonoGame font within the content directory.
    /// </summary>
    public string MonoGameFontPath {
        get;
        private set => this.Set(ref field, value);
    } = string.Empty;

    /// <summary>
    /// Gets the path to the sprite sheet font within the content directory.
    /// </summary>
    public string SpriteSheetFontPath {
        get;
        private set => this.Set(ref field, value);
    } = string.Empty;

    private void ResetFontName() {
        if (this._definition.FontId != Guid.Empty && this._definition.SpriteSheetId != Guid.Empty && this._assetManager.TryGetMetadata(this.Definition.SpriteSheetId, out var metadata)) {
            this.SpriteSheetFontPath = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
        }
        else {
            this.SpriteSheetFontPath = string.Empty;
        }

        if (this._definition.LegacyFontId != Guid.Empty && this._assetManager.TryGetMetadata(this._definition.LegacyFontId, out var monoGameFontMetadata)) {
            this.MonoGameFontPath = $"{monoGameFontMetadata.GetContentPath()}{monoGameFontMetadata.ContentFileExtension}";
        }
        else {
            this.MonoGameFontPath = string.Empty;
        }
    }
}