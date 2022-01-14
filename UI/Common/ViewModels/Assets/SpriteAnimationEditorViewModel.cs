namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using DynamicData;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for editing sprite animations.
/// </summary>
public class SpriteAnimationEditorViewModel : BaseViewModel {
    private const double MaxStepSize = 64;
    private readonly SpriteAnimation _animation;
    private readonly IUndoService _undoService;
    private bool _isSettingSpriteIndexToNull;
    private SpriteDisplayModel _selectedSprite;
    private SpriteAnimationStep _selectedStep;
    private ThumbnailSize _selectedThumbnailSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteAnimationEditorViewModel" /> class.
    /// </summary>
    public SpriteAnimationEditorViewModel() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteAnimationEditorViewModel" /> class.
    /// </summary>
    /// <param name="undoService">The parent undo service.</param>
    /// <param name="animation">The animation being edited.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="file">The content file.</param>
    [InjectionConstructor]
    public SpriteAnimationEditorViewModel(
        IUndoService undoService,
        SpriteAnimation animation,
        SpriteSheetAsset spriteSheet,
        ContentFile file) : base() {
        this._undoService = undoService;
        this._animation = animation;
        this.MaxIndex = spriteSheet.MaxIndex;

        var selectedNotNull = this.WhenAny(x => x.SelectedStep, x => x.Value != null);
        this.ClearSpriteCommand = ReactiveCommand.Create<SpriteAnimationStep>(this.ClearSprite, selectedNotNull);
        this.AddCommand = ReactiveCommand.Create<SpriteAnimationStep>(this.AddStep);
        this.InsertCommand = ReactiveCommand.Create<SpriteAnimationStep>(this.AddStep, selectedNotNull);
        this.RemoveCommand = ReactiveCommand.Create<SpriteAnimationStep>(this.RemoveStep, selectedNotNull);

        var notAtStart = this.WhenAny(
            x => x.SelectedStep,
            x => !this.IsStepAtStart(x.Value));
        this.MoveUpCommand = ReactiveCommand.Create<SpriteAnimationStep>(this.MoveUp, notAtStart);
        this.MoveToStartCommand = ReactiveCommand.Create<SpriteAnimationStep>(this.MoveToStart, notAtStart);

        var notAtEnd = this.WhenAny(
            x => x.SelectedStep,
            x => !this.IsStepAtEnd(x.Value));
        this.MoveDownCommand = ReactiveCommand.Create<SpriteAnimationStep>(this.MoveDown, notAtEnd);
        this.MoveToEndCommand = ReactiveCommand.Create<SpriteAnimationStep>(this.MoveToEnd, notAtEnd);

        this.SpriteCollection = new SpriteDisplayCollection(spriteSheet, file);
        this.SelectedStep = this.Steps.FirstOrDefault();
        this.StepSize = this.GetStepSize();
    }

    /// <summary>
    /// Gets a command to add a new step.
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Clears the selected sprite from the selected step.
    /// </summary>
    public ICommand ClearSpriteCommand { get; }

    /// <summary>
    /// Gets a command to insert a new step after the selected step.
    /// </summary>
    public ICommand InsertCommand { get; }

    /// <summary>
    /// Gets the max index for the sprite sheet.
    /// </summary>
    public byte MaxIndex { get; }

    /// <summary>
    /// Gets a command to move a step down.
    /// </summary>
    public ICommand MoveDownCommand { get; }

    /// <summary>
    /// Gets a command to move a step to the end of the animation.
    /// </summary>
    public ICommand MoveToEndCommand { get; }

    /// <summary>
    /// Gets a command to move a step to the start of the animation.
    /// </summary>
    public ICommand MoveToStartCommand { get; }

    /// <summary>
    /// Gets a command to move a step up.
    /// </summary>
    public ICommand MoveUpCommand { get; }

    /// <summary>
    /// Gets a command to remove the selected step.
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Gets the sprite collection.
    /// </summary>
    public SpriteDisplayCollection SpriteCollection { get; }

    /// <summary>
    /// Gets the steps.
    /// </summary>
    public IReadOnlyCollection<SpriteAnimationStep> Steps => this._animation.Steps;

    /// <summary>
    /// Gets the size of a step's sprite in pixels.
    /// </summary>
    public Size StepSize { get; }

    /// <summary>
    /// Gets or sets the selected sprite.
    /// </summary>
    public SpriteDisplayModel SelectedSprite {
        get => this._selectedSprite;
        set {
            if (this._selectedStep is SpriteAnimationStep selectedStep) {
                var previousSprite = this._selectedSprite;
                this._undoService.Do(() => {
                    try {
                        this._isSettingSpriteIndexToNull = value == null;
                        this.RaiseAndSetIfChanged(ref this._selectedSprite, value);
                        selectedStep.SpriteIndex = this._selectedSprite?.Index;
                    }
                    finally {
                        this._isSettingSpriteIndexToNull = false;
                    }
                }, () => {
                    try {
                        this._isSettingSpriteIndexToNull = value == null;
                        this.RaiseAndSetIfChanged(ref this._selectedSprite, previousSprite);
                        selectedStep.SpriteIndex = this._selectedSprite?.Index;
                    }
                    finally {
                        this._isSettingSpriteIndexToNull = false;
                    }
                });
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected step.
    /// </summary>
    public SpriteAnimationStep SelectedStep {
        get => this._selectedStep;
        private set {
            if (this._selectedStep != value) {
                this.RaiseAndSetIfChanged(ref this._selectedStep, value);
                this._selectedSprite = this._selectedStep != null ? this.SpriteCollection.FirstOrDefault(x => x.Index == this._selectedStep.SpriteIndex) : null;
                this.RaisePropertyChanged(nameof(this.SelectedSprite));
            }
        }
    }

    /// <summary>
    /// Gets or sets the thumbnail size.
    /// </summary>
    public ThumbnailSize ThumbnailSize {
        get => this._selectedThumbnailSize;
        set => this.RaiseAndSetIfChanged(ref this._selectedThumbnailSize, value);
    }

    /// <summary>
    /// Commits a changed sprite index.
    /// </summary>
    /// <param name="step">The step.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    public void CommitFrames(SpriteAnimationStep step, int oldValue, int newValue) {
        if (step != null && step.Frames != newValue && oldValue != newValue) {
            this._undoService.Do(() => { step.Frames = newValue; }, () => { step.Frames = oldValue; });
        }
    }

    /// <summary>
    /// Commits a changed sprite index.
    /// </summary>
    /// <param name="step">The step.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    public void CommitSpriteIndex(SpriteAnimationStep step, byte? oldValue, byte? newValue) {
        if (step != null && !this._isSettingSpriteIndexToNull && step.SpriteIndex != newValue && oldValue != newValue) {
            this._undoService.Do(() => { step.SpriteIndex = newValue; }, () => { step.SpriteIndex = oldValue; });
        }
    }

    private void AddStep(SpriteAnimationStep selectedStep) {
        var index = -1;

        if (selectedStep != null && this._animation.Steps.Contains(selectedStep)) {
            index = this._animation.Steps.IndexOf(selectedStep) + 1;
        }

        SpriteAnimationStep step = null;
        this._undoService.Do(() => {
            step = this._animation.AddStep(index);
            this.SelectedStep = step;
        }, () => {
            if (step != null) {
                this._animation.RemoveStep(step);
            }

            if (selectedStep != null) {
                this.SelectedStep = selectedStep;
            }
        });
    }

    private void ClearSprite(SpriteAnimationStep step) {
        if (step?.SpriteIndex != null) {
            this.SelectedSprite = null;
        }
    }

    private Size GetStepSize() {
        var size = Size.Empty;
        if (this.SpriteCollection.Sprites.FirstOrDefault() is SpriteDisplayModel { Size: { Width: > 0, Height: > 0 } spriteSize }) {
            if (spriteSize.Width == spriteSize.Height) {
                size = new Size(MaxStepSize, MaxStepSize);
            }
            else if (spriteSize.Width > spriteSize.Height) {
                var ratio = spriteSize.Height / (double)spriteSize.Width;
                size = new Size(MaxStepSize, MaxStepSize * ratio);
            }
            else {
                var ratio = spriteSize.Width / (double)spriteSize.Height;
                size = new Size(MaxStepSize * ratio, MaxStepSize);
            }
        }

        return size;
    }

    private bool IsStepAtEnd(SpriteAnimationStep step) {
        return step == null || !this._animation.Steps.Contains(step) || this._animation.Steps.IndexOf(step) >= this._animation.Steps.Count - 1;
    }

    private bool IsStepAtStart(SpriteAnimationStep step) {
        return step == null || !this._animation.Steps.Contains(step) || this._animation.Steps.IndexOf(step) == 0;
    }

    private void MoveDown(SpriteAnimationStep step) {
        if (!this.IsStepAtEnd(step)) {
            var originalIndex = this._animation.Steps.IndexOf(step);
            var newIndex = originalIndex + 1;
            this._undoService.Do(() => {
                this._animation.RemoveStep(step);
                this._animation.AddStep(step, newIndex);
                this.SelectedStep = step;
            }, () => {
                this._animation.RemoveStep(step);
                this._animation.AddStep(step, originalIndex);
                this.SelectedStep = step;
            });
        }
    }

    private void MoveToEnd(SpriteAnimationStep step) {
        if (!this.IsStepAtEnd(step)) {
            var originalIndex = this._animation.Steps.IndexOf(step);
            this._undoService.Do(() => {
                this._animation.RemoveStep(step);
                this._animation.AddStep(step);
                this.SelectedStep = step;
            }, () => {
                this._animation.RemoveStep(step);
                this._animation.AddStep(step, originalIndex);
                this.SelectedStep = step;
            });
        }
    }

    private void MoveToStart(SpriteAnimationStep step) {
        if (!this.IsStepAtStart(step)) {
            var originalIndex = this._animation.Steps.IndexOf(step);
            this._undoService.Do(() => {
                this._animation.RemoveStep(step);
                this._animation.AddStep(step, 0);
                this.SelectedStep = step;
            }, () => {
                this._animation.RemoveStep(step);
                this._animation.AddStep(step, originalIndex);
                this.SelectedStep = step;
            });
        }
    }

    private void MoveUp(SpriteAnimationStep step) {
        if (!this.IsStepAtStart(step)) {
            var originalIndex = this._animation.Steps.IndexOf(step);
            var newIndex = originalIndex - 1;
            this._undoService.Do(() => {
                this._animation.RemoveStep(step);
                this._animation.AddStep(step, newIndex);
                this.SelectedStep = step;
            }, () => {
                this._animation.RemoveStep(step);
                this._animation.AddStep(step, originalIndex);
                this.SelectedStep = step;
            });
        }
    }

    private void RemoveStep(SpriteAnimationStep selectedStep) {
        if (selectedStep != null && this._animation.Steps.Contains(selectedStep)) {
            var index = this._animation.Steps.IndexOf(selectedStep);
            this._undoService.Do(() => { this._animation.RemoveStep(selectedStep); }, () => {
                this._animation.AddStep(selectedStep, index);
                this.SelectedStep = selectedStep;
            });
        }
    }
}