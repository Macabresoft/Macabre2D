namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// Represents analog input in the cardinal directions.
/// </summary>
[DataContract]
public class CardinalAnalogInput {
    /// <summary>
    /// Gets the <see cref="InputAction" /> for down on this analog stick.
    /// </summary>
    [DataMember]
    public InputAction Down { get; set; }

    /// <summary>
    /// Gets the <see cref="InputAction" /> for left on this analog stick.
    /// </summary>
    [DataMember]
    public InputAction Left { get; set; }

    /// <summary>
    /// Gets the <see cref="InputAction" /> for right on this analog stick.
    /// </summary>
    [DataMember]
    public InputAction Right { get; set; }

    /// <summary>
    /// Gets the <see cref="InputAction" /> for up on this analog stick.
    /// </summary>
    [DataMember]
    public InputAction Up { get; set; }
}