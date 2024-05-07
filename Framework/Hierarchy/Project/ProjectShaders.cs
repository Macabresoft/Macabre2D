namespace Macabresoft.Macabre2D.Framework;

using System.Collections;using System.Collections.Generic;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A collection of shaders stored at the project level.
/// </summary>
public class ProjectShaders : ObservableCollectionExtended<ProjectShader>, INameableCollection {

    /// <inheritdoc />
    public string Name => "Screen Shaders";

    /// <summary>
    /// Gets a value indicating whether or not this has shaders.
    /// </summary>
    [DataMember]
    public bool HasShaders {
        get {
            if (this.Count > 0) {
                foreach (var shader in this) {
                    if (shader.Shader.Asset != null) {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();
}