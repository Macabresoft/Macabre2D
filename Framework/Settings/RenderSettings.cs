namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

/// <summary>
/// Settings for rendering. This includes colors and shaders.
/// </summary>
[DataContract]
public class RenderSettings {
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<RenderPriority, Color> _renderPriorityToColor = [];

    private readonly Dictionary<RenderPriority, Guid> _renderPriorityToShaderId = [];

    /// <summary>
    /// Called when a color changes for a specific <see cref="RenderPriority" />.
    /// </summary>
    public event EventHandler<RenderPriority>? ColorChanged;

    /// <summary>
    /// Called when a shader changes for a specific <see cref="RenderPriority" />.
    /// </summary>
    public event EventHandler<RenderPriority>? ShaderChanged;

    /// <summary>
    /// Clears color overrides for all render priorities.
    /// </summary>
    public void Clear() {
        this._renderPriorityToColor.Clear();
        this._renderPriorityToShaderId.Clear();
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public RenderSettings Clone() {
        var settings = new RenderSettings();
        this.CopyTo(settings);
        return settings;
    }

    /// <summary>
    /// Copies color settings to another instance.
    /// </summary>
    /// <param name="other">The other instance.</param>
    public void CopyTo(RenderSettings other) {
        other._renderPriorityToColor.Clear();
        foreach (var entry in this._renderPriorityToColor) {
            other._renderPriorityToColor[entry.Key] = entry.Value;
        }

        foreach (var entry in this._renderPriorityToShaderId) {
            other._renderPriorityToShaderId[entry.Key] = entry.Value;
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
    /// Removes shader override for the specified render priority.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    public void RemoveRenderPriorityShader(RenderPriority renderPriority) {
        this._renderPriorityToShaderId.Remove(renderPriority);
        this.ShaderChanged.SafeInvoke(this, renderPriority);
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
    /// Sets the shader for a specified <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <param name="shaderId">The shader identifier.</param>
    public void SetRenderPriorityShader(RenderPriority renderPriority, Guid shaderId) {
        this._renderPriorityToShaderId[renderPriority] = shaderId;
        this.ShaderChanged.SafeInvoke(this, renderPriority);
    }

    /// <summary>
    /// Tries to get the color for the specified <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <param name="color">The found color.</param>
    /// <returns>A value indicating whether this render priority has an associated color.</returns>
    public bool TryGetColorForRenderPriority(RenderPriority renderPriority, out Color color) => this._renderPriorityToColor.TryGetValue(renderPriority, out color);

    /// <summary>
    /// Tries to get the shader identifier for the specified <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <param name="shaderId">The found shader identifier.</param>
    /// <returns>A value indicating whether this render priority has an associated shader.</returns>
    public bool TryGetShaderIdForRenderPriority(RenderPriority renderPriority, out Guid shaderId) => this._renderPriorityToShaderId.TryGetValue(renderPriority, out shaderId);
}