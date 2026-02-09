namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;

/// <summary>
/// Interface for a scene template.
/// </summary>
public interface ISceneTemplate {
    /// <summary>
    /// Gets the name of this template.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a list of the default systems to be added to scenes created with this template.
    /// </summary>
    IReadOnlyCollection<Type> DefaultSystems { get; }
}

/// <summary>
/// A scene template.
/// </summary>
public abstract class SceneTemplate : ISceneTemplate {

    /// <inheritdoc />
    public abstract string Name { get; }
    
    /// <inheritdoc />
    public abstract IReadOnlyCollection<Type> DefaultSystems { get; }
}