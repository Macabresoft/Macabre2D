namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia.Threading;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for editing keyboard icon sets.
/// </summary>
public class KeyboardIconSetEditorViewModel : BaseViewModel {
    private readonly IUndoService _undoService;
    private KeyboardIconIndexModel _selectedIcon;
    private SpriteDisplayModel _selectedSprite;
    private ThumbnailSize _selectedThumbnailSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardIconSetEditorViewModel" /> class.
    /// </summary>
    public KeyboardIconSetEditorViewModel() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardIconSetEditorViewModel" /> class.
    /// </summary>
    /// <param name="undoService">The parent undo service.</param>
    /// <param name="iconSet">The icon set being edited.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="file">The content file.</param>
    [InjectionConstructor]
    public KeyboardIconSetEditorViewModel(
        IUndoService undoService,
        KeyboardIconSet iconSet,
        SpriteSheet spriteSheet,
        ContentFile file) : base() {
        this._undoService = undoService;
        this.ClearSpriteCommand = ReactiveCommand.Create(
            this.ClearSprite,
            this.WhenAny(x => x.SelectedIcon, x => x.Value != null));
        this.SelectIconCommand = ReactiveCommand.Create<KeyboardIconIndexModel>(this.SelectIcon);
        this.SpriteCollection = new SpriteDisplayCollection(spriteSheet, file);

        var keys = Enum.GetValues<Keys>().ToList();
        keys.Remove(Keys.None);
        var icons = keys.Select(key => new KeyboardIconIndexModel(iconSet, key)).ToList();
        this.Icons = icons;
        this.SelectedIcon = this.Icons.First();
    }

    /// <summary>
    /// Clears the selected sprite from the selected icon.
    /// </summary>
    public ICommand ClearSpriteCommand { get; }

    /// <summary>
    /// Gets the icons.
    /// </summary>
    public IReadOnlyCollection<KeyboardIconIndexModel> Icons { get; }

    /// <summary>
    /// Gets the select icon command.
    /// </summary>
    public ICommand SelectIconCommand { get; }

    /// <summary>
    /// Gets the sprite collection.
    /// </summary>
    public SpriteDisplayCollection SpriteCollection { get; }

    /// <summary>
    /// Gets or sets the selected icon.
    /// </summary>
    public KeyboardIconIndexModel SelectedIcon {
        get => this._selectedIcon;
        private set {
            if (this._selectedIcon != value) {
                this.RaiseAndSetIfChanged(ref this._selectedIcon, value);
                this._selectedSprite = this._selectedIcon != null ? this.SpriteCollection.FirstOrDefault(x => x.Index == this._selectedIcon.SpriteIndex) : null;
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
            if (this._selectedIcon is { } selectedTile) {
                var previousSprite = this._selectedSprite;
                this._undoService.Do(() =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, value);
                    selectedTile.SpriteIndex = this._selectedSprite?.Index;
                }, () =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, previousSprite);
                    selectedTile.SpriteIndex = this._selectedSprite?.Index;
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
        if (this.SelectedIcon is { SpriteIndex: not null }) {
            this.SelectedSprite = null;
        }
    }

    private void SelectIcon(KeyboardIconIndexModel icon) {
        if (icon != null) {
            this.SelectedIcon = icon;
            // HACK: this makes things work well in the UI, just trust me.
            Dispatcher.UIThread.Post(() => this.RaisePropertyChanged(nameof(this.SelectedIcon)), DispatcherPriority.ApplicationIdle);
        }
    }
}