namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="SpriteRenderer" /> that scrolls the sprite over time.
/// </summary>
public class ScrollingSpriteRenderer : SpriteRenderer, IAnimatableEntity {
    private readonly List<SpriteLocation> _spriteLocations = [];
    private Point _currentOffset;
    private double _millisecondsPassed;

    /// <inheritdoc />
    [DataMember(Order = 11, Name = "Frame Rate")]
    public ByteOverride FrameRateOverride { get; } = new();

    /// <inheritdoc />
    public bool ShouldAnimate => this.IsEnabled;

    /// <summary>
    /// Gets the number of pixels to move per frame.
    /// </summary>
    [DataMember]
    public Point PixelsPerFrame { get; private set; }

    /// <summary>
    /// Get the number of milliseconds in a single frame.
    /// </summary>
    protected int MillisecondsPerFrame { get; private set; }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.FrameRateOverride.PropertyChanged -= this.FrameRateOverride_PropertyChanged;
    }

    /// <inheritdoc />
    public void IncrementTime(FrameTime frameTime) {
        this._millisecondsPassed += frameTime.MillisecondsPassed;

        if (this._millisecondsPassed >= this.MillisecondsPerFrame && this.MillisecondsPerFrame > 0) {
            while (this._millisecondsPassed >= this.MillisecondsPerFrame) {
                this._millisecondsPassed -= this.MillisecondsPerFrame;
                this.NextFrame();
            }
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.ResetFrameRate();
        this.FrameRateOverride.PropertyChanged += this.FrameRateOverride_PropertyChanged;
    }

    /// <inheritdoc />
    public void NextFrame() {
        this._currentOffset -= this.PixelsPerFrame;
        this.ResetSpritePositions();
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (BaseGame.IsDesignMode) {
            base.Render(frameTime, viewBoundingArea, colorOverride);
        }
        else if (this.SpriteSheet is { Content: { } content } && this.SpriteBatch is { } spriteBatch) {
            foreach (var location in this._spriteLocations) {
                spriteBatch.Draw(
                    content,
                    location.Position,
                    location.Rectangle,
                    colorOverride,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    this.RenderOptions.Orientation,
                    0f);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();
        this.RaisePropertyChanged(nameof(this.ShouldAnimate));
    }

    private void FrameRateOverride_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.ResetFrameRate();
    }

    private void ResetFrameRate() {
        if (this.FrameRateOverride is { IsEnabled: true, Value: > 0 }) {
            this.MillisecondsPerFrame = 1000 / this.FrameRateOverride.Value;
        }
        else {
            this.MillisecondsPerFrame = 0;
        }
    }

    private void ResetSpritePositions() {
        this._spriteLocations.Clear();

        if (this.SpriteSheet is { } spriteSheet && this.SpriteIndex is { } spriteIndex) {
            var spriteSize = spriteSheet.SpriteSize;
            var spriteLocation = spriteSheet.GetSpriteLocation(spriteIndex);
            var renderTransform = this.GetRenderTransform() * this.Project.PixelsPerUnit;

            while (this._currentOffset.X > spriteSize.X) {
                this._currentOffset = new Point(this._currentOffset.X - spriteSize.X, this._currentOffset.Y);
            }

            while (this._currentOffset.Y > spriteSize.Y) {
                this._currentOffset = new Point(this._currentOffset.X, this._currentOffset.Y - spriteSize.Y);
            }

            while (this._currentOffset.X < 0) {
                this._currentOffset = new Point(this._currentOffset.X + spriteSize.X, this._currentOffset.Y);
            }

            while (this._currentOffset.Y < 0) {
                this._currentOffset = new Point(this._currentOffset.X, this._currentOffset.Y + spriteSize.Y);
            }

            if (this._currentOffset == Point.Zero) {
                this._spriteLocations.Add(new SpriteLocation(renderTransform, new Rectangle(spriteLocation, spriteSize)));
            }
            else if (this._currentOffset.X == 0) {
                var bottomHeight = spriteSize.Y - this._currentOffset.Y;
                var topHeight = spriteSize.Y - bottomHeight;

                // Bottom
                this._spriteLocations.Add(
                    new SpriteLocation(
                        renderTransform,
                        new Rectangle(spriteLocation.X, spriteLocation.Y, spriteSize.X, bottomHeight)));

                // Top
                this._spriteLocations.Add(
                    new SpriteLocation(
                        new Vector2(renderTransform.X, renderTransform.Y + bottomHeight),
                        new Rectangle(spriteLocation.X, spriteLocation.Y + bottomHeight, spriteSize.X, topHeight)));
            }
            else if (this._currentOffset.Y == 0) {
                var leftWidth = spriteSize.X - this._currentOffset.X;
                var rightWidth = spriteSize.X - leftWidth;

                // Left
                this._spriteLocations.Add(
                    new SpriteLocation(
                        renderTransform,
                        new Rectangle(spriteLocation.X + rightWidth, spriteLocation.Y, leftWidth, spriteSize.Y)));

                // Right
                this._spriteLocations.Add(
                    new SpriteLocation(
                        new Vector2(renderTransform.X + leftWidth, renderTransform.Y),
                        new Rectangle(spriteLocation.X, spriteLocation.Y, rightWidth, spriteSize.Y)));
            }
            else {
                var leftWidth = spriteSize.X - this._currentOffset.X;
                var rightWidth = spriteSize.X - leftWidth;
                var bottomHeight = spriteSize.Y - this._currentOffset.Y;
                var topHeight = spriteSize.Y - bottomHeight;

                // Bottom Left
                this._spriteLocations.Add(
                    new SpriteLocation(
                        renderTransform,
                        new Rectangle(spriteLocation.X + rightWidth, spriteLocation.Y, leftWidth, bottomHeight)));

                // Bottom Right
                this._spriteLocations.Add(
                    new SpriteLocation(
                        new Vector2(renderTransform.X + leftWidth, renderTransform.Y),
                        new Rectangle(spriteLocation.X, spriteLocation.Y, rightWidth, bottomHeight)));

                // Top Left
                this._spriteLocations.Add(
                    new SpriteLocation(
                        new Vector2(renderTransform.X, renderTransform.Y + bottomHeight),
                        new Rectangle(spriteLocation.X + rightWidth, spriteLocation.Y + bottomHeight, leftWidth, topHeight)));

                // Top Right
                this._spriteLocations.Add(
                    new SpriteLocation(
                        new Vector2(renderTransform.X + leftWidth, renderTransform.Y + bottomHeight),
                        new Rectangle(spriteLocation.X, spriteLocation.Y + bottomHeight, rightWidth, topHeight)));
            }
        }
    }


    private readonly record struct SpriteLocation(Vector2 Position, Rectangle Rectangle);
}