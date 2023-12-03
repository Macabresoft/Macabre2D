namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// An observable collection of <see cref="AutoTileSet" />.
/// </summary>
[DataContract]
[Category("Auto Tile Sets")]
public class AutoTileSetCollection : SpriteSheetMemberCollection<AutoTileSet> {
    /// <inheritdoc />
    public override string Name => "Auto Tile Sets";
}