namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs {
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Avalonia;
    using DynamicData;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Models.Rendering;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for editing sprite animations.
    /// </summary>
    public class SpriteAnimationEditorViewModel : BaseDialogViewModel {
        private const double MaxStepSize = 128;
        private readonly SpriteAnimation _animation;
        private readonly IChildUndoService _childUndoService;
        private readonly IUndoService _parentUndoService;
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
        /// <param name="childUndoService">The child undo service.</param>
        /// <param name="parentUndoService">The parent undo service.</param>
        /// <param name="animation">The animation being edited.</param>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="file">The content file.</param>
        [InjectionConstructor]
        public SpriteAnimationEditorViewModel(
            IChildUndoService childUndoService,
            IUndoService parentUndoService,
            SpriteAnimation animation,
            SpriteSheet spriteSheet,
            ContentFile file) : base(childUndoService) {
            this._childUndoService = childUndoService;
            this._parentUndoService = parentUndoService;
            this._animation = animation;

            this.ClearSpriteCommand = ReactiveCommand.Create(
                this.ClearSprite,
                this.WhenAny(x => x.SelectedStep, x => x.Value != null));

            this.AddCommand = ReactiveCommand.Create<SpriteAnimationStep>(this.AddStep);
            this.InsertCommand = ReactiveCommand.Create<SpriteAnimationStep>(
                this.AddStep,
                this.WhenAny(x => x.SelectedStep, x => x.Value != null));
            this.RemoveCommand = ReactiveCommand.Create<SpriteAnimationStep>(
                this.RemoveStep,
                this.WhenAny(x => x.SelectedStep, x => x.Value != null));


            this.SpriteCollection = new SpriteDisplayCollection(spriteSheet, file);
            this.SelectedStep = this.Steps.FirstOrDefault();
            this.StepSize = this.GetStepSize();
            this.IsOkEnabled = true;
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
                    this.UndoService.Do(() => {
                        this.RaiseAndSetIfChanged(ref this._selectedSprite, value);
                        selectedStep.SpriteIndex = this._selectedSprite?.Index;
                    }, () => {
                        this.RaiseAndSetIfChanged(ref this._selectedSprite, previousSprite);
                        selectedStep.SpriteIndex = this._selectedSprite?.Index;
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

        /// <inheritdoc />
        protected override void OnCancel() {
            var command = this._childUndoService.GetChanges();
            command?.Undo();
            base.OnCancel();
        }

        /// <inheritdoc />
        protected override void OnOk() {
            var command = this._childUndoService.GetChanges();
            this._parentUndoService.CommitExternalChanges(command);
            base.OnOk();
        }

        private void AddStep(SpriteAnimationStep selectedStep) {
            var index = -1;

            if (selectedStep != null && this._animation.Steps.Contains(selectedStep)) {
                index = this._animation.Steps.IndexOf(selectedStep) + 1;
            }

            SpriteAnimationStep step = null;
            this._childUndoService.Do(() => {
                step = this._animation.AddStep(index);
            }, () => {
                if (step != null) {
                    this._animation.RemoveStep(step);
                }
            });
        }

        private void ClearSprite() {
            if (this.SelectedStep is { SpriteIndex: { } }) {
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

        private void RemoveStep(SpriteAnimationStep selectedStep) {
            if (selectedStep != null && this._animation.Steps.Contains(selectedStep)) {
                var index = this._animation.Steps.IndexOf(selectedStep);
                this._childUndoService.Do(() => { this._animation.RemoveStep(selectedStep); }, () => { this._animation.AddStep(selectedStep, index); });
            }
        }
    }
}