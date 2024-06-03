namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;

public class ProjectFontModel : PropertyChangedNotifier {
    private readonly IAssetManager _assetManager;
    private readonly ProjectFonts _fonts;
    private readonly IUndoService _undoService;
    private ProjectFontDefinition _definition;
    private string _selectedFontName = string.Empty;

    public ProjectFontModel(ProjectFontKey key, IAssetManager assetManager, IUndoService undoService, ProjectFonts fonts) {
        this.Key = key;
        this._assetManager = assetManager;
        this._undoService = undoService;
        this._fonts = fonts;

        fonts.TryGetFont(this.Key, out this._definition);
        this.ResetFontName();
    }

    public FontCategory Category => this.Key.Category;

    public ProjectFontKey Key { get; }

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

    public string SelectedFontName {
        get => this._selectedFontName;
        set => this.Set(ref this._selectedFontName, value);
    }

    private void ResetFontName() {
        if (this._definition.FontId != Guid.Empty && this._definition.SpriteSheetId != Guid.Empty && this._assetManager.TryGetMetadata(this.Definition.SpriteSheetId, out var metadata)) {
            this.SelectedFontName = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
        }
        else {
            this.SelectedFontName = string.Empty;
        }
    }
}