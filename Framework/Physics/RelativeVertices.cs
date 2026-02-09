namespace Macabre2D.Framework;

using System.Collections.Generic;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A relative list of vertices, each one being added onto the next to create a line strip.
/// </summary>
public class RelativeVertices : ObservableCollectionExtended<Vector2> {
    /// <summary>
    /// Returns the position of each vertex in the line strip relative to the line strip's origin, but not relative to one another.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Vector2> ToAbsolute() {
        var vertices = new List<Vector2>();
        var previousVector = Vector2.Zero;

        foreach (var relativeVertex in this.Items) {
            previousVector += relativeVertex;
            vertices.Add(previousVector);
        }

        return vertices;
    }
}