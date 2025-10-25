namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;

/// <summary>
/// A template for a completely empty scene.
/// </summary>
public class EmptySceneTemplate : SceneTemplate {

    /// <inheritdoc />
    public override string Name => "Empty Scene";

    /// <inheritdoc />
    public override IReadOnlyCollection<Type> DefaultSystems { get; } = [];
}