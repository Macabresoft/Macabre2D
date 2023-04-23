namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// Represents analog input in the cardinal directions.
/// </summary>
[DataContract]
public class CardinalAnalogInput {
    /// <summary>
    /// Gets or sets the <see cref="InputAction" /> for down on this analog stick.
    /// </summary>
    [DataMember]
    public InputAction Down { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not this analog stick is enabled.
    /// </summary>
    [DataMember]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the <see cref="InputAction" /> for left on this analog stick.
    /// </summary>
    [DataMember]
    public InputAction Left { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="InputAction" /> for right on this analog stick.
    /// </summary>
    [DataMember]
    public InputAction Right { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="InputAction" /> for up on this analog stick.
    /// </summary>
    [DataMember]
    public InputAction Up { get; set; }

    /// <summary>
    /// Copies these analog input settings to another analog input.
    /// </summary>
    /// <param name="other">The other analog input.</param>
    public void CopyTo(CardinalAnalogInput other) {
        other.IsEnabled = this.IsEnabled;
        other.Left = this.Left;
        other.Up = this.Up;
        other.Right = this.Right;
        other.Down = this.Down;
    }
}