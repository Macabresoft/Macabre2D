namespace Macabre2D.UI.Common;

using System;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;

public class ProjectFontModel : PropertyChangedNotifier {
    private readonly IAssetManager _assetManager;
    private readonly ProjectFonts _fonts;
    private readonly IUndoService _undoService;
    private ProjectFontDefinition _definition;

    public ProjectFontModel(ProjectFontKey key, IAssetManager assetManager, IUndoService undoService, ProjectFonts fonts) {
        this.Key = key;
        this._assetManager = assetManager;
        this._undoService = undoService;
        this._fonts = fonts;

        fonts.TryGetFont(this.Key, out this._definition);
        this.ResetFontName();
    }

    public FontCategory Category => this.Key.Category;

    public ProjectFontDefinition Definition {
        get => this._definition;
        set {
            var currentDefinition = this._definition;
            this._undoService.Do(
                () =>
                {
                    this._definition = value;
                    this._fonts.SetFont(this.Key, this._definition);
                    this.ResetFontName();
                },
                () =>
                {
                    this._definition = currentDefinition;
                    this._fonts.SetFont(this.Key, this._definition);
                    this.ResetFontName();
                });
        }
    }

    public ProjectFontKey Key { get; }

    public string MonoGameFontPath {
        get;
        private set => this.Set(ref field, value);
    } = string.Empty;

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

        if (this._definition.MonoGameFontId != Guid.Empty && this._assetManager.TryGetMetadata(this._definition.MonoGameFontId, out var monoGameFontMetadata)) {
            this.MonoGameFontPath = $"{monoGameFontMetadata.GetContentPath()}{monoGameFontMetadata.ContentFileExtension}";
        }
        else {
            this.MonoGameFontPath = string.Empty;
        }
    }
}