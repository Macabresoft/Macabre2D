namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A single
/// </summary>
[DataContract]
public abstract class RenderStep : PropertyChangedNotifier, IEnableable, IIdentifiable, INameable {
    private bool _isEnabled = true;
    private string _name = "Render Step";

    /// <inheritdoc />
    [DataMember]
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    [DataMember]
    public bool IsEnabled {
        get => this._isEnabled;
        set => this.Set(ref this._isEnabled, value);
    }
    
    /// <inheritdoc />
    [DataMember]
    public string Name {
        get => this._name;
        set => this.Set(ref this._name, value);
    }

    /// <summary>
    /// Gets the calculated render size for this step.
    /// </summary>
    /// <param name="viewPortSize">The view port size.</param>
    /// <param name="pixelRenderSize">The render size in converted pixels.</param>
    /// <returns>The render size.</returns>
    public abstract Point GetRenderSize(Point viewPortSize, Point pixelRenderSize);

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="assets">The asset manager.</param>
    /// <param name="game">The game.</param>
    public abstract void Initialize(IAssetManager assets, IGame game);
}