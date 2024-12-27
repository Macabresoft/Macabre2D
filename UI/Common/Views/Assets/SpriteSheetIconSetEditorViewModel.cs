﻿namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia.Threading;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for editing sprite sheet icons sets.
/// </summary>
public class SpriteSheetIconSetEditorViewModel : BaseViewModel {
    private readonly IUndoService _undoService;
    private SpriteSheetIcon _selectedIcon;
    private SpriteDisplayModel _selectedSprite;
    private ThumbnailSize _selectedThumbnailSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetIconSetEditorViewModel" /> class.
    /// </summary>
    public SpriteSheetIconSetEditorViewModel() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetIconSetEditorViewModel" /> class.
    /// </summary>
    /// <param name="undoService">The parent undo service.</param>
    /// <param name="iconSet">The icon set being edited.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="file">The content file.</param>
    [InjectionConstructor]
    public SpriteSheetIconSetEditorViewModel(
        IUndoService undoService,
        ISpriteSheetIconSet iconSet,
        SpriteSheet spriteSheet,
        ContentFile file) : base() {
        this._undoService = undoService;
        this.ClearSpriteCommand = ReactiveCommand.Create(
            this.ClearSprite,
            this.WhenAny(x => x.SelectedIcon, x => x.Value != null));
        this.SelectIconCommand = ReactiveCommand.Create<SpriteSheetIcon>(this.SelectIcon);
        this.SpriteCollection = new SpriteDisplayCollection(spriteSheet, file);

        iconSet.RefreshIcons();
        this.Icons = iconSet.Icons;
        this.SelectedIcon = this.Icons.First();
    }

    /// <summary>
    /// Clears the selected sprite from the selected icon.
    /// </summary>
    public ICommand ClearSpriteCommand { get; }

    /// <summary>
    /// Gets the icons.
    /// </summary>
    public IReadOnlyCollection<SpriteSheetIcon> Icons { get; }

    /// <summary>
    /// Gets the select icon command.
    /// </summary>
    public ICommand SelectIconCommand { get; }

    /// <summary>
    /// Gets the sprite collection.
    /// </summary>
    public SpriteDisplayCollection SpriteCollection { get; }

    /// <summary>
    /// Gets or sets the selected tile.
    /// </summary>
    public SpriteSheetIcon SelectedIcon {
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
            if (this._selectedIcon is { } selectedIcon) {
                var previousSprite = this._selectedSprite;
                this._undoService.Do(() =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, value);
                    selectedIcon.SpriteIndex = this._selectedSprite?.Index;
                }, () =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, previousSprite);
                    selectedIcon.SpriteIndex = this._selectedSprite?.Index;
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

    private void SelectIcon(SpriteSheetIcon icon) {
        if (icon != null) {
            this.SelectedIcon = icon;
            // HACK: this makes things work well in the UI, just trust me.
            Dispatcher.UIThread.Post(() => this.RaisePropertyChanged(nameof(this.SelectedIcon)), DispatcherPriority.ApplicationIdle);
        }
    }
}