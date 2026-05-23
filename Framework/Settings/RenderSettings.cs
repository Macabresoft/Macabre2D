namespace Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabre2D.Common;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

/// <summary>
/// Settings for rendering. This includes colors and shaders.
/// </summary>
[DataContract]
[Category("Render Settings")]
public class RenderSettings : CopyableSettings<RenderSettings>, INameableSettings {

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<RenderPriority, BlendStateType> _renderPriorityToBlendStateType = [];

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<RenderPriority, Color> _renderPriorityToColor = [];

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<RenderPriority, ShaderReference> _renderPriorityToScreenSpaceShaderReference = [];

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<RenderPriority, ShaderReference> _renderPriorityToShaderReference = [];

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private readonly Dictionary<RenderPriority, bool> _renderPriorityToShareShader = [];

    /// <inheritdoc />
    public string Name => "Render Settings";

    /// <summary>
    /// Gets a value indicating whether a given <see cref="RenderPriority" /> should share the same shader in screen space as it does in the internal render.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <returns>A value indicating whether to use the same shader in screen space.</returns>
    public bool CheckIfRenderPriorityShaderSharedWithScreenSpace(RenderPriority renderPriority) => this._renderPriorityToShareShader.TryGetValue(renderPriority, out var value) && value;

    /// <summary>
    /// Clears color overrides for all render priorities.
    /// </summary>
    public void Clear() {
        this._renderPriorityToBlendStateType.Clear();
        this._renderPriorityToColor.Clear();
        this._renderPriorityToShaderReference.Clear();
    }

    /// <inheritdoc />
    public override void CopyTo(RenderSettings other) {
        base.CopyTo(other);

        other._renderPriorityToBlendStateType.Clear();
        foreach (var entry in this._renderPriorityToBlendStateType) {
            other._renderPriorityToBlendStateType[entry.Key] = entry.Value;
        }

        other._renderPriorityToColor.Clear();
        foreach (var entry in this._renderPriorityToColor) {
            other._renderPriorityToColor[entry.Key] = entry.Value;
        }

        other._renderPriorityToShaderReference.Clear();
        foreach (var entry in this._renderPriorityToShaderReference) {
            other._renderPriorityToShaderReference[entry.Key] = entry.Value;
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
    /// Gets the <see cref="BlendState" /> for a given <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <returns>The blend state.</returns>
    public BlendState GetRenderPriorityBlendState(RenderPriority renderPriority) => this._renderPriorityToBlendStateType.TryGetValue(renderPriority, out var blendStateType) ? blendStateType.ToBlendState() : BlendState.AlphaBlend;

    /// <summary>
    /// Gets the <see cref="BlendStateType" /> for a given <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <returns>The blend state type.</returns>
    public BlendStateType GetRenderPriorityBlendStateType(RenderPriority renderPriority) => this._renderPriorityToBlendStateType.GetValueOrDefault(renderPriority, BlendStateType.AlphaBlend);

    /// <summary>
    /// Tries to get the screen space shader for the specified <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <returns>The existing screen space shader reference or a new one associated with the specified <see cref="RenderPriority" />.</returns>
    public ShaderReference GetScreenSpaceShaderForRenderPriority(RenderPriority renderPriority) {
        if (!this.CheckIfRenderPriorityShaderSharedWithScreenSpace(renderPriority)) {
            if (!this._renderPriorityToScreenSpaceShaderReference.TryGetValue(renderPriority, out var shaderReference)) {
                shaderReference = new ShaderReference();
                this._renderPriorityToScreenSpaceShaderReference[renderPriority] = shaderReference;
            }

            return shaderReference;
        }

        return this.GetShaderForRenderPriority(renderPriority);
    }

    /// <summary>
    /// Tries to get the shader for the specified <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <returns>The existing shader reference or a new one associated with the specified <see cref="RenderPriority" />.</returns>
    public ShaderReference GetShaderForRenderPriority(RenderPriority renderPriority) {
        if (!this._renderPriorityToShaderReference.TryGetValue(renderPriority, out var shaderReference)) {
            shaderReference = new ShaderReference();
            this._renderPriorityToShaderReference[renderPriority] = shaderReference;
        }

        return shaderReference;
    }

    /// <summary>
    /// Removes color override for the specified render priority.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    public void RemoveRenderPriorityColor(RenderPriority renderPriority) {
        this._renderPriorityToColor.Remove(renderPriority);
    }

    /// <summary>
    /// Sets the <see cref="BlendStateType" /> for a given <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <param name="blendStateType">The blend state type.</param>
    public void SetRenderPriorityBlendState(RenderPriority renderPriority, BlendStateType blendStateType) {
        this._renderPriorityToBlendStateType[renderPriority] = blendStateType;
    }

    /// <summary>
    /// Sets the color for a specified <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <param name="color">The color.</param>
    public void SetRenderPriorityColor(RenderPriority renderPriority, Color color) {
        this._renderPriorityToColor[renderPriority] = color;
    }

    /// <summary>
    /// Sets a value indicating whether a given <see cref="RenderPriority" /> should use the same shader in screen space as it does in the internal render.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <param name="useSameShaderForScreenSpace">A value indicating whether to use the same shader in screen space.</param>
    public void SetShareRenderPriorityShaderWithScreenSpace(RenderPriority renderPriority, bool useSameShaderForScreenSpace) {
        this._renderPriorityToShareShader[renderPriority] = useSameShaderForScreenSpace;
    }

    /// <summary>
    /// Tries to get the color for the specified <see cref="RenderPriority" />.
    /// </summary>
    /// <param name="renderPriority">The render priority.</param>
    /// <param name="color">The found color.</param>
    /// <returns>A value indicating whether this render priority has an associated color.</returns>
    public bool TryGetColorForRenderPriority(RenderPriority renderPriority, out Color color) => this._renderPriorityToColor.TryGetValue(renderPriority, out color);
}