namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia.Threading;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for editing <see cref="SpriteSheetFont" />.
/// </summary>
public class SpriteSheetFontEditorViewModel : BaseViewModel {
    private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
    private const string Numbers = "0123456789";
    private const string Symbols = ".?!,-=+:";
    private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string WhiteSpace = " ";

    private readonly IUndoService _undoService;
    private SpriteSheetFontIndexModel _selectedCharacter;
    private SpriteDisplayModel _selectedSprite;
    private ThumbnailSize _selectedThumbnailSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoTileSetEditorViewModel" /> class.
    /// </summary>
    public SpriteSheetFontEditorViewModel() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoTileSetEditorViewModel" /> class.
    /// </summary>
    /// <param name="undoService">The parent undo service.</param>
    /// <param name="font">The font being edited.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="file">The content file.</param>
    [InjectionConstructor]
    public SpriteSheetFontEditorViewModel(
        IUndoService undoService,
        SpriteSheetFont font,
        SpriteSheet spriteSheet,
        ContentFile file) : base() {
        this._undoService = undoService;
        this.ClearSpriteCommand = ReactiveCommand.Create(
            this.ClearSprite,
            this.WhenAny(x => x.SelectedCharacter, x => x.Value != null));
        this.SelectCharacterCommand = ReactiveCommand.Create<SpriteSheetFontIndexModel>(this.SelectCharacter);
        this.SpriteCollection = new SpriteDisplayCollection(spriteSheet, file);
        this.Characters = this.CreateCharacterModels(font);
        this.SelectedCharacter = this.Characters.First();
    }

    /// <summary>
    /// Gets the characters.
    /// </summary>
    public IReadOnlyCollection<SpriteSheetFontIndexModel> Characters { get; }


    /// <summary>
    /// Clears the selected sprite from the selected tile.
    /// </summary>
    public ICommand ClearSpriteCommand { get; }

    /// <summary>
    /// Gets the select tile command.
    /// </summary>
    public ICommand SelectCharacterCommand { get; }

    /// <summary>
    /// Gets the sprite collection.
    /// </summary>
    public SpriteDisplayCollection SpriteCollection { get; }

    /// <summary>
    /// Gets or sets the selected tile.
    /// </summary>
    public SpriteSheetFontIndexModel SelectedCharacter {
        get => this._selectedCharacter;
        private set {
            if (this._selectedCharacter != value) {
                this.RaiseAndSetIfChanged(ref this._selectedCharacter, value);
                this._selectedSprite = this._selectedCharacter != null ? this.SpriteCollection.FirstOrDefault(x => x.Index == this._selectedCharacter.SpriteIndex) : null;
                this.RaisePropertyChanged(nameof(this.SelectedSprite));
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected sprite.
    /// </summary>
    public SpriteDisplayModel SelectedSprite {
        get => this._selectedSprite;
        set {
            if (this._selectedCharacter is { } selectedCharacter) {
                var previousSprite = this._selectedSprite;
                this._undoService.Do(() =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, value);
                    selectedCharacter.SpriteIndex = this._selectedSprite?.Index;
                }, () =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, previousSprite);
                    selectedCharacter.SpriteIndex = this._selectedSprite?.Index;
                });
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected thumbnail size.
    /// </summary>
    public ThumbnailSize SelectedThumbnailSize {
        get => this._selectedThumbnailSize;
        set => this.RaiseAndSetIfChanged(ref this._selectedThumbnailSize, value);
    }

    private void ClearSprite() {
        if (this.SelectedCharacter is { SpriteIndex: { } }) {
            this.SelectedSprite = null;
        }
    }

    private IReadOnlyCollection<SpriteSheetFontIndexModel> CreateCharacterModels(SpriteSheetFont font) {
        var characters = new List<SpriteSheetFontIndexModel>();
        foreach (var character in WhiteSpace.Concat(Numbers).Concat(UppercaseLetters).Concat(LowercaseLetters).Concat(Symbols)) {
            characters.Add(new SpriteSheetFontIndexModel(font, character));
        }

        return characters;
    }

    private void SelectCharacter(SpriteSheetFontIndexModel character) {
        if (character != null) {
            this.SelectedCharacter = character;
            // HACK: this makes things work well in the UI, just trust me.
            Dispatcher.UIThread.Post(() => this.RaisePropertyChanged(nameof(this.SelectedCharacter)), DispatcherPriority.ApplicationIdle);
        }
    }
}