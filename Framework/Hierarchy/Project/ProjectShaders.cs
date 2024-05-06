namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A collection of shaders stored at the project level.
/// </summary>
public class ProjectShaders : ObservableCollectionExtended<ProjectShader>, IEnableable, INameableCollection {

    /// <inheritdoc />
    public string Name => "Screen Shaders";

    /// <inheritdoc />
    [DataMember]
    public bool IsEnabled { get; set; }

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();
}