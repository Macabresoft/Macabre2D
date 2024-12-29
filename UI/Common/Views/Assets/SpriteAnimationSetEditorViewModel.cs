namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia.Threading;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for editing sprite animation sets.
/// </summary>
public class SpriteAnimationSetEditorViewModel : BaseViewModel {
    private readonly IUndoService _undoService;
    private SpriteAnimationKey _selectedKey;
    private ThumbnailSize _selectedThumbnailSize;
    private SpriteDisplayModel _selectedSprite;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteAnimationSetEditorViewModel" /> class.
    /// </summary>
    public SpriteAnimationSetEditorViewModel() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteAnimationSetEditorViewModel" /> class.
    /// </summary>
    /// <param name="undoService">The parent undo service.</param>
    /// <param name="animationSet">The animation set being edited.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="file">The content file.</param>
    [InjectionConstructor]
    public SpriteAnimationSetEditorViewModel(
        IUndoService undoService,
        SpriteAnimationSet animationSet,
        SpriteSheet spriteSheet,
        ContentFile file) : base() {
        this._undoService = undoService;
        this.SpriteCollection = SpriteDisplayCollection.CreateFromAnimations(spriteSheet, file);

        this.ClearSpriteCommand = ReactiveCommand.Create(
            this.ClearSprite,
            this.WhenAny(x => x.SelectedKey, x => x.Value != null));
        this.SelectKeyCommandCommand = ReactiveCommand.Create<SpriteAnimationKey>(this.SelectKey);

        animationSet.RefreshAnimationKeys();
        this.AnimationKeys = animationSet.Animations;
        this.SelectedKey = this.AnimationKeys.First();
    }
    
    /// <summary>
    /// Gets the sprite collection.
    /// </summary>
    public SpriteDisplayCollection SpriteCollection { get; }

    /// <summary>
    /// Clears the selected sprite from the selected animation key.
    /// </summary>
    public ICommand ClearSpriteCommand { get; }

    /// <summary>
    /// Gets the animation keys.
    /// </summary>
    public IReadOnlyCollection<SpriteAnimationKey> AnimationKeys { get; }

    /// <summary>
    /// Gets the select animation key command.
    /// </summary>
    public ICommand SelectKeyCommandCommand { get; }

    /// <summary>
    /// Gets or sets the selected key.
    /// </summary>
    public SpriteAnimationKey SelectedKey {
        get => this._selectedKey;
        private set {
            if (this._selectedKey != value) {
                this.RaiseAndSetIfChanged(ref this._selectedKey, value);
                this._selectedSprite = this._selectedKey != null ? this.SpriteCollection
                    .FirstOrDefault(x => x.Member?.Id == this._selectedKey.AnimationId) : null;
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
            if (this._selectedKey is { } selectedKey) {
                var previousSprite = this._selectedSprite;
                this._undoService.Do(() =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, value);
                    selectedKey.AnimationId = this._selectedSprite?.Member?.Id ?? Guid.Empty;
                }, () =>
                {
                    this.RaiseAndSetIfChanged(ref this._selectedSprite, previousSprite);
                    selectedKey.AnimationId = this._selectedSprite?.Member?.Id ?? Guid.Empty;
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
        if (this.SelectedKey != null && this.SelectedKey.AnimationId != Guid.Empty) {
            this.SelectedSprite = null;
        }
    }

    private void SelectKey(SpriteAnimationKey spriteAnimationKey) {
        if (spriteAnimationKey != null) {
            this.SelectedKey = spriteAnimationKey;
            // HACK: this makes things work well in the UI, just trust me.
            Dispatcher.UIThread.Post(() => this.RaisePropertyChanged(nameof(this.SelectedKey)), DispatcherPriority.ApplicationIdle);
        }
    }
}