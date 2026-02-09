namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;

/// <summary>
/// A template for the default scene that can handle updates, rendering, input, and physics.
/// </summary>
public class DefaultSceneTemplate : SceneTemplate {
    /// <inheritdoc />
    public override string Name => "Default Scene";
    
    /// <inheritdoc />
    public override IReadOnlyCollection<Type> DefaultSystems { get; } = [typeof(InputSystem), typeof(UpdateSystem), typeof(AnimationSystem), typeof(SimplePhysicsSystem), typeof(RenderSystem)];
}