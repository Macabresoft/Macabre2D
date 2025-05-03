namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

/// <summary>
/// Settings for colors that the user can specify.
/// </summary>
[DataContract]
public class ColorSettings {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<RenderPriority, Color> _renderPriorityToColor = new();

    /// <summary>
    /// Called when a color changes for a specific <see cref="RenderPriority" />.
    /// </summary>
    public event EventHandler<RenderPriority>? ColorChanged;

    /// <summary>
    /// Clears color overrides for all render priorities.
    /// </summary>
    public void Clear() {
        this._renderPriorityToColor.Clear();
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public ColorSettings Clone() {
        var settings = new ColorSettings();
        this.CopyTo(settings);
        return settings;
    }

    /// <summary>
    /// Copies color settings to another instance.
    /// </summary>
    /// <param name="other">The other instance.</param>
    public void CopyTo(ColorSettings other) {
        other._renderPriorityToColor.Clear();
        foreach (var entry in this._renderPriorityToColor) {
            other._renderPriorityToColor[entry.Key] = entry.Value;
        }
    }

    /// <summary>
    /// Enables color override for the specified render priority.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    public void EnableRenderPriorityColor(RenderPriority renderPriority) {
        if (!this.TryGetColorForRenderPriority(renderPriority, out _)) {
            this.SetRenderPriorityColor(renderPriority, Color.White);
        }
    }

    /// <summary>
    /// Removes color override for the specified render priority.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    public void RemoveRenderPriorityColor(RenderPriority renderPriority) {
        this._renderPriorityToColor.Remove(renderPriority);
        this.ColorChanged.SafeInvoke(this, renderPriority);
    }

    /// <summary>
    /// Sets the color for a specified <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <param name="color">The color.</param>
    public void SetRenderPriorityColor(RenderPriority renderPriority, Color color) {
        this._renderPriorityToColor[renderPriority] = color;
        this.ColorChanged.SafeInvoke(this, renderPriority);
    }

    /// <summary>
    /// Tries to get the color for the specified <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <param name="color">The found color.</param>
    /// <returns>A value indicating whether this render priority has a specified color.</returns>
    public bool TryGetColorForRenderPriority(RenderPriority renderPriority, out Color color) => this._renderPriorityToColor.TryGetValue(renderPriority, out color);
}