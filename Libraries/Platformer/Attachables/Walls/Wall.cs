namespace Macabresoft.Macabre2D.Libraries.Platformer.Walls;

using System.Runtime.Serialization;

/// <summary>
/// A wall to which actors can attach.
/// </summary>
public class Wall : Attachable {
    /// <summary>
    /// Gets or sets a value indicating whether or not this wall is a ladder.
    /// </summary>
    [DataMember]
    public bool IsLadder { get; set; }
}