namespace Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// Four sided padding.
/// </summary>
[DataContract]
public readonly struct Padding {
    /// <summary>
    /// The left padding.
    /// </summary>
    [DataMember]
    public readonly float Left;

    /// <summary>
    /// The top padding.
    /// </summary>
    [DataMember]
    public readonly float Top;

    /// <summary>
    /// The right padding.
    /// </summary>
    [DataMember]
    public readonly float Right;

    /// <summary>
    /// The bottom padding.
    /// </summary>
    [DataMember]
    public readonly float Bottom;

    /// <summary>
    /// Initializes a new instance of <see cref="Padding" />.
    /// </summary>
    public Padding() : this(0f, 0f, 0f, 0f) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Padding" />.
    /// </summary>
    /// <param name="horizontal">The left and right padding.</param>
    /// <param name="vertical">The top and bottom padding.</param>
    public Padding(float horizontal, float vertical) : this(horizontal, vertical, horizontal, vertical) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Padding" />.
    /// </summary>
    /// <param name="left">The left padding.</param>
    /// <param name="top">The top padding.</param>
    /// <param name="right">The right padding.</param>
    /// <param name="bottom">The bottom padding.</param>
    public Padding(float left, float top, float right, float bottom) {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
    }
}