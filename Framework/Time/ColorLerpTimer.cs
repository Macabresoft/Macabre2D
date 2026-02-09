namespace Macabre2D.Framework;

using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="GameTimer" /> that handles lerping between two colors over time.
/// </summary>
public class ColorLerpTimer : GameTimer {
    private readonly ResettableLazy<Color> _currentColor;
    private Color _endColor;
    private float _lerpMultiplier;
    private Color _startColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorLerpTimer" /> class.
    /// </summary>
    public ColorLerpTimer() : base() {
        this._currentColor = new ResettableLazy<Color>(this.GetCurrentColor);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorLerpTimer" /> class.
    /// </summary>
    /// <param name="timeLimit">The time limit.</param>
    public ColorLerpTimer(float timeLimit) : this() {
        this.TimeLimit = timeLimit;
    }

    /// <summary>
    /// Gets the current color.
    /// </summary>
    public Color CurrentColor => this._currentColor.Value;

    /// <summary>
    /// Gets the end color of the lerp. This will be the current color when percent complete is 1.
    /// </summary>
    [DataMember]
    public Color EndColor {
        get => this._endColor;
        set {
            this._endColor = value;
            this._currentColor.Reset();
        }
    }

    /// <summary>
    /// Gets the start color of the lerp. This will be the current color when percent complete is 0.
    /// </summary>
    [DataMember]
    public Color StartColor {
        get => this._startColor;
        set {
            this._startColor = value;
            this._currentColor.Reset();
        }
    }

    /// <inheritdoc />
    protected override void OnPercentCompleteChanged() {
        base.OnPercentCompleteChanged();
        this._currentColor.Reset();
    }

    private Color GetCurrentColor() => Color.Lerp(this.StartColor, this.EndColor, this.PercentComplete);
}