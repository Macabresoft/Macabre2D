namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for common measurements that are used on a per-frame basis that should not be calculated more than necessary.
/// </summary>
public interface ICommonMeasurements {

    /// <summary>
    /// Gets the size of half of a pixel in units.
    /// </summary>
    float HalfPixelInUnits { get; }

    /// <summary>
    /// Gets the ratio of the internal render resolution. This is width / height. So, if your render resolution is 800x600, the value of this will be equal to 800/600 or 4/3.
    /// </summary>
    float InternalRenderResolutionRatio { get; }

    /// <summary>
    /// Gets the pixels per unit. This value is the number of pixels per arbitrary game units.
    /// </summary>
    ushort PixelsPerUnit { get; }

    /// <summary>
    /// Gets the screen pixels per unit. This value is the number of pixels per arbitrary game units.
    /// </summary>
    ushort ScreenPixelsPerUnit { get; }

    /// <summary>
    /// Gets the screen resolution to internal resolution ratio. This is calculated as ViewPort.Y / InternalResolution.Y.
    /// </summary>
    float ScreenResolutionToInternalResolution { get; }

    /// <summary>
    /// Gets the inverse of <see cref="PixelsPerUnit" />.
    /// </summary>
    float UnitsPerPixel { get; }

    /// <summary>
    /// Gets the inverse of <see cref="ScreenPixelsPerUnit" />.
    /// </summary>
    /// <remarks>
    /// This will be calculated when <see cref="ScreenPixelsPerUnit" /> is set.
    /// Multiplication is a quicker operation than division, so if you find yourself dividing by
    /// <see cref="UnitsPerScreenPixel" /> regularly, consider multiplying by this instead as it will
    /// produce the same value, but quicker.
    /// </remarks>
    float UnitsPerScreenPixel { get; }

    /// <summary>
    /// Gets the view height based on render resolution and units per pixel.
    /// </summary>
    float ViewHeight { get; }

    /// <summary>
    /// Deinitializes this instance.
    /// </summary>
    void Deinitialize();

    /// <summary>
    /// Gets the unit length of the number of internal pixels.
    /// </summary>
    /// <param name="numberOfPixels">The number of pixels.</param>
    /// <returns>The length in units.</returns>
    float GetLengthInUnits(int numberOfPixels);

    /// <summary>
    /// Gets a pixel agnostic ratio. This can be used to make something appear the same size on
    /// screen regardless of the current view size.
    /// </summary>
    /// <param name="unitViewHeight">Height of the unit view.</param>
    /// <param name="pixelViewHeight">Height of the pixel view.</param>
    /// <returns>A pixel agnostic ratio.</returns>
    float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight);

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="project">The project.</param>
    void Initialize(IGame game, IGameProject project);
}

/// <summary>
/// Common measurements that are used on a per-frame basis that should not be calculated more than necessary.
/// </summary>
public class CommonMeasurements : ICommonMeasurements {
    private readonly Dictionary<int, float> _pixelsToLength = [];
    private IGame _game = BaseGame.Empty;
    private IGameProject _project = GameProject.Empty;

    /// <inheritdoc />
    public float HalfPixelInUnits { get; private set; }

    /// <inheritdoc />
    public float InternalRenderResolutionRatio { get; private set; }

    /// <inheritdoc />
    public ushort PixelsPerUnit { get; private set; }

    /// <inheritdoc />
    public ushort ScreenPixelsPerUnit { get; private set; }

    /// <inheritdoc />
    public float ScreenResolutionToInternalResolution { get; private set; }

    /// <inheritdoc />
    public float UnitsPerPixel { get; private set; }

    /// <inheritdoc />
    public float UnitsPerScreenPixel { get; private set; }

    /// <inheritdoc />
    public float ViewHeight {
        get;
        private set => field = Math.Max(value, 0.1f); // View height cannot be 0, that would be chaos.
    } = 1f;

    /// <inheritdoc />
    public void Deinitialize() {
        this._game.ViewportSizeChanged -= this.Game_ViewportSizeChanged;
        this._project.InternalSizeChanged -= this.Project_InternalSizeChanged;

        this._game = BaseGame.Empty;
        this._project = GameProject.Empty;
    }

    /// <inheritdoc />
    public float GetLengthInUnits(int numberOfPixels) {
        if (!this._pixelsToLength.TryGetValue(numberOfPixels, out var result)) {
            result = numberOfPixels * this.UnitsPerPixel;
            this._pixelsToLength[numberOfPixels] = result;
        }

        return result;
    }

    /// <inheritdoc />
    public float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight) => unitViewHeight * ((float)this.PixelsPerUnit / pixelViewHeight);

    /// <inheritdoc />
    public void Initialize(IGame game, IGameProject project) {
        this._game = game;
        this._project = project;

        this.ResetInternal();
        this.ResetScreenSpace();

        this._game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        this._project.InternalSizeChanged += this.Project_InternalSizeChanged;
    }


    private void Game_ViewportSizeChanged(object? sender, Point e) {
        this.ResetScreenSpace();
    }

    private void Project_InternalSizeChanged(object? sender, EventArgs e) {
        this.ResetInternal();
    }

    private void ResetInternal() {
        this.PixelsPerUnit = this._project.PixelsPerUnit;

        if (this.PixelsPerUnit > 0) {
            this.UnitsPerPixel = 1f / this.PixelsPerUnit;
        }
        else {
            this.UnitsPerPixel = 1f;
        }

        this.HalfPixelInUnits = this.UnitsPerPixel * 0.5f;
        this.InternalRenderResolutionRatio = this._project.InternalRenderResolution.X / (float)this._project.InternalRenderResolution.Y;
        this.ViewHeight = this._project.InternalRenderResolution.Y * this.UnitsPerPixel;
        this._pixelsToLength.Clear();
    }

    private void ResetScreenSpace() {
        if (!BaseGame.IsDesignMode) {
            this.ScreenPixelsPerUnit = (ushort)Math.Floor(this._game.CroppedViewportSize.Y / this.ViewHeight);
        }
        else {
            this.ScreenPixelsPerUnit = this._project.PixelsPerUnit;
        }

        if (this.ScreenPixelsPerUnit > 0) {
            this.UnitsPerScreenPixel = 1f / this.ScreenPixelsPerUnit;
        }
        else {
            this.UnitsPerScreenPixel = 1f;
        }

        if (!BaseGame.IsDesignMode && this._project.InternalRenderResolution.Y > 0) {
            this.ScreenResolutionToInternalResolution = this._game.ViewportSize.Y / (float)this._project.InternalRenderResolution.Y;
        }
        else {
            this.ScreenResolutionToInternalResolution = 1f;
        }
    }
}