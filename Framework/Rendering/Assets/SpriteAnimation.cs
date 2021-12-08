namespace Macabresoft.Macabre2D.Framework;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// An animation that explicitly uses sprites.
/// </summary>
public sealed class SpriteAnimation : SpriteSheetAsset {
    /// <summary>
    /// The default name.
    /// </summary>
    public const string DefaultName = "Animation";

    [DataMember]
    private readonly ObservableCollectionExtended<SpriteAnimationStep> _steps = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteAnimation" /> class.
    /// </summary>
    public SpriteAnimation() {
        if (BaseGame.IsDesignMode) {
            this._steps.CollectionChanged += this.Steps_CollectionChanged;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteAnimation" /> class.
    /// </summary>
    /// <param name="steps">The steps.</param>
    public SpriteAnimation(IEnumerable<SpriteAnimationStep> steps) : this() {
        this._steps.AddRange(steps);
    }

    /// <summary>
    /// Gets the steps.
    /// </summary>
    /// <value>The steps.</value>
    public IReadOnlyCollection<SpriteAnimationStep> Steps => this._steps;

    /// <summary>
    /// Adds the step.
    /// </summary>
    /// <returns>The added step.</returns>
    public SpriteAnimationStep AddStep() {
        var step = new SpriteAnimationStep();
        this._steps.Add(step);
        return step;
    }

    /// <summary>
    /// Adds the step.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The added step.</returns>
    public SpriteAnimationStep AddStep(int index) {
        var step = new SpriteAnimationStep();
        this.AddStep(step, index);
        return step;
    }

    /// <summary>
    /// Adds the step.
    /// </summary>
    /// <param name="step">The step.</param>
    public void AddStep(SpriteAnimationStep step) {
        this._steps.Add(step);
    }

    /// <summary>
    /// Adds the step.
    /// </summary>
    /// <param name="step">The step.</param>
    /// <param name="index">The index.</param>
    public void AddStep(SpriteAnimationStep step, int index) {
        if (index >= this._steps.Count || index < 0) {
            this._steps.Add(step);
        }
        else {
            this._steps.Insert(index, step);
        }
    }

    /// <summary>
    /// Removes the step.
    /// </summary>
    /// <param name="step">The step.</param>
    /// <returns>A value indicating whether or not the step was removed.</returns>
    public bool RemoveStep(SpriteAnimationStep step) {
        return this._steps.Remove(step);
    }

    private void HandleAdd(IEnumerable? newItems) {
        if (newItems != null) {
            foreach (var asset in newItems.OfType<SpriteAnimationStep>()) {
                this.OnAdd(asset);
            }
        }
    }

    private void HandleRemove(IEnumerable? oldItems) {
        if (oldItems != null) {
            foreach (var asset in oldItems.OfType<SpriteAnimationStep>()) {
                this.OnRemove(asset);
            }
        }
    }

    private void OnAdd(SpriteAnimationStep step) {
        if (step is INotifyPropertyChanged notifier) {
            notifier.PropertyChanged += this.Step_PropertyChanged;
        }
    }

    private void OnRemove(SpriteAnimationStep asset) {
        if (asset is INotifyPropertyChanged notifier) {
            notifier.PropertyChanged -= this.Step_PropertyChanged;
        }
    }

    private void Step_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RaisePropertyChanged(nameof(this.Steps));
    }

    private void Steps_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                this.HandleAdd(e.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                this.HandleRemove(e.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace or NotifyCollectionChangedAction.Reset:
                this.HandleRemove(e.OldItems);
                this.HandleAdd(e.NewItems);
                break;
        }

        this.RaisePropertyChanged(nameof(this.Steps));
    }
}