namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using Macabresoft.Core;

/// <summary>
/// A collection of shaders stored at the project level.
/// </summary>
public class ScreenShaderCollection : ObservableCollectionExtended<ScreenShader>, INameableCollection {

    /// <inheritdoc />
    public string Name => "Screen Shaders";

    /// <summary>
    /// Checks whether this collection has any enabled shaders.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <returns>A value indicating whether this has enabled shaders.</returns>
    public bool CheckHasEnabledShaders(IGame game) {
        if (this.Count > 0) {
            foreach (var shader in this) {
                if (shader.IsEnabled && shader.Shader.Asset != null && !game.DisplaySettings.DisabledScreenShaders.Contains(shader.Id)) {
                    return true;
                }
            }
        }

        return false;
    }

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();
}