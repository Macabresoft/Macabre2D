namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for editing <see cref="SpriteSheetFont" />.
/// </summary>
public class SpriteSheetFontEditorViewModel : BaseViewModel {
    private const string LowercaseLetters = "";
    private const string Numbers = "0123456789";
    private const string Symbols = ".?!,-=+:";
    private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.?!,-=+: ";
    private const string WhiteSpace = " ";
    private readonly ObservableCollectionExtended<SpriteSheetFontIndexModel> _characters = new();
    private readonly ICommonDialogService _dialogService;
    private readonly SpriteSheetFont _font;
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
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="undoService">The parent undo service.</param>
    /// <param name="font">The font being edited.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="file">The content file.</param>
    [InjectionConstructor]
    public SpriteSheetFontEditorViewModel(
        ICommonDialogService dialogService,
        IUndoService undoService,
        SpriteSheetFont font,
        ContentFile file) : base() {
        this._dialogService = dialogService;
        this._undoService = undoService;
        this._font = font;
        this.AutoLayoutCommand = ReactiveCommand.CreateFromTask(this.PerformAutoLayout);
        this.ClearSpriteCommand = ReactiveCommand.Create(
            this.ClearSprite,
            this.WhenAny(x => x.SelectedCharacter, x => x.Value != null));
        this.SelectCharacterCommand = ReactiveCommand.Create<SpriteSheetFontIndexModel>(this.SelectCharacter);
        this.SpriteCollection = new SpriteDisplayCollection(font.SpriteSheet, file);
        this._characters.Reset(this.CreateCharacterModels());
        this.SelectedCharacter = this.Characters.First();
    }

    /// <summary>
    /// Gets a command to automatically create a new character layout.
    /// </summary>
    public ICommand AutoLayoutCommand { get; }

    /// <summary>
    /// Gets the characters.
    /// </summary>
    public IReadOnlyCollection<SpriteSheetFontIndexModel> Characters => this._characters;

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
                this.RaisePropertyChanged(nameof(this.SelectedKerning));
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected kerning.
    /// </summary>
    public int SelectedKerning {
        get => this._selectedCharacter is { } selectedCharacter ? selectedCharacter.Kerning : 0;
        set {
            if (this._selectedCharacter is { } selectedCharacter && selectedCharacter.Kerning != value) {
                var previousKerning = selectedCharacter.Kerning;
                this._undoService.Do(() =>
                {
                    this._selectedCharacter.Kerning = value;
                    this.RaisePropertyChanged();
                }, () =>
                {
                    this._selectedCharacter.Kerning = previousKerning;
                    this.RaisePropertyChanged();
                });
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
        if (this.SelectedCharacter is { SpriteIndex: not null }) {
            this.SelectedSprite = null;
        }
    }

    private IReadOnlyCollection<SpriteSheetFontIndexModel> CreateCharacterModels() {
        var characters = new List<SpriteSheetFontIndexModel>();
        foreach (var character in this._font.CharacterLayout) {
            characters.Add(new SpriteSheetFontIndexModel(this._font, character));
        }

        return characters;
    }

    private async Task PerformAutoLayout() {
        var result = await this._dialogService.ShowFontLayoutDialog(this._font.CharacterLayout);

        if (result != null) {
            var previousLayout = this._font.CharacterLayout;
            var previousCharacters = this._characters.ToList();

            this._undoService.Do(() =>
            {
                if (result.PerformAutoLayout) {
                    this._font.ClearSprites();

                    if (this._font.SpriteSheet is { } spriteSheet && spriteSheet.Columns * spriteSheet.Rows >= this._font.CharacterLayout.Length) {
                        for (var i = 0; i < this._font.CharacterLayout.Length; i++) {
                            this._font.SetSprite((byte)i, this._font.CharacterLayout[i], 0);
                        }
                    }
                }
                else {
                    this._font.CharacterLayout = result.CharacterLayout;
                }
                
                this._characters.Reset(this.CreateCharacterModels());
            }, () =>
            {
                foreach (var character in this._font.CharacterLayout) {
                    this._font.UnsetSprite(character);
                }

                this._font.CharacterLayout = previousLayout;

                foreach (var character in previousCharacters) {
                    if (character.SpriteIndex.HasValue) {
                        this._font.SetSprite(character.SpriteIndex.Value, character.Character, character.Kerning);
                    }
                }

                this._characters.Reset(previousCharacters);
            });
        }
    }

    private void SelectCharacter(SpriteSheetFontIndexModel character) {
        if (character != null) {
            this.SelectedCharacter = character;
            // HACK: this makes things work well in the UI, just trust me.
            Dispatcher.UIThread.Post(() => this.RaisePropertyChanged(nameof(this.SelectedCharacter)), DispatcherPriority.ApplicationIdle);
        }
    }
}